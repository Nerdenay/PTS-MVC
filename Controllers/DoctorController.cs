using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace PatientTrackingSite.Controllers
{
    public class DoctorController : Controller
    {
        private readonly PTSDBContext _context;

        private readonly IWebHostEnvironment _env;


        public DoctorController(IWebHostEnvironment env, PTSDBContext context)
        {
            _context = context;
            _env = env;
        }

        // DashBoard ------------------------------------------------------------------------------------------

        public IActionResult Index()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            var today = DateTime.Today;
            var future14End = today.AddDays(14);
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Doktorun hastaları
            var patientIds = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToList();

            ViewBag.TodayAppointments = _context.Appointments
                .Count(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today);

            ViewBag.TotalPatients = patientIds.Count;

            ViewBag.TotalPrescriptions = _context.Medications
                .Count(m => patientIds.Contains(m.PatientId));

            ViewBag.TotalDiseases = _context.Diseases
                .Count(d => patientIds.Contains(d.PatientId));

            ViewBag.TotalMedicalImages = _context.MedicalImages
                .Count(i => patientIds.Contains(i.PatientId));

            ViewBag.UpcomingAppointments = _context.Appointments
                .Count(a => a.DoctorId == doctorId &&
                            a.AppointmentDate.Date > today &&
                            a.AppointmentDate.Date <= future14End);

            var busiestUpcomingDay = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.AppointmentDate.Date > today &&
                            a.AppointmentDate.Date <= future14End)
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            ViewBag.BusiestUpcomingDayDate = busiestUpcomingDay?.Date.ToString("dd.MM.yyyy") ?? "-";
            ViewBag.BusiestUpcomingDayCount = busiestUpcomingDay?.Count ?? 0;

            return View();
        }


        // Patients  ------------------------------------------------------------------------------------------

        public IActionResult MyPatients()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");


            var patients = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .GroupBy(a => a.PatientId)
                .Select(g => new PatientListItemViewModel
                {
                    PatientId = g.Key,
                    FirstName = g.First().Patient.FirstName,
                    LastName = g.First().Patient.LastName,
                    AppointmentCount = g.Count()
                })
                .ToList();

            return View(patients);
        }

        // Patient Details ------------------------------------------------------------------------------------------

        public IActionResult PatientDetails(int id)
        {
            var patient = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Patient");
            if (patient == null) return NotFound();

            var medications = _context.Medications
                .Where(m => m.PatientId == id)
                .OrderByDescending(m => m.Id)
                .ToList();

            var diseases = _context.Diseases
                .Where(d => d.PatientId == id)
                .OrderByDescending(d => d.Id)
                .ToList();

            var images = _context.MedicalImages
                .Where(i => i.PatientId == id)
                .OrderByDescending(i => i.Id)
                .ToList();

            var appointmentCount = _context.Appointments
                .Count(a => a.PatientId == id);

            var viewModel = new PatientDetailViewModel
            {
                Patient = patient,
                Medications = medications,
                Diseases = diseases,
                MedicalImages = images,
                AppointmentCount = appointmentCount
            };

            return View(viewModel);
        }


        // Appointments  ------------------------------------------------------------------------------------------

        public IActionResult Appointments()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");


            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = start.AddMonths(1);

            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate >= start && a.AppointmentDate < end)
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        // Edit Appointments  ------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult EditAppointment(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(Appointment updatedAppointment)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == updatedAppointment.Id);
            if (appointment == null) return NotFound();

            appointment.AppointmentDate = updatedAppointment.AppointmentDate;
            appointment.Status = updatedAppointment.Status;
            appointment.Notes = updatedAppointment.Notes;

            _context.SaveChanges();

            return RedirectToAction("Appointments");
        }

        // CreatePrescription  ------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult CreatePrescription()
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var patients = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .ToList();

            var model = new PrescriptionViewModel
            {
                PatientList = patients.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult CreatePrescription(PrescriptionViewModel model)
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");

            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            // Hasta yetkilendirmesi
            bool isValidPatient = _context.Appointments.Any(a =>
                a.DoctorId == doctorId && a.PatientId == model.SelectedPatientId);

            if (!isValidPatient)
            {
                TempData["ValidationError"] = "Please select a valid patient.";
                return RedirectToAction("CreatePrescription");
            }

            // ModelState hatalıysa
            if (!ModelState.IsValid)
            {
                TempData["ValidationError"] = "Please fill in all required fields.";
                return RedirectToAction("CreatePrescription");
            }

            // Kayıt işlemi
            var medication = new Medication
            {
                Name = model.MedicationName,
                Dosage = model.Dosage,
                Instructions = model.Instructions,
                PatientId = model.SelectedPatientId.Value,
                DoctorId = doctorId.Value
            };

            _context.Medications.Add(medication);
            _context.SaveChanges();

            return RedirectToAction("MyPatients");
        }


        // Edit Prescription  ------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult EditPrescription(int id)
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            var medication = _context.Medications.FirstOrDefault(m => m.Id == id && m.DoctorId == doctorId);
            if (medication == null) return Unauthorized();

            var model = new PrescriptionViewModel
            {
                SelectedPatientId = medication.PatientId,
                MedicationName = medication.Name,
                Dosage = medication.Dosage,
                Instructions = medication.Instructions,
                PatientList = new List<SelectListItem> // sadece gösterim için, seçim yapılmayacak
                {
                    new SelectListItem
                    {
                        Value = medication.PatientId.ToString(),
                        Text = _context.Users.Where(u => u.Id == medication.PatientId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault()
                    }
                }
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult EditPrescription(int id, PrescriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PatientList = _context.Users
                    .Where(u => u.Role == "Patient")
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FirstName + " " + u.LastName
                    }).ToList();

                return View(model);

            }

            var medication = _context.Medications.FirstOrDefault(m => m.Id == id);
            if (medication == null) return NotFound();

            medication.Name = model.MedicationName;
            medication.Dosage = model.Dosage;
            medication.Instructions = model.Instructions;

            _context.SaveChanges();

            return RedirectToAction("PatientDetails", new { id = medication.PatientId });
        }


        // UploadMedicalFile  ------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult UploadMedicalFile()
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var patients = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .ToList();

            var model = new MedicalImageUploadViewModel
            {
                PatientList = patients.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UploadMedicalFile(MedicalImageUploadViewModel model)
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Hasta kontrolü
            bool isValidPatient = _context.Appointments.Any(a =>
                a.DoctorId == doctorId && a.PatientId == model.SelectedPatientId);

            if (!isValidPatient)
            {
                TempData["ValidationError"] = "Please select a valid patient.";
                return RedirectToAction("UploadMedicalFile");
            }

            // ModelState kontrolü (zorunlu alanlar eksikse)
            if (!ModelState.IsValid)
            {
                TempData["ValidationError"] = "Please fill in all required fields and upload a valid file.";
                return RedirectToAction("UploadMedicalFile");
            }

            // Klasör ve dosya kaydı
            var uploadsFolder = Path.Combine(_env.WebRootPath, "medical-files");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            // Veritabanı kaydı
            var medicalImage = new MedicalImage
            {
                PatientId = model.SelectedPatientId.Value,
                ImagePath = "/medical-files/" + uniqueName,
                Description = model.Description,
                UploadedAt = DateTime.Now
            };

            _context.MedicalImages.Add(medicalImage);
            await _context.SaveChangesAsync();

            return RedirectToAction("PatientDetails", new { id = model.SelectedPatientId });
        }

        // CreateDisease ------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult CreateDisease()
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var patients = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .ToList();

            var model = new DiseaseCreateViewModel
            {
                PatientList = patients.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateDisease(DiseaseCreateViewModel model)
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");

            if (doctorId == null || HttpContext.Session.GetString("UserRole") != "Doctor")
                return RedirectToAction("Login", "Account");

            // Yetkisiz hasta kontrolü
            bool isValidPatient = _context.Appointments.Any(a =>
                a.DoctorId == doctorId && a.PatientId == model.SelectedPatientId);

            if (!isValidPatient)
            {
                ModelState.AddModelError(nameof(model.SelectedPatientId), "Please select a valid patient.");
            }

            if (!ModelState.IsValid)
            {
                // PatientList'i yeniden doldur
                var patients = _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .Select(a => a.Patient)
                    .Distinct()
                    .ToList();

                model.PatientList = patients.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                }).ToList();

                return View(model); 
            }

            // Kayıt işlemi
            var disease = new Disease
            {
                Name = model.Name,
                Description = model.Description,
                PatientId = model.SelectedPatientId.Value
            };

            _context.Diseases.Add(disease);
            _context.SaveChanges();

            return RedirectToAction("MyPatients");
        }





        // EditDisease ------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult EditDisease(int id)
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var disease = _context.Diseases
                .Include(d => d.Patient)
                .FirstOrDefault(d => d.Id == id);

            bool isValid = _context.Appointments.Any(a =>
                a.DoctorId == doctorId && a.PatientId == disease.PatientId);

            if (!isValid)
                return Unauthorized();

            var model = new DiseaseCreateViewModel
            {
                Id = disease.Id,
                Name = disease.Name,
                Description = disease.Description,
                SelectedPatientId = disease.PatientId,
                PatientList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = disease.PatientId.ToString(),
                    Text = disease.Patient.FirstName + " " + disease.Patient.LastName
                }
            }
                        };

                        return View(model);
        }

        [HttpPost]
        public IActionResult EditDisease(DiseaseCreateViewModel model)
        {
            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            bool isValid = _context.Appointments.Any(a =>
                a.DoctorId == doctorId && a.PatientId == model.SelectedPatientId);

            if (!isValid)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                model.PatientList = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = model.SelectedPatientId.ToString(),
                            Text = _context.Users
                                .Where(u => u.Id == model.SelectedPatientId)
                                .Select(u => u.FirstName + " " + u.LastName)
                                .FirstOrDefault()
                        }
                    };

                return View(model);
            }

            var disease = _context.Diseases.FirstOrDefault(d => d.Id == model.Id);
            if (disease == null) return NotFound();

            disease.Name = model.Name;
            disease.Description = model.Description;

            _context.SaveChanges();

            return RedirectToAction("PatientDetails", new { id = model.SelectedPatientId });
        }


        // SearchPatient  ------------------------------------------------------------------------------------------------


        [HttpGet]
        public IActionResult SearchPatient(string query)
        {
            if (string.IsNullOrEmpty(query))
                return RedirectToAction("Index");

            int doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Sadece bu doktora ait hastalar içinde arama
            var patient = _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .FirstOrDefault(p =>
                    (p.FirstName + " " + p.LastName).ToLower().Contains(query.ToLower()));

            if (patient != null)
                return RedirectToAction("PatientDetails", new { id = patient.Id });

            TempData["SearchError"] = "Patient not found.";
            return RedirectToAction("Index");
        }


    }
}