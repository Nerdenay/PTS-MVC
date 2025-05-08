using Microsoft.AspNetCore.Mvc;

namespace PatientTrackingSite.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            // Örnek veriler - ileride DB'den çekilecek
            ViewBag.TodayAppointments = 7;
            ViewBag.TotalPatients = 124;
            ViewBag.NewLabResults = 3;

            return View();
        }

        public IActionResult MyPatients()
        {
            // Geçici dummy veriler (ileride DB'den gelecek)
            var patients = new List<string>
            {
                "Ahmet Yılmaz",
                "Elif Kaya",
                "Mehmet Güneş"
            };

            ViewBag.Patients = patients;

            return View();
        }

        public IActionResult Appointments()
        {
            var appointments = new List<dynamic>
    {
        new { Patient = "Ahmet Yılmaz", Date = "08.05.2025", Time = "10:00", Status = "Onaylı" },
        new { Patient = "Elif Kaya", Date = "08.05.2025", Time = "11:30", Status = "Bekliyor" },
        new { Patient = "Mehmet Güneş", Date = "08.05.2025", Time = "14:00", Status = "İptal" }
    };

            ViewBag.Appointments = appointments;
            return View();
        }

    }
}
