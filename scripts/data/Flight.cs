using FlightGlobe.Base;
using Godot;

namespace FlightGlobe.Data
{
    public partial class Flight : Node
    {
        public Route Route { get; set; }
        public Airplane Airplane { get; set; }
        public Vector3[] Path { get; set; }
        public Direction Direction { get; set; }
        public float DurationInSeconds { get; set; }
        public float Progress { get; set; }

        public void ProcessDirection(float delta)
        {
            var progressDelta = delta / DurationInSeconds;

            if (Direction == Direction.FROM)
            {
                Progress += progressDelta;
                if (Progress >= 1.0f)
                {
                    Progress = 1.0f;
                    Direction = Direction.TO;
                }
            }
            else
            {
                Progress -= progressDelta;
                if (Progress <= 0.0f)
                {
                    Progress = 0.0f;
                    Direction = Direction.FROM;
                }
            }
        }

        public Transform3D ProcessTransform(OrbitCamera orbitCamera)
        {
            var pathLength = Path.Length - 1;
            var t = Progress * pathLength;

            var index = Mathf.Clamp(Mathf.FloorToInt(t), 0, pathLength - 1);
            var nextIndex = Mathf.Min(index + 1, pathLength);

            var fraction = t - index;
            var position = Path[index] + (Path[nextIndex] - Path[index]) * fraction;
            var surfaceNormal = position.Normalized();

            var movementDirection = Direction == Direction.FROM ? Path[nextIndex] - Path[index] : Path[index] - Path[nextIndex];
            movementDirection = movementDirection.Normalized();

            var forward = movementDirection;
            var right = forward.Cross(surfaceNormal).Normalized();

            var scale = 0.01f * (orbitCamera.Zoom / 1.0f);
            scale = Mathf.Clamp(scale, 0.05f, 0.5f);

            var basis = new Basis(right, forward, surfaceNormal);
            basis = basis.Scaled(new Vector3(scale, scale, scale));

            return new Transform3D(basis, position);
        }

        public float GetDurationHours()
        {
            var distanceKm = Route.GetDistanceInKilometers();

            var baseFlightTime = distanceKm / Airplane.SpeedInKilometers;
            var extraTime = distanceKm < 1000 ? 0.5f : 0.75f;

            return baseFlightTime + extraTime;
        }
    }
}