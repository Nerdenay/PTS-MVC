using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PatientTrackingSite.Models

{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // "Patient", "Doctor", "Admin"

        [Phone]
        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Address { get; set; }

        public string? Specialization { get; set; } // Only for doctors

        public string? ProfileImagePath { get; set; } // Only For Doctors

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC No should be 11 character.")]
        public string TCNo { get; set; }

        // Navigation
        public ICollection<Appointment> AppointmentsAsPatient { get; set; }
        public ICollection<Appointment> AppointmentsAsDoctor { get; set; }

        public ICollection<Disease> Diseases { get; set; }
        public ICollection<Medication> MedicationsAsPatient { get; set; }
        public ICollection<Medication> MedicationsAsDoctor { get; set; }
        public ICollection<MedicalImage> MedicalImages { get; set; }


    }
}
