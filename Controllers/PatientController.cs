using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using System;

namespace PatientTrackingSite.Controllers
{
    public class PatientController : Controller

    {
        private readonly PTSDBContext _context;

        public PatientController(PTSDBContext context)
        {
            _context = context;
        }

        public IActionResult Doctors()
        {
            var doctors = _context.Users
           .Where(u => u.Role == "Doctor")
           .ToList();

            return View(doctors); ;
        }

        public IActionResult Medication()
        {

            // Simülasyon: Giriş yapan kullanıcının hasta ID’si
            var patientId = HttpContext.Session.GetInt32("UserId");

            if (patientId == null)
                return RedirectToAction("Login", "Account");  // Girii yapmamışsa yapsın

            var medications = _context.Medications
                .Where(m => m.PatientId == patientId)
                .Include(m => m.Patient)
                .ToList();

            return View(medications);



        }
    }
}
