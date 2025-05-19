using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class DiseaseCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hasta seçimi zorunludur.")]
        public int SelectedPatientId { get; set; }

        [Required(ErrorMessage = "Hastalık adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }


        public List<SelectListItem>? PatientList { get; set; }
    }
}
