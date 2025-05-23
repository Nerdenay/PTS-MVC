﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PatientTrackingSite.Enums;


namespace PatientTrackingSite.ViewModels
{
    public class RegisterViewModel
    {

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "This e-mail is used.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Remote("IsPhoneAvailable", "Account", ErrorMessage = "This phone number is used.")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "TC ID number is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC ID number must be 11 digits long.")]
        [Remote("IsTCNoAvailable", "Account", ErrorMessage = "This ID number is already registered in the system.")]
        public string TCNo { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Address { get; set; }

        [Required]
        public GenderType Gender { get; set; }

    }
}
