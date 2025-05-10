using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientTrackingSite.Models

{
    public class MedicalImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; } // wwwroot içindeki dosya yolu

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public User Patient { get; set; }


    }
}
