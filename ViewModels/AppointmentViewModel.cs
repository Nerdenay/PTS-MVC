using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class AppointmentViewModel
    {

        [Required]
        public string Specialization { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Notes { get; set; }
    }
}
