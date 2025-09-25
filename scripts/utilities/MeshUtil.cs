using Godot;

namespace FlightGlobe.Utilities
{
    public class MeshUtil
    {
        public static SphereMesh CreateSphereMesh(float radius, Material material = null, Texture2D texture = null, Color color = default)
        {
            material ??= new StandardMaterial3D()
            {
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor,
                AlphaScissorThreshold = 0.5f,
            };

            if (material is StandardMaterial3D material3D)
            {
                if (texture != null) material3D.AlbedoTexture = texture;
                if (color != default) material3D.AlbedoColor = color;
            }

            return new SphereMesh
            {
                Radius = radius,
                Height = radius * 2.0f,
                RadialSegments = 64,
                Rings = 32,
                Material = material
            };
        }

        public static QuadMesh CreateQuadMesh(Texture2D texture, Vector2 size)
        {
            var airplaneMaterial = new StandardMaterial3D()
            {
                AlbedoTexture = texture,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                Transparency = BaseMaterial3D.TransparencyEnum.AlphaScissor,
                AlphaScissorThreshold = 0.5f,
            };

            return new QuadMesh
            {
                Size = size,
                Material = airplaneMaterial,
            };
        }
    }
}