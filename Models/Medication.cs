
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientTrackingSite.Models
{
    public class Medication
    {

        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Dosage { get; set; } // Örn: "500mg"

        public string Instructions { get; set; } // Kullanım talimatları

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public User Patient { get; set; }

        public int? DoctorId { get; set; } // optional

        [ForeignKey("DoctorId")]
        public User Doctor { get; set; }

    }
}
