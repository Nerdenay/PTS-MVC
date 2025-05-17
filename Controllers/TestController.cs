using Microsoft.AspNetCore.Mvc;

namespace PatientTrackingSite.Controllers
{
    public class TestController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public TestController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult UploadTest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadTest(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                ViewBag.Message = "Yükleme başarılı: " + fileName;
                ViewBag.ImagePath = "/uploads/" + fileName;
            }
            else
            {
                ViewBag.Message = "Dosya seçilmedi veya boş.";
            }

            return View();
        }
    }
}
