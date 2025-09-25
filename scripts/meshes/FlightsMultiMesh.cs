using System;
using FlightGlobe.areas;
using FlightGlobe.Base;
using FlightGlobe.Data;
using FlightGlobe.Utilities;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class FlightsMultiMesh : MultiMeshInstance3D
    {
        public Texture2D AirplaneTexture { get; set; }
        public OrbitCamera OrbitCamera { get; set; }
        public Flight[] Flights { get; set; }

        [Signal]
        public delegate void FlightClickedEventHandler(int flightIndex);
       
        private FlightCollisionArea[] flightAreas;
        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 1f / 60f;

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = Flights.Length,
                Mesh = MeshUtil.CreateQuadMesh(AirplaneTexture, new (0.2f, 0.2f))
            };

            flightAreas = new FlightCollisionArea[Flights.Length];
            for (int i = 0; i < Flights.Length; i++)
            {
                var flightArea = new FlightCollisionArea
                {
                    FlightIndex = i,
                };

                flightArea.FlightClicked += (flightIndex, screenPosition) =>
                {
                    EmitSignal(SignalName.FlightClicked, flightIndex);
                };

                flightAreas[i] = flightArea;

                GetParent().AddChild(flightAreas[i]);
            }
        }

        public override void _Process(double delta)
        {
            updateTimer += (float)delta;
            if (updateTimer < UPDATE_INTERVAL) return;
            
            var deltaF = updateTimer;
            updateTimer = 0f;

            for (int i = 0; i < Flights.Length; i++)
            {
                var flight = Flights[i];
                var transform = flight.ProcessTransform(OrbitCamera);

                flight.ProcessDirection(deltaF);

                flightAreas[i].UpdateTransform(transform);
                Multimesh.SetInstanceTransform(i, transform);
            }
        }
    }
}