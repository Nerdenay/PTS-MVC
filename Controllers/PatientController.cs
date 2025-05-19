using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;

namespace PatientTrackingSite.Controllers
{
    public class PatientController : Controller
    {
        private readonly PTSDBContext _context;

        public PatientController(PTSDBContext context)
        {
            _context = context;
        }


        //Homepage ------------------------------------------------------------------------------------------


        public IActionResult Index()
        {

            int? patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null || HttpContext.Session.GetString("UserRole") != "Patient")
                return RedirectToAction("Login", "Account");

            return View();
        }



        // Doctors ------------------------------------------------------------------------------------------
        public IActionResult Doctors()
        {
            var doctors = _context.Users
                .Where(u => u.Role == "Doctor")
                .ToList();

            return View(doctors);
        }


        // Medications ---------------------------------------------------------------------------------------
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



        // Medical imagess ---------------------------------------------------------------------------------

        public IActionResult MedicalImages()
        {
            var patientId = HttpContext.Session.GetInt32("UserId"); // Giriş yapan hastanın ID'sini session'dan al

            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var images = _context.MedicalImages
                .Where(m => m.PatientId == patientId)
                .ToList();

            return View(images);
        }


        // Appointments -------------------------------------------------------------------------------------

        public IActionResult MakeAppointment()
        {
            // Tüm doktorlardan branşları al ve tekilleştir
            var specializations = _context.Users
                .Where(u => u.Role == "Doctor" && u.Specialization != null)
                .Select(u => u.Specialization)
                .Distinct()
                .ToList();

            ViewBag.Specializations = new SelectList(specializations);

            return View();
        }

        [HttpGet]
        public JsonResult GetDoctorsBySpecialization(string specialization)
        {
            var doctors = _context.Users
                .Where(u => u.Role == "Doctor" && u.Specialization.ToLower() == specialization.ToLower())
                .Select(u => new
                {
                    u.Id,
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToList();

            return Json(doctors);
        }

        [HttpPost]
        public IActionResult MakeAppointment(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var specializations = _context.Users
                .Where(u => u.Role == "Doctor" && u.Specialization != null)
                .Select(u => u.Specialization)
                 .Distinct()
                 .ToList();
                ViewBag.Specializations = new SelectList(specializations);
                return View(model);
            }

            var patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var appointment = new Appointment
            {
                PatientId = patientId.Value,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Notes = model.Notes,
                Status = "Scheduled"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Appointments", "Patient");
        }


        // Diseases ------------------------------------------------------------------------------------------








        // My Appointments ---------------------------------------------------------------------------------





    }
}
