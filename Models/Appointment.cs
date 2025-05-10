using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PatientTrackingSite.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } // "Scheduled", "Completed", "Cancelled"

        public string Notes { get; set; }

        // Navigation
        [ForeignKey("PatientId")]
        public User Patient { get; set; }

        [ForeignKey("DoctorId")]
        public User Doctor { get; set; }

    }
}
