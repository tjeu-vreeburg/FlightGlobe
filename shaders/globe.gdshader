shader_type spatial;
render_mode unshaded;

uniform sampler2D day_texture : source_color;
uniform sampler2D night_texture : source_color;
uniform float sun_longitude : hint_range(0.0, 6.283185) = 0.0; // 0 to 2π radians
uniform float sun_latitude : hint_range(-1.570796, 1.570796) = 0.0; // -π/2 to π/2 radians (seasonal tilt)
uniform float transition_smoothness : hint_range(0.0, 1.0) = 0.1;
uniform float night_blend_factor : hint_range(0.0, 1.0) = 0.3;
uniform float night_brightness : hint_range(0.1, 2.0) = 0.8;
uniform float emission_strength : hint_range(0.0, 3.0) = 1.5; // How much the lights glow
uniform float emission_threshold : hint_range(0.0, 1.0) = 0.3; // Brightness threshold for emission
uniform float flicker_intensity : hint_range(0.0, 1.0) = 0.3; // How much lights flicker
uniform float flicker_speed : hint_range(0.1, 10.0) = 2.0; // Speed of flickering
uniform float time_offset : hint_range(0.0, 100.0) = 0.0; // Time for animation

// Simple noise function for flickering
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

// Smoother noise with time
float flickering_noise(vec2 uv, float time) {
    vec2 i = floor(uv * 100.0); // Scale up for more variation
    vec2 f = fract(uv * 100.0);
    
    // Sample noise at different time intervals
    float a = noise(i + vec2(0.0, 0.0) + time);
    float b = noise(i + vec2(1.0, 0.0) + time);
    float c = noise(i + vec2(0.0, 1.0) + time);
    float d = noise(i + vec2(1.0, 1.0) + time);
    
    // Smooth interpolation
    vec2 u = f * f * (3.0 - 2.0 * f);
    
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

void fragment() {
    // Sample both textures
    vec4 day_color = texture(day_texture, UV);
    vec4 night_color = texture(night_texture, UV);
    
    // Convert UV to spherical coordinates
    float longitude = UV.x * 6.283185; // 0 to 2π
    float latitude = (UV.y - 0.5) * 3.141593; // -π/2 to π/2
    
    // Calculate the sun direction vector
    vec3 sun_dir = vec3(
        cos(sun_latitude) * cos(sun_longitude),
        sin(sun_latitude),
        cos(sun_latitude) * sin(sun_longitude)
    );
    
    // Calculate the surface normal at this UV point
    vec3 surface_normal = vec3(
        cos(latitude) * cos(longitude),
        sin(latitude),
        cos(latitude) * sin(longitude)
    );
    
    // Calculate how much this point faces the sun
    float sun_factor = dot(surface_normal, sun_dir);
    
    // Create smooth day/night transition
    float day_factor = smoothstep(-transition_smoothness, transition_smoothness, sun_factor);
    
    // Create a blended night texture
    vec4 blended_night = mix(night_color, day_color * night_brightness, night_blend_factor);
    
    // Mix between day and blended night
    vec4 final_color = mix(blended_night, day_color, day_factor);
    
    // Calculate emission for night areas
    float night_factor = 1.0 - day_factor;
    
    // Use night texture brightness to determine emission intensity
    float night_luminance = dot(night_color.rgb, vec3(0.299, 0.587, 0.114)); // RGB to luminance
    float emission_mask = smoothstep(emission_threshold, emission_threshold + 0.2, night_luminance);
    
    // Create flickering effect
    float flicker_time = time_offset * flicker_speed;
    float flicker1 = flickering_noise(UV, flicker_time * 0.7);
    float flicker2 = flickering_noise(UV * 1.3, flicker_time * 1.2);
    float flicker3 = flickering_noise(UV * 2.1, flicker_time * 0.5);
    
    // Combine multiple noise layers for more realistic flickering
    float combined_flicker = (flicker1 + flicker2 * 0.5 + flicker3 * 0.3) / 1.8;
    
    // Apply flickering only to bright areas (cities) and only at night
    float flicker_multiplier = 1.0 - (flicker_intensity * (1.0 - combined_flicker) * emission_mask * night_factor);
    
    // Create emission effect with flickering
    vec3 emission = night_color.rgb * emission_strength * emission_mask * night_factor * flicker_multiplier;
    
    ALBEDO = final_color.rgb + emission;
}