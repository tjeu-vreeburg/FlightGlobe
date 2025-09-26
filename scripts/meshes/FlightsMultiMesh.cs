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
        public DateTime DateTime { get; set; }

        private FlightCollisionArea[] flightAreas;
        private DeltaTimer deltaTimer = new();

        [Signal]
        public delegate void FlightClickedEventHandler(int flightIndex);

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = Flights.Length,
                Mesh = MeshUtil.CreateQuadMesh(AirplaneTexture, new(0.2f, 0.2f))
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

                var transform = Transform3D.Identity.Scaled(new(0.001f, 0.001f, 0.001f));

                flightAreas[i] = flightArea;

                Flights[i].UpdateSchedule(DateTime.AddDays(-1));
                flightAreas[i].UpdateTransform(transform);
                Multimesh.SetInstanceTransform(i, transform);

                GetParent().AddChild(flightAreas[i]);
            }
        }

        public override void _Process(double delta)
        {
            if (deltaTimer.IsWaiting(delta)) return;

            for (int i = 0; i < Flights.Length; i++)
            {
                var flight = Flights[i];
                var progress = flight.GetProgress(DateTime);
                if (progress > 0.0f)
                {
                    var transform = flight.ProcessTransform(OrbitCamera, progress);

                    flightAreas[i].UpdateTransform(transform);
                    Multimesh.SetInstanceTransform(i, transform);
                }
            }
        }
    }
}