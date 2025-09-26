using System;

namespace FlightGlobe.Data
{
    public partial class Schedule
    {
        public DayOfWeek DepartureDay { get; set; }
        public TimeOnly DepartureTime { get; set; }

        public DateTime? CurrentDepartureTime { get; set; }
        public DateTime? CurrentArrivalTime { get; set; }

        public void UpdateCurrentSchedule(DateTime fromDateTime, float durationHours)
        {
            var targetTime = fromDateTime.Date.Add(DepartureTime.ToTimeSpan());
            if (targetTime <= fromDateTime) targetTime = targetTime.AddDays(1);
    
            var daysToAdd = ((int)DepartureDay - (int)targetTime.DayOfWeek + 7) % 7;
            var nextDepartureDateTime = targetTime.AddDays(daysToAdd);

            CurrentDepartureTime = nextDepartureDateTime;
            CurrentArrivalTime = CurrentDepartureTime?.AddHours(durationHours);
        }

        public override string ToString()
        {
            var departureStr = CurrentDepartureTime?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
            var arrivalStr = CurrentArrivalTime?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
            return $"Departure: {departureStr}, Arrival: {arrivalStr}";
        }
    }
}