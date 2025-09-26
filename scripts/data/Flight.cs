using System;
using System.Linq;
using FlightGlobe.Base;
using Godot;

namespace FlightGlobe.Data
{
    public partial class Flight : Node
    {
        public Route Route { get; set; }
        public Airplane Airplane { get; set; }
        public Schedule[] Schedules { get; set; }
        public Schedule CurrentSchedule { get; set; }
        public Vector3[] Path { get; set; }

        public void UpdateSchedule(DateTime dateTime)
        {
            var distanceKm = Route.GetDistanceInKilometers();
            var baseFlightTime = distanceKm / Airplane.SpeedInKilometers;
            var extraTime = distanceKm < 1000 ? 0.5f : 0.75f;
            var durationHours = baseFlightTime + extraTime;

            foreach (var schedule in Schedules)
            {
                schedule.UpdateCurrentSchedule(dateTime, durationHours);
            }

            CurrentSchedule = Schedules
                .Where(x => x.CurrentDepartureTime.HasValue)
                .OrderBy(x => x.CurrentDepartureTime)
                .FirstOrDefault();
        }

        public Transform3D ProcessTransform(OrbitCamera orbitCamera, float progress)
        {
            var pathLength = Path.Length - 1;
            var t = Mathf.Clamp(progress, 0f, 1f) * pathLength;

            var index = Mathf.Clamp(Mathf.FloorToInt(t), 0, pathLength - 1);
            var nextIndex = Mathf.Min(index + 1, pathLength);

            var fraction = t - index;
            var position = Path[index].Lerp(Path[nextIndex], fraction);
            var surfaceNormal = position.Normalized();

            var movementDirection = Path[nextIndex] - Path[index];
            movementDirection = movementDirection.Normalized();

            var forward = movementDirection;
            var right = forward.Cross(surfaceNormal).Normalized();

            var scale = 0.01f * (orbitCamera.Zoom / 1.0f);
            scale = Mathf.Clamp(scale, 0.05f, 0.5f);

            var basis = new Basis(right, forward, surfaceNormal);
            basis = basis.Scaled(new Vector3(scale, scale, scale));

            return new Transform3D(basis, position);
        }

        public bool IsIdle(DateTime dateTime)
        {
            var departureTime = CurrentSchedule.CurrentDepartureTime.Value;
            return dateTime < departureTime;
        }

        public float GetProgress(DateTime dateTime)
        {
            if (CurrentSchedule?.CurrentDepartureTime == null || CurrentSchedule?.CurrentArrivalTime == null)
                return 0.0f;

            var departureTime = CurrentSchedule.CurrentDepartureTime.Value;
            var arrivalTime = CurrentSchedule.CurrentArrivalTime.Value;

            if (dateTime < departureTime) return 0.0f;

            if (dateTime >= arrivalTime)
            {
                UpdateSchedule(dateTime);
                if (CurrentSchedule?.CurrentDepartureTime == null || CurrentSchedule?.CurrentArrivalTime == null) return 0.0f;

                var newDepartureTime = CurrentSchedule.CurrentDepartureTime.Value;
                var newArrivalTime = CurrentSchedule.CurrentArrivalTime.Value;

                if (dateTime < newDepartureTime) return 0.0f;

                if (dateTime < newArrivalTime)
                {
                    var newTotalDuration = newArrivalTime - newDepartureTime;
                    var newElapsed = dateTime - newDepartureTime;
                    return (float)(newElapsed.TotalSeconds / newTotalDuration.TotalSeconds);
                }

                return 0.0f;
            }

            var totalFlightDuration = arrivalTime - departureTime;
            var elapsed = dateTime - departureTime;

            return (float)(elapsed.TotalSeconds / totalFlightDuration.TotalSeconds);
        }
    }
}