using FlightGlobe.Data;
using FlightGlobe.Utilities;
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
            if (!Earth.UseRealisticSeasons) return 0.0f;

            var daysSinceSolstice = dayOfYear - 172.0f;
            var angle = daysSinceSolstice / 365.25f * 2.0f * Mathf.Pi;
            
            return EARTH_AXIAL_TILT * Mathf.Cos(angle);
        }

        public override void _EnterTree()
        {
            shaderMaterial = new ShaderMaterial
            {
                Shader = GD.Load<Shader>("res://shaders/globe.gdshader"),
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

            Mesh = MeshUtil.CreateSphereMesh(Earth.Radius, material: shaderMaterial);

            var degreesPerSecond = 360.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;
            radiansPerSecond = Mathf.DegToRad(degreesPerSecond);
            daysPerSecond = 1.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;

            RotateY(Mathf.DegToRad(-90.0f));
        }

        public override void _Process(double delta)
        {
            var deltaF = (float)delta;
            timeElapsed += deltaF;

            currentSunLongitude += deltaF * radiansPerSecond;
            if (currentSunLongitude > Mathf.Tau)
                currentSunLongitude -= Mathf.Tau;

            if (Earth.UseRealisticSeasons)
            {
                dayOfYear += deltaF * daysPerSecond;
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