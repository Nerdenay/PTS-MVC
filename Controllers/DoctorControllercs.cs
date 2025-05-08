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

    }
}
