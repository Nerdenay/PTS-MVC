using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class DiseaseCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        public int? SelectedPatientId { get; set; }

        [Required(ErrorMessage = "Please enter a disease name.")]
        public string Name { get; set; }

        
        public string Description { get; set; }

        public List<SelectListItem> PatientList { get; set; }
    }


}
