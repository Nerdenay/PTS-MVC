using PatientTrackingSite.Models;

namespace PatientTrackingSite.ViewModels
{
    public class PatientDetailViewModel
    {
        public User Patient { get; set; }
        public List<Medication> Medications { get; set; }
        public List<Disease> Diseases { get; set; }
        public List<MedicalImage> MedicalImages { get; set; }
        public int AppointmentCount { get; set; }
    }

}
