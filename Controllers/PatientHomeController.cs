using Microsoft.AspNetCore.Mvc;
using PatientTrackingSite.Models;

namespace PatientTrackingSite.Controllers
{
    public class PatientHomeController : Controller
    {

        private readonly PTSDBContext _context;

        public PatientHomeController(PTSDBContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {

            return View();
        }
    }
}
