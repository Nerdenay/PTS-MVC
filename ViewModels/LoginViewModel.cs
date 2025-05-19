using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class LoginViewModel
    {

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC ID should be 11 digits.")]
        public string TCNo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
