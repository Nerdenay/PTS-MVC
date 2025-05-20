using System;
using System.Collections.Generic;

namespace PatientTrackingSite.Helpers
{
    public class AppointmentHelper
    {
        public static List<TimeSpan> GetAvailableTimeSlots()
        {
            var startTime = new TimeSpan(8, 0, 0);   // 08:00
            var endTime = new TimeSpan(17, 0, 0);    // 17:00
            var lunchStart = new TimeSpan(12, 0, 0);
            var lunchEnd = new TimeSpan(13, 0, 0);

            List<TimeSpan> timeSlots = new();

            for (var time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(30)))
            {
                if (time < lunchStart || time >= lunchEnd)
                {
                    timeSlots.Add(time);
                }
            }

            return timeSlots;
        }
    


     }
}
