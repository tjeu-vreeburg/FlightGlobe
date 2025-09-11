using System;
using System.Linq;
using FlightGlobe.Data;
using FlightGlobe.Loaders;
using FlightGlobe.Meshes;
using Godot;

namespace FlightGlobe
{
    public partial class FlightGlobeDemo : Node3D
    {
        [Export] private Texture2D sphereTexture;
        [Export] private Texture2D airplaneTexture;
        [Export] private DirectionalLight3D directionalLight;
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
            var flightPaths = routes.Select(r => r.GetCirclePath(radius, radiusOffset, segments)).ToArray();
            var durations = routes.Select(r => r.GetDurationHours()).ToArray();

            var earthMesh = new EarthMesh
            {
                Radius = radius,
                Texture = sphereTexture,
                DirectionalLight = directionalLight,
                SpeedMultiplier = speedMultiplier
            };

            var flightsMultiMesh = new FlightsMultiMesh
            {
                AirplaneTexture = airplaneTexture,
                FlightDurations = durations,
                FlightPaths = flightPaths,
                SpeedMultiPlier = speedMultiplier
            };

            var routeLinesMesh = new RouteLinesMesh
            {
                FlightPaths = flightPaths
            };

            AddChild(earthMesh);
            AddChild(routeLinesMesh);
            AddChild(flightsMultiMesh);
        }
        public override void _Process(double delta)
        {
            fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }
    }
}