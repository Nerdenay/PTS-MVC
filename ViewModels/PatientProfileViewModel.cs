
using PatientTrackingSite.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using PatientTrackingSite.Enums;

namespace PatientTrackingSite.ViewModels
{
    public class PatientProfileViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Address { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string TCNo { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        public string? ProfileImagePath { get; set; }

        public IFormFile? ProfileImageFile { get; set; }

        // Password update
        public string? CurrentPassword { get; set; }
        [MinLength(6)]
        public string? NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }


}



