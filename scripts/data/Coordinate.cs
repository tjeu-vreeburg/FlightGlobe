using Godot;

namespace FlightGlobe.Data
{
    public partial class Coordinate : Node
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Vector3 ToCartesian(float radius)
        {
            var latRad = Mathf.DegToRad(Latitude);
            var lonRad = Mathf.DegToRad(Longitude);

            var x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
            var y = radius * Mathf.Sin(latRad);                      
            var z = -radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

            return new Vector3(x, y, z);
        }
    }
}