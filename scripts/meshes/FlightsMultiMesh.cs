using System;
using FlightGlobe.Base;
using FlightGlobe.Data;
using FlightGlobe.Utilities;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class FlightsMultiMesh : MultiMeshInstance3D
    {
        public OrbitCamera OrbitCamera { get; set; }
        public Texture2D AirplaneTexture { get; set; }
        public Vector3[][] FlightPaths { get; set; }
        public Direction[] FlightDirections { get; set; }
        public float[] FlightDurations { get; set; }
        private float[] flightProgress;
        private float[] progressSpeeds;
        private readonly Random random = new();

        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 1f / 60f;

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = FlightPaths.Length,
                Mesh = MeshUtil.CreateQuadMesh(AirplaneTexture, new (0.2f, 0.2f))
            };

            flightProgress = new float[FlightPaths.Length];
            for (int i = 0; i < FlightPaths.Length; i++)
            {
                flightProgress[i] = random.NextSingle();
            }
        }

        public override void _Process(double delta)
        {
            updateTimer += (float)delta;
            if (updateTimer < UPDATE_INTERVAL) return;
            
            var deltaF = updateTimer;
            updateTimer = 0f;

            for (int i = 0; i < FlightPaths.Length; i++)
            {
                ref var progress = ref flightProgress[i];

                ProcessDirection(i, deltaF, ref progress);
                ProcessMovement(i, FlightPaths[i], ref progress);
            }
        }

        private void ProcessDirection(int i, float deltaF, ref float progress)
        {
            var progressDelta = deltaF / FlightDurations[i];
            if (FlightDirections[i] == Direction.FROM)
            {
                progress += progressDelta;
                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                    FlightDirections[i] = Direction.TO;
                }
            }
            else
            {
                progress -= progressDelta;
                if (progress <= 0.0f)
                {
                    progress = 0.0f;
                    FlightDirections[i] = Direction.FROM;
                }
            }
        }

        private void ProcessMovement(int i, Vector3[] path, ref float progress)
        {
            var pathLength = path.Length - 1;
            var t = progress * pathLength;
            var index = Mathf.Clamp(Mathf.FloorToInt(t), 0, pathLength - 1);
            var nextIndex = Mathf.Min(index + 1, pathLength);

            var fraction = t - index;
            var position = path[index] + (path[nextIndex] - path[index]) * fraction;
            var surfaceNormal = position.Normalized();

            var movementDirection = FlightDirections[i] == Direction.FROM ?
                path[nextIndex] - path[index] :
                path[index] - path[nextIndex];

            movementDirection = movementDirection.Normalized();

            var forward = movementDirection;
            var right = forward.Cross(surfaceNormal).Normalized();

            var basis = new Basis(right, forward, surfaceNormal);

            var scale = 0.01f * (OrbitCamera.Zoom / 1.0f);
            scale = Mathf.Clamp(scale, 0.05f, 0.5f);

            basis = basis.Scaled(new Vector3(scale, scale, scale));

            var transform = new Transform3D(basis, position);

            Multimesh.SetInstanceTransform(i, transform);
        }
    }
}