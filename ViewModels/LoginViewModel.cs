using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class LoginViewModel
    {

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 haneli olmalıdır.")]
        public string TCNo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
