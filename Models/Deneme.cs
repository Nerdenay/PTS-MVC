using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.Models
{
    public class Deneme

    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        [StringLength(100)] 
        public string? Name { get; set; }

        public decimal Price { get; set; }

    }
}
