using Godot;

namespace FlightGlobe.Meshes
{
    public partial class EarthMesh : MeshInstance3D
    {
        public Texture2D Texture { get; set; }
        public DirectionalLight3D DirectionalLight { get; set; }
        public float SpeedMultiplier { get; set; }
        public float Radius { get; set; }

        public override void _EnterTree()
        {
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoTexture = Texture,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel,
            };

            Mesh = new SphereMesh()
            {
                Radius = Radius,
                Height = Radius * 2.0f,
                RadialSegments = 32,
                Rings = 16
            };
        }

        public override void _Process(double delta)
        {
            if (DirectionalLight != null)
            {
                var degreesPerSecond = 360.0f / (24.0f * 3600.0f) * SpeedMultiplier;
                var radiansPerSecond = Mathf.DegToRad(degreesPerSecond);
                
                DirectionalLight.RotateY((float)delta * radiansPerSecond);
            }
        }
    }
}