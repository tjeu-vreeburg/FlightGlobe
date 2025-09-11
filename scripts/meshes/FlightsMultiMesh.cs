using System;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class FlightsMultiMesh : MultiMeshInstance3D
    {
        public Texture2D AirplaneTexture { get; set; }
        public Vector3[][] FlightPaths { get; set; }
        public float[] FlightDurations { get; set; }
        public float SpeedMultiPlier { get; set; }
        private float[] flightProgress;
        private bool[] flightDirections;
        private readonly Random random = new();

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = FlightPaths.Length,
                Mesh = GetAirplaneMesh()
            };

            flightProgress = new float[FlightPaths.Length];
            flightDirections = new bool[FlightPaths.Length];

            for (int i = 0; i < FlightPaths.Length; i++)
            {
                flightProgress[i] = random.NextSingle();
                flightDirections[i] = random.Next(2) == 0;

                var path = FlightPaths[i];
                var t = flightProgress[i] * (path.Length - 1);
                var index = Mathf.Clamp((int)t, 0, path.Length - 2);
                var fraction = t - index;
                var position = path[index].Lerp(path[index + 1], fraction);

                Multimesh.SetInstanceTransform(i, Transform3D.Identity.Translated(position));
            }
        }

        public override void _Process(double delta)
        {
            var deltaF = (float)delta;

            for (int i = 0; i < FlightPaths.Length; i++)
            {
                ref var progress = ref flightProgress[i];
                ref var direction = ref flightDirections[i];

                ProcessDirection(i, deltaF, ref progress, ref direction);
                ProcessMovement(i, FlightPaths[i], ref progress, ref direction);
            }
        }

        private void ProcessDirection(int i, float deltaF, ref float progress, ref bool direction)
        {
            var flightDurationSeconds = FlightDurations[i] * 3600f / SpeedMultiPlier;
            var progressDelta = deltaF / flightDurationSeconds;
            if (direction)
            {
                progress += progressDelta;
                if (progress >= 1.0f)
                {
                    progress = 1.0f;
                    direction = false;
                }
            }
            else
            {
                progress -= progressDelta;
                if (progress <= 0.0f)
                {
                    progress = 0.0f;
                    direction = true;
                }
            }
        }

        private void ProcessMovement(int i, Vector3[] path, ref float progress, ref bool direction)
        {
            var t = progress * (path.Length - 1);
            var index = Mathf.Clamp((int)t, 0, path.Length - 2);

            var fraction = t - index;
            var position = path[index].Lerp(path[index + 1], fraction);

            var movementDirection = direction ? path[index + 1] - path[index] : path[index] - path[index + 1];
            movementDirection = movementDirection.Normalized();

            var surfaceNormal = position.Normalized();
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
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            };

            return new QuadMesh
            {
                Size = new Vector2(0.01f, 0.01f),
                Material = airplaneMaterial,
            };
        }
    }
}