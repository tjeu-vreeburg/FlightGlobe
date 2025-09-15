using System;
using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class FlightsMultiMesh : MultiMeshInstance3D
    {
        public Texture2D AirplaneTexture { get; set; }
        public Vector3[][] FlightPaths { get; set; }
        public Direction[] FlightDirections { get; set; }
        public float[] FlightDurations { get; set; }
        private float[] flightProgress;
        private float[] progressSpeeds;
        private readonly Random random = new();

        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 1f / 10f;

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = FlightPaths.Length,
                Mesh = GetAirplaneMesh()
            };

            flightProgress = new float[FlightPaths.Length];

            for (int i = 0; i < FlightPaths.Length; i++)
            {
                flightProgress[i] = random.NextSingle();

                var path = FlightPaths[i];
                var t = flightProgress[i] * (path.Length - 1);
                var index = Mathf.Clamp((int)t, 0, path.Length - 2);
                var fraction = t - index;
                var position = path[index].Lerp(path[index + 1], fraction);
                var transform = Transform3D.Identity.Translated(position);

                Multimesh.SetInstanceTransform(i, transform);
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
            var transform = new Transform3D(basis, position);

            Multimesh.SetInstanceTransform(i, transform);
        }

        private QuadMesh GetAirplaneMesh()
        {
            var airplaneMaterial = new StandardMaterial3D()
            {
                AlbedoTexture = AirplaneTexture,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor,
                AlphaScissorThreshold = 0.5f,
            };

            return new QuadMesh
            {
                Size = new Vector2(0.01f, 0.01f),
                Material = airplaneMaterial,
            };
        }
    }
}