using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.UserInterface
{
    public partial class FlightCard : PanelContainer
    {
        [Export] private Label flightOrigin;
        [Export] private Label flightDestination;
        [Export] private Label flightAircraft;
        [Export] private Label flightDeparture;
        [Export] private Label flightArrival;

        public void Update(Flight flight)
        {
            var currentSchedule = flight.CurrentSchedule;
            var arrival = currentSchedule?.CurrentArrivalTime;
            var departure = currentSchedule?.CurrentDepartureTime;

            flightOrigin.Text = $"Origin: {flight.Route.Origin.IATA}";
            flightDestination.Text = $"Destination: {flight.Route.Destination.IATA}";
            flightAircraft.Text = $"Aircraft: {flight.Airplane.Manufacturer} {flight.Airplane.Model}";
            flightArrival.Text = $"Arrival: {arrival?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}";
            flightDeparture.Text = $"Departure: {departure?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}";
        }
    }
}