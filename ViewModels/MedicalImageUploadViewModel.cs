using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class MedicalImageUploadViewModel
    {

        [Required(ErrorMessage = "Hasta seçimi zorunludur.")]
        public int SelectedPatientId { get; set; }

        [Required(ErrorMessage = "Bir dosya seçmelisiniz.")]
        public IFormFile File { get; set; }

        public string Description { get; set; }

        public List<SelectListItem> PatientList { get; set; } = new List<SelectListItem>();
    }
}
