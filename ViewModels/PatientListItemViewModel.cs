namespace PatientTrackingSite.ViewModels
{
    public class PatientListItemViewModel
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AppointmentCount { get; set; }
    }
}
