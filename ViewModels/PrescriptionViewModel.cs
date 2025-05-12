using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using PatientTrackingSite.Models;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class PrescriptionViewModel
    {
        [Required]
        public int SelectedPatientId { get; set; }

        [BindNever]
        public List<SelectListItem> PatientList { get; set; } = new();

        [Required]
        public string MedicationName { get; set; }

        public string Dosage { get; set; }

        public string Instructions { get; set; }
    }
}
