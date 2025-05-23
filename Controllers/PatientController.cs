﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;
using PatientTrackingSite.Helpers;
using System.Reflection.Metadata.Ecma335;

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

            var result = allSlots.Select(slot => new
            {
                time = slot.ToString(@"hh\:mm"),
                isAvailable = !takenSlots.Contains(slot)
            }).ToList();

            return Json(new { success = true, timeSlots = result });
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

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); 
            }

            var patientId = HttpContext.Session.GetInt32("UserId");

            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var date = model.AppointmentDate.Date;
            var time = TimeSpan.Parse(model.TimeSlot); 

            var finalDateTime = model.AppointmentDate.Date + time;


            var sameDayAppointmentCount = _context.Appointments.Count(a => 
                a.PatientId == patientId.Value &&
                a.DoctorId == model.DoctorId &&
                a.AppointmentDate.Date == model.AppointmentDate.Date);



            if (sameDayAppointmentCount >= 1)  // aynı doktordan aynı gün 1 randevu almasını sağladım burda da
            {
                ModelState.AddModelError("", "You can take only 1 appointment on the same day.");

                var specializations = _context.Users
                    .Where(u => u.Role == "Doctor" && u.Specialization != null)
                    .Select(u => u.Specialization)
                    .Distinct()
                    .ToList();

                ViewBag.Specializations = new SelectList(specializations);
                return View(model);
            }





            var totalAppointmentsThatDay = _context.Appointments.Count(a =>
                 a.PatientId == patientId && a.AppointmentDate.Date == date);


            if (totalAppointmentsThatDay >= 2) // Burda amaç hasta aynı gün hastaneden 2 randevu alabilir aynı branş başka hocalar olablr ona validation yapadım
            {
                ModelState.AddModelError("", "You have reached your daily appointment limit. Please choose another day.");

                var specializations = _context.Users
                    .Where(u => u.Role == "Doctor" && u.Specialization != null)
                    .Select(u => u.Specialization)
                    .Distinct()
                    .ToList();

                ViewBag.Specializations = new SelectList(specializations);
                return View(model);
            }

            // Eğer o saatte bu doktora ait bir randevu varsa, hata döndür

            bool isSlotTaken = _context.Appointments.Any(a =>
                a.DoctorId == model.DoctorId &&
                a.AppointmentDate == finalDateTime);

            if (isSlotTaken)
            {
                ModelState.AddModelError(string.Empty, "This time slot is already taken.");

                // Dropdown'ların yeniden yüklenmesini sağla

                var specializations = _context.Users
                    .Where(u => u.Role == "Doctor" && u.Specialization != null)
                    .Select(u => u.Specialization)
                    .Distinct()
                    .ToList();
                ViewBag.Specializations = new SelectList(specializations);

                return View(model);
            }


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

            TempData["SuccessMessage"] = "Your appointment was done. Pending.";

            return RedirectToAction("Index", "Patient");
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




        // My Appointments ---------------------------------------------------------------------------------------


        public IActionResult MyAppointments()

        {
            // Giriş yapan hastanın ID'sini session'dan al
            var patientId = HttpContext.Session.GetInt32("UserId");


            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);

        }


        [HttpPost]
        public IActionResult DeleteAppointment(int id)
        {
            var patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null)
                return RedirectToAction("Login", "Account");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.Id == id && a.PatientId == patientId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "There is no appointment.";
                return RedirectToAction("MyAppointments");
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Appointment Deleted.";
            return RedirectToAction("MyAppointments");
        }


    }
}
