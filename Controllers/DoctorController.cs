using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using System;

namespace PatientTrackingSite.Controllers
{
    public class DoctorController : Controller

    {
        private readonly PTSDBContext _context;

        public DoctorController(PTSDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doctors = _context.Users
           .Where(u => u.Role == "Doctor")
           .ToList();

            return View(doctors); ;
        }
    }
}
