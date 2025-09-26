using FlightGlobe.Data;
using FlightGlobe.Utilities;
using Godot;
using System;

namespace FlightGlobe.Meshes
{
    public partial class EarthMesh : MeshInstance3D
    {
        public Texture2D DayTexture { get; set; }
        public Texture2D NightTexture { get; set; }
        public Earth Earth { get; set; }
        public DateTime DateTime { get; set; }

        private DateTime startTime;
        private ShaderMaterial shaderMaterial;

        private const float EARTH_AXIAL_TILT = 23.44f;
        private float radiansPerSecond;
        private float currentSunLongitude = 0.0f;
        private float daysPerSecond;
        private float dayOfYear;

        private static float CalculateDayOfYear(DateTime dateTime)
        {
            var startOfYear = new DateTime(dateTime.Year, 1, 1);
            var dayOfYear = (dateTime - startOfYear).TotalDays + 1;
            return (float)dayOfYear;
        }

        private static float CalculateSunLongitude(DateTime dateTime)
        {
            var hoursFromMidnight = dateTime.Hour + (dateTime.Minute / 60.0f) + (dateTime.Second / 3600.0f);
            
            var longitude = (hoursFromMidnight * 15.0f) + 180.0f;
            if (longitude >= 360.0f) longitude -= 360.0f;
            
            return Mathf.DegToRad(longitude);
        }

        private float CalculateSunLatitude()
        {
            if (!Earth.UseRealisticSeasons) return 0.0f;

            var daysSinceVernalEquinox = dayOfYear - 79.0f;
            if (daysSinceVernalEquinox < 0) daysSinceVernalEquinox += 365.25f;
            
            var angle = daysSinceVernalEquinox / 365.25f * 2.0f * Mathf.Pi;
            
            return EARTH_AXIAL_TILT * Mathf.Sin(angle);
        }

        public override void _EnterTree()
        {
            dayOfYear = CalculateDayOfYear(DateTime);
            currentSunLongitude = CalculateSunLongitude(DateTime);

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
            shaderMaterial.SetShaderParameter("time_offset", 0.0f); // Reset since we're using real time

            MaterialOverride = shaderMaterial;

            Mesh = MeshUtil.CreateSphereMesh(Earth.Radius, material: shaderMaterial);

            var degreesPerSecond = 360.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;
            radiansPerSecond = Mathf.DegToRad(degreesPerSecond);
            daysPerSecond = 1.0f / (24.0f * 3600.0f) * Earth.SpeedMultiplier;

            RotateY(Mathf.DegToRad(-90.0f));
        }

        public override void _Process(double delta)
        {
            dayOfYear = CalculateDayOfYear(DateTime);
            currentSunLongitude = CalculateSunLongitude(DateTime);

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
                
                var realElapsedTime = (float)(DateTime.Now - startTime).TotalSeconds;
                shaderMaterial.SetShaderParameter("time_offset", realElapsedTime);
            }
        }
    }
}