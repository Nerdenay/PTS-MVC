using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class MedicalImageUploadViewModel
    {

        [Required(ErrorMessage = "Please select a patient.")]
        public int? SelectedPatientId { get; set; }

        [Required(ErrorMessage = "You must select a file.")]
        public IFormFile File { get; set; }

        
        public string Description { get; set; }

        public List<SelectListItem> PatientList { get; set; } = new List<SelectListItem>();
    }
}
