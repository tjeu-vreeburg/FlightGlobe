using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class AirportMultiMesh : MultiMeshInstance3D
    {
        public Airport[] Airports { get; set; }
        public float Radius { get; set; }

        public override void _Ready()
        {
            Multimesh = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = Airports.Length,
                Mesh = GetSphereMesh()
            };
            for (var i = 0; i < Airports.Length; i++)
            {
                var coordinate = Airports[i].Coordinate;
                var cartesianCoordinate = coordinate.ToCartesian(Radius);
                var transform = new Transform3D(Basis.Identity, cartesianCoordinate);
                Multimesh.SetInstanceTransform(i, transform);
            }
        }

         private SphereMesh GetSphereMesh()
        {
            var sphereMaterial = new StandardMaterial3D()
            {
                AlbedoColor = Colors.Aqua,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor,
                AlphaScissorThreshold = 0.5f,
            };

            return new SphereMesh
            {
                Material = sphereMaterial,
                Radius = 0.005f,
                Height = 0.005f * 2.0f,
                RadialSegments = 32,
                Rings = 16,
            };
        }
    }
}


