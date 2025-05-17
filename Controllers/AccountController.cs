using Microsoft.AspNetCore.Mvc;
using PatientTrackingSite.Models;
using PatientTrackingSite.ViewModels;

namespace PatientTrackingSite.Controllers
{
    public class AccountController : Controller
    {

        private readonly PTSDBContext _context;

        public AccountController(PTSDBContext context)
        {
            _context = context;
        }

        // REGISTER -----------------------------------------------------------------------------------------------------------------



        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {

            return View();

        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ViewModel’den gerçek kullanıcı modeline geçiyoruz:
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    TCNo = model.TCNo,
                    PasswordHash = model.Password, // ileride hashlenecek
                    Role = "Patient",
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    Address = model.Address
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FirstName);

                return RedirectToAction("Index", "PatientHome");
            }

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsEmailAvailable(string email)
        {
            bool exists = _context.Users.Any(u => u.Email == email);
            return Json(!exists); // Eğer false dönerse hata mesajı gösterilir
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsPhoneAvailable(string phone)
        {
            bool exists = _context.Users.Any(u => u.Phone == phone);
            return Json(!exists);
        }


        // LOGIN -----------------------------------------------------------------------------------------------------------------


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {   
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u =>
                     u.TCNo == model.TCNo && u.PasswordHash == model.Password);
    
                if (user == null)
                {
                    ModelState.AddModelError("", "Geçersiz TC Kimlik No veya şifre.");
                    return View(model);
                }

                // Session set
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FirstName);

                if (user.Role == "Patient")
                    return RedirectToAction("Index", "PatientHome");
                else if (user.Role == "Doctor")
                    return RedirectToAction("Index", "DoctorHome");
                else
                    return RedirectToAction("Index", "Home"); // admin veya bilinmeyen rol
            }

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsTCNoAvailable(string tcNo)
        {
            bool exists = _context.Users.Any(u => u.TCNo == tcNo);
            return Json(!exists); // varsa false dön → hata mesajı gösterilsin
        }
    }
}
