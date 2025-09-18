using System;
using System.Linq;
using FlightGlobe.Base;
using FlightGlobe.Data;
using FlightGlobe.Loaders;
using FlightGlobe.Meshes;
using Godot;

namespace FlightGlobe
{
    public partial class FlightGlobeDemo : Node3D
    {
        [Export] private OrbitCamera orbitCamera;
        [Export] private Texture2D earthDayTexture;
        [Export] private Texture2D earthNightTexture;
        [Export] private Texture2D airplaneTexture;
        [Export] private Label fpsLabel;
        [Export] private float radius = 1.0f;
        [Export] private float radiusOffset = 0.001f;
        [Export] private float speedMultiplier = 100.0f;
        [Export] private int segments = 32;
        [Export] private int routeCount = 100;

        public override void _Ready()
        {
            var random = new Random();

            var airports = JsonLoader.GetValues<Airport>("res://data/airports.json");
            var airplanes = JsonLoader.GetValues<Airplane>("res://data/airplanes.json");

            var routes = RouteLoader.GetRoutes(airports, airplanes, routeCount);
            var flightDirections = routes.Select(r => r.Direction).ToArray();
            var flightDurations = routes.Select(r => r.GetDurationHours() * 3600 / speedMultiplier).ToArray();
            var flightPaths = routes.Select(r => r.GetCirclePath(radius, radiusOffset, segments)).ToArray();

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
            };

            var flightsMultiMesh = new FlightsMultiMesh
            {
                OrbitCamera = orbitCamera,
                AirplaneTexture = airplaneTexture,
                FlightDirections = flightDirections,
                FlightDurations = flightDurations,
                FlightPaths = flightPaths,
            };

            var routeLinesMesh = new RouteLinesMesh
            {
                FlightPaths = flightPaths
            };

            var airportMultiMesh = new AirportMultiMesh
            {
                OrbitCamera = orbitCamera,
                Airports = airports,
                Radius = radius
            };

            AddChild(earthMesh);
            AddChild(airportMultiMesh);
            AddChild(routeLinesMesh);
            AddChild(flightsMultiMesh);
        }
        public override void _Process(double delta)
        {
            fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }
    }
}