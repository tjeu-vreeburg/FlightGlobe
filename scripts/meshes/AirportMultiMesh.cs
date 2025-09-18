using FlightGlobe.Base;
using FlightGlobe.Data;
using FlightGlobe.Utilities;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class AirportMultiMesh : MultiMeshInstance3D
    {
        public OrbitCamera OrbitCamera { get; set; }
        public Airport[] Airports { get; set; }
        public float Radius { get; set; }

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = Airports.Length,
                Mesh = MeshUtil.CreateSphereMesh(0.01f, color: Colors.Aqua)
            };

            UpdateMesh(OrbitCamera.Zoom);
            OrbitCamera.OnZoomSignal += UpdateMesh;
        }

        private void UpdateMesh(float zoom = 0.0f)
        {
           
            var scale = 0.02f * (zoom / 1.0f);
            scale = Mathf.Clamp(scale, 0.1f, 1.0f);

            var basis = Basis.Identity.Scaled(new Vector3(scale, scale, scale));

            for (var i = 0; i < Airports.Length; i++)
            {
                var coordinate = Airports[i].Coordinate;
                var cartesianCoordinate = coordinate.ToCartesian(Radius);

                var transform = new Transform3D(zoom == 0.0f ? Basis.Identity : basis, cartesianCoordinate);
                Multimesh.SetInstanceTransform(i, transform);
            }
        }
    }
}


