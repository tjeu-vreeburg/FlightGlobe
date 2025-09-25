using Godot;

namespace FlightGlobe.Data
{
    public partial class Airplane : Node
    {
        public string TexturePath { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int SpeedInKilometers { get; set; }
    }
}