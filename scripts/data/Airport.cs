using Godot;

namespace FlightGlobe.Data
{
    public partial class Airport : Node
    {
        public new string Name { get; set; }
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public Coordinate Coordinate { get; set; }
    }
}