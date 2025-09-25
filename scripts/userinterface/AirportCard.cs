using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.UserInterface
{
    public partial class AirportCard : PanelContainer
    {
        [Export] private Label airportName;
        [Export] private Label airportCode;
        [Export] private Label airportLatitude;
        [Export] private Label airportLongitude;

        public void Update(Airport airport)
        {
            airportName.Text = $"Name: {airport.Name}";
            airportCode.Text = $"Code: {airport.IATA}";
            airportLatitude.Text = $"Latitude: {airport.Coordinate.Latitude:F4}";
            airportLongitude.Text = $"Longitude: {airport.Coordinate.Longitude:F4}";
        }
    }
}