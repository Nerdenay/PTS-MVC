using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;

namespace PatientTrackingSite.Controllers
{
    public class PatientController : Controller
    {
        private readonly PTSDBContext _context;

        public PatientController(PTSDBContext context)
        {
            _context = context;
        }

        // Doktorları listele
        public IActionResult Doctors()
        {
            var doctors = _context.Users
                .Where(u => u.Role == "Doctor")
                .ToList();

            return View(doctors);
        }

        // Reçeteleri listele
        public IActionResult Medication()
        {
            // Giriş yapan hastanın ID'sini session'dan al
            var patientId = HttpContext.Session.GetInt32("UserId");

            if (patientId == null)
                return RedirectToAction("Login", "Account");  // Giriş yapılmamışsa login sayfasına yönlendir

            var medications = _context.Medications
                .Where(m => m.PatientId == patientId)
                .Include(m => m.Doctor)  // Reçeteyi yazan doktor bilgisi
                .ToList();
            // Ayrıca doktor reçeteyi oluştururken DoctorId değerini de veritabanına kaydediyor olman gerekir. !!!

            return View(medications);
        }
    }
}
