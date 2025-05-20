using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace PatientTrackingSite.Controllers
{
    public class PatientProfileController : Controller
    {
        private readonly PTSDBContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientProfileController(PTSDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> PatientProfile()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            if (id == null || HttpContext.Session.GetString("UserRole") != "Patient")
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == "Patient");
            if (user == null) return NotFound();

            var model = new PatientProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                BirthDate = user.BirthDate,
                Address = user.Address,
                TCNo = user.TCNo,
                Gender = user.Gender,
                ProfileImagePath = user.ProfileImagePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PatientProfile(PatientProfileViewModel model)
        {
            ModelState.Remove("ProfileImagePath");
            ModelState.Remove("ProfileImageFile");

            if (string.IsNullOrWhiteSpace(model.CurrentPassword) &&
                string.IsNullOrWhiteSpace(model.NewPassword) &&
                string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id && u.Role == "Patient");
            if (user == null) return NotFound();

            // Update basic fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.BirthDate = model.BirthDate;
            user.Address = model.Address;
            user.TCNo = model.TCNo;

            // Handle profile image upload
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.ProfileImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ProfileImageFile.CopyToAsync(stream);

                user.ProfileImagePath = "/uploads/" + fileName;
            }

            // Handle password update with hashing
            if (!string.IsNullOrWhiteSpace(model.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(model.NewPassword) &&
                !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

                if (result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                    return View(model);
                }

                user.PasswordHash = hasher.HashPassword(user, model.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("PatientProfile", "PatientProfile");
        }
    }
}
