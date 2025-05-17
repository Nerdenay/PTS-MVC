using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;

namespace PatientTrackingSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly PTSDBContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(PTSDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int doctorId = 1; // Test için sabit
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == doctorId && u.Role == "Doctor");
            if (user == null) return NotFound();

            var model = new DoctorProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                BirthDate = user.BirthDate,
                Address = user.Address,
                TCNo = user.TCNo,
                ProfileImagePath = user.ProfileImagePath,
                Specialization = user.Specialization

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(DoctorProfileViewModel model)
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id && u.Role == "Doctor");
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.BirthDate = model.BirthDate;
            user.Address = model.Address;
            user.TCNo = model.TCNo;
            user.Specialization = model.Specialization;

            Console.WriteLine("Fotoğraf geldi mi?: " + (model.ProfileImageFile != null ? "Evet" : "Hayır"));
            Console.WriteLine("Dosya adı: " + model.ProfileImageFile?.FileName);

            if (model.ProfileImageFile != null)
            {
                Console.WriteLine("Length: " + model.ProfileImageFile.Length);
            }


            // Profil resmi
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                user.ProfileImagePath = "/uploads/" + fileName;
            }
            else
            {
                Console.WriteLine("Yeni fotoğraf seçilmedi, mevcut fotoğraf korunuyor.");
            }


            // Şifre güncelle
            if (!string.IsNullOrWhiteSpace(model.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(model.NewPassword) &&
                !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (user.PasswordHash != model.CurrentPassword) // NOT: hash kontrolü eklenmeli
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre hatalı.");
                    return View(model);
                }

                user.PasswordHash = model.NewPassword;
            }

            _context.Users.Update(user);

            Console.WriteLine("Veritabanına kayıt yapılacak path: " + user.ProfileImagePath);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }
    }
}
