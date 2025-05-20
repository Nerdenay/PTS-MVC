using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class DiseaseCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient selection is required.")]
        public int SelectedPatientId { get; set; }

        [Required(ErrorMessage = "Disease name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }


        public List<SelectListItem>? PatientList { get; set; }
    }
}
