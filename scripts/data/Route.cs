using Godot;

namespace FlightGlobe.Data
{
    public partial class Route : Node
    {
        public Airport Origin { get; set; }
        public Airport Destination { get; set; }
        public Airplane Airplane { get; set; }
        public Direction Direction { get; set; }

        public Vector3[] GetCirclePath(float radius, float radiusOffset, int segments)
        {
            var originCartesianCoordinate = Origin.Coordinate.ToCartesian(radius);
            var destinationCartesianCoordiate = Destination.Coordinate.ToCartesian(radius);

            var points = new Vector3[segments + 1];
            for (var i = 0; i <= segments; i++)
            {
                var t = (float)i / segments;
                var interpolation = originCartesianCoordinate.Slerp(destinationCartesianCoordiate, t);
                points[i] = interpolation.Normalized() * (radius + radiusOffset);
            }

            return points;
        }

        public float GetDistanceInKilometers()
        {
            const float earthRadiusInKilometers = 6371.0f;

            var lat1Rad = Mathf.DegToRad(Origin.Coordinate.Latitude);
            var lon1Rad = Mathf.DegToRad(Origin.Coordinate.Longitude);
            var lat2Rad = Mathf.DegToRad(Destination.Coordinate.Latitude);
            var lon2Rad = Mathf.DegToRad(Destination.Coordinate.Longitude);

            var deltaLat = lat2Rad - lat1Rad;
            var deltaLon = lon2Rad - lon1Rad;

            var a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2) +
                    Mathf.Cos(lat1Rad) * Mathf.Cos(lat2Rad) *
                    Mathf.Sin(deltaLon / 2) * Mathf.Sin(deltaLon / 2);

            var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

            return earthRadiusInKilometers * c;
        }

        public float GetDurationHours()
        {
            var distanceKm = GetDistanceInKilometers();

            var baseFlightTime = distanceKm / Airplane.SpeedInKilometers;
            var extraTime = distanceKm < 1000 ? 0.5f : 0.75f;

            return baseFlightTime + extraTime;
        }
    }
}