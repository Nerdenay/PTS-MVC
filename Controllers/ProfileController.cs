﻿using Microsoft.AspNetCore.Mvc;
using PatientTrackingSite.Models;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace PatientTrackingSite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly PTSDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(PTSDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

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

            // Şifre güncelleme (hash ile)
            if (!string.IsNullOrWhiteSpace(model.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(model.NewPassword) &&
                !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                var hasher = new PasswordHasher<User>();

                // Mevcut şifreyi doğrula
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
                if (result != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                    return View(model);
                }

                // Yeni şifreyi hashle
                user.PasswordHash = hasher.HashPassword(user, model.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }
    }
}
