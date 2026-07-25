shader_type canvas_item;

const int color_count = 8;
const float half_sqrt2 = 0.70710678;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D original_palette : filter_nearest;
uniform sampler2D target_palette : filter_nearest;

uniform float vignette_amount : hint_range(0, 1);

void fragment() {
  vec4 screen_color = texture(screen_texture, SCREEN_UV);
  
  vec2 offset_from_pixel = vec2(mod(SCREEN_UV.x, SCREEN_PIXEL_SIZE.x * 4.0), mod(SCREEN_UV.y, SCREEN_PIXEL_SIZE.y * 4.0));
  vec2 pixelized_screen_uv = SCREEN_UV - offset_from_pixel;
  float distance_from_center = distance(vec2(0.5, 0.5), pixelized_screen_uv);
  
  if (distance_from_center > vignette_amount * half_sqrt2 || vignette_amount < 0.01) {
    float step = 1.0 / float(color_count);
    vec4 target_color = screen_color;
    
    for (int i = 0; i < color_count; i++) {
      float x = (float(i) + 0.5) * step;
      vec4 original_color = texture(original_palette, vec2(x, 0.5));
      if (distance(screen_color, original_color) < 0.01) {
        target_color = texture(target_palette, vec2(x, 0.5));
      }
    }
    
    COLOR = target_color;
  } else {
    COLOR = screen_color;
  }
}
