using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace PatientTrackingSite.ViewModels
{
    public class DoctorProfileViewModel
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

        [Display(Name = "Specialization")]
        [StringLength(100)]
        public string? Specialization { get; set; }

        [Required, StringLength(11)]
        public string TCNo { get; set; }

        public IFormFile? ProfileImageFile { get; set; }
        public string ProfileImagePath { get; set; }

        public string CurrentPassword { get; set; }

        [MinLength(6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
