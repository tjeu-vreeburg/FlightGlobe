using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.UserInterface
{
    public partial class FlightCard : PanelContainer
    {
        [Export] private Label flightOrigin;
        [Export] private Label flightDestination;
        [Export] private Label flightAircraft;

        public void Update(Flight flight)
        {
            flightOrigin.Text = $"Origin: {flight.Route.Origin.IATA}";
            flightDestination.Text = $"Destination: {flight.Route.Destination.IATA}";
            flightAircraft.Text = $"Aircraft: {flight.Airplane.Manufacturer} {flight.Airplane.Model}";
        }
    }
}