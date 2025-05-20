using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;
using PatientTrackingSite.Helpers;

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

            var healthTips = new List<string>
                {
                    "Drinking enough water every day can improve your focus and mood.",
                    "Regular exercise boosts your immune system.",
                    "Eating fruits and vegetables helps prevent chronic diseases.",
                    "Getting 7-8 hours of sleep per night improves mental health.",
                    "Take short walks to reduce the effects of a sedentary lifestyle.",
                    "Wash your hands regularly to prevent infections.",
                    "Avoid sugary drinks for better heart health."
                };


            var random = new Random();
            int index = random.Next(healthTips.Count);
            ViewBag.HealthTip = healthTips[index];


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

        [HttpGet]
        public JsonResult GetAvailableTimeSlots(int doctorId, DateTime selectedDate)
        {
            if (selectedDate.Date < DateTime.Today || selectedDate.DayOfWeek == DayOfWeek.Saturday || selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json(new { success = false, message = "Invalid date." });
            }

            var allSlots = AppointmentHelper.GetAvailableTimeSlots();

            var takenSlots = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == selectedDate.Date)
                .Select(a => a.AppointmentDate.TimeOfDay)
                .ToList();

            var availableSlots = allSlots.Except(takenSlots)
                .Select(t => t.ToString(@"hh\:mm"))
                .ToList();

            return Json(new { success = true, timeSlots = availableSlots });
        }

        [HttpGet]
        public JsonResult GetDoctorsBySpecialization(string specialization)
        {
            var doctors = _context.Users

               .Where(u => u.Role == "Doctor" && u.Specialization != null && u.Specialization.ToLower() == specialization.ToLower())

                .Select(u => new
                {
                    u.Id,
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToList();

            return Json(doctors);
        }


        [HttpGet]
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

            var date = model.AppointmentDate.Date;
            var time = TimeSpan.Parse(model.TimeSlot); // "08:30" gibi

            var finalDateTime = model.AppointmentDate.Date + time;


            var appointment = new Appointment
            {
                PatientId = patientId.Value,
                DoctorId = model.DoctorId,
                AppointmentDate = finalDateTime,
                Notes = model.Notes,
                Status = "Scheduled"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Appointments", "Patient");
        }



        // Diseases ------------------------------------------------------------------------------------------

        public IActionResult Diseases()
        {
            var patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var diseases = _context.Diseases
                .Where(d => d.PatientId == patientId)
                .ToList();

            return View(diseases);
        }




        // My Appointments ---------------------------------------------------------------------------------





    }
}
