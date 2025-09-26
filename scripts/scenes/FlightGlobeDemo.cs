using System;
using FlightGlobe.Base;
using FlightGlobe.Data;
using FlightGlobe.Loaders;
using FlightGlobe.Meshes;
using FlightGlobe.UserInterface;
using Godot;

namespace FlightGlobe
{
    public partial class FlightGlobeDemo : Node3D
    {
        [Export] private OrbitCamera orbitCamera;
        [Export] private Texture2D earthDayTexture;
        [Export] private Texture2D earthNightTexture;
        [Export] private Texture2D airplaneTexture;
        [Export] private AirportCard airportCard;
        [Export] private FlightCard flightCard;
        [Export] private Label fpsLabel;
        [Export] private float radius = 1.0f;
        [Export] private float radiusOffset = 0.001f;
        [Export] private float speedMultiplier = 100.0f;
        [Export] private int segments = 32;
        [Export] private int routeCount = 100;

        public override void _Ready()
        {
            var airports = JsonLoader.GetValues<Airport>("res://data/airports.json");
            var airplanes = JsonLoader.GetValues<Airplane>("res://data/airplanes.json");

            var routes = RouteLoader.GetRoutes(airports, airplanes, routeCount);
            var flights = RouteLoader.CreateFlightsFromRoutes(routes, speedMultiplier, radius, radiusOffset, segments);

            var dateTime = DateTime.Now;

            var earth = new Earth
            {
                TransitionSmoothness = 0.1f,
                NightBlendFactor = 0.3f,
                NightBrightness = 0.8f,
                UseRealisticSeasons = true,
                EmissionStrength = 1.1f,
                EmissionThreshold = 0.5f,
                FlickerIntensity = 0.7f,
                FlickerSpeed = 1.0f,
                Radius = radius,
                SpeedMultiplier = speedMultiplier,
            };

            var earthMesh = new EarthMesh
            {
                DayTexture = earthDayTexture,
                NightTexture = earthNightTexture,
                Earth = earth,
                DateTime = dateTime, 
            };

            var flightsMultiMesh = new FlightsMultiMesh
            {
                OrbitCamera = orbitCamera,
                AirplaneTexture = airplaneTexture,
                Flights = flights,
                DateTime = dateTime, 
            };

            var flightLineMesh = new FlightLineMesh
            {
                Flights = flights,
                DateTime = dateTime, 
            };

            var airportMultiMesh = new AirportMultiMesh
            {
                OrbitCamera = orbitCamera,
                Airports = airports,
                Radius = radius
            };

            airportMultiMesh.AirportClicked += (airportIndex) =>
            {
                airportCard.Update(airports[airportIndex]);
                airportCard.Show();
                flightCard.Hide();
            };

            flightsMultiMesh.FlightClicked += (flightIndex) =>
            {
                flightCard.Update(flights[flightIndex]);
                flightCard.Show();
                airportCard.Hide();
            };

            AddChild(earthMesh);
            AddChild(airportMultiMesh);
            AddChild(flightLineMesh);
            AddChild(flightsMultiMesh);
        }
        public override void _Process(double delta)
        {
            fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }
    }
}