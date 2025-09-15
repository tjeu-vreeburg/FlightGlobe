using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.Meshes
{
    public partial class EarthMesh : MeshInstance3D
    {
        public Texture2D DayTexture { get; set; }
        public Texture2D NightTexture { get; set; }
        public Earth Earth { get; set; }

        private const float EARTH_AXIAL_TILT = 23.44f;
        private float radiansPerSecond;
        private ShaderMaterial shaderMaterial;
        private float currentSunLongitude = 0.0f;
        private float daysPerSecond;
        private float timeElapsed = 0.0f;
        private float dayOfYear;

        private float CalculateSunLatitude()
        {
            if (!Earth.UseRealisticSeasons)
                return 0.0f;

            float daysSinceSolstice = dayOfYear - 172.0f;
            float angle = daysSinceSolstice / 365.25f * 2.0f * Mathf.Pi;
            return EARTH_AXIAL_TILT * Mathf.Cos(angle);
        }

        public override void _EnterTree()
        {
            var shader = GD.Load<Shader>("res://shaders/globe.gdshader");
            shaderMaterial = new ShaderMaterial
            {
                Shader = shader
            };

            shaderMaterial.SetShaderParameter("day_texture", DayTexture);
            shaderMaterial.SetShaderParameter("night_texture", NightTexture);
            shaderMaterial.SetShaderParameter("transition_smoothness", Earth.TransitionSmoothness);
            shaderMaterial.SetShaderParameter("night_blend_factor", Earth.NightBlendFactor);
            shaderMaterial.SetShaderParameter("night_brightness", Earth.NightBrightness);
            shaderMaterial.SetShaderParameter("sun_longitude", currentSunLongitude);
            shaderMaterial.SetShaderParameter("sun_latitude", Mathf.DegToRad(CalculateSunLatitude()));
            shaderMaterial.SetShaderParameter("emission_strength", Earth.EmissionStrength);
            shaderMaterial.SetShaderParameter("emission_threshold", Earth.EmissionThreshold);
            shaderMaterial.SetShaderParameter("flicker_intensity", Earth.FlickerIntensity);
            shaderMaterial.SetShaderParameter("flicker_speed", Earth.FlickerSpeed);
            shaderMaterial.SetShaderParameter("time_offset", timeElapsed);

            MaterialOverride = shaderMaterial;

            Mesh = new SphereMesh()
            {
                Radius = Earth.Radius,
                Height = Earth.Radius * 2.0f,
                RadialSegments = 64,
                Rings = 32,
            };

            var degreesPerSecond = 360.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;
            radiansPerSecond = Mathf.DegToRad(degreesPerSecond);
            daysPerSecond = 1.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;

            RotateY(Mathf.DegToRad(-90.0f));
        }

        public override void _Process(double delta)
        {
            timeElapsed += (float)delta;

            currentSunLongitude += (float)delta * radiansPerSecond;
            if (currentSunLongitude > Mathf.Tau)
                currentSunLongitude -= Mathf.Tau;

            if (Earth.UseRealisticSeasons)
            {
                dayOfYear += (float)delta * daysPerSecond;
                if (dayOfYear > 365.25f)
                    dayOfYear -= 365.25f;
            }

            if (shaderMaterial != null)
            {
                shaderMaterial.SetShaderParameter("sun_longitude", currentSunLongitude);
                shaderMaterial.SetShaderParameter("sun_latitude", Mathf.DegToRad(CalculateSunLatitude()));
                shaderMaterial.SetShaderParameter("transition_smoothness", Earth.TransitionSmoothness);
                shaderMaterial.SetShaderParameter("night_blend_factor", Earth.NightBlendFactor);
                shaderMaterial.SetShaderParameter("night_brightness", Earth.NightBrightness);
                shaderMaterial.SetShaderParameter("emission_strength", Earth.EmissionStrength);
                shaderMaterial.SetShaderParameter("emission_threshold", Earth.EmissionThreshold);
                shaderMaterial.SetShaderParameter("flicker_intensity", Earth.FlickerIntensity);
                shaderMaterial.SetShaderParameter("flicker_speed", Earth.FlickerSpeed);
                shaderMaterial.SetShaderParameter("time_offset", timeElapsed);
            }
        }
    }
}