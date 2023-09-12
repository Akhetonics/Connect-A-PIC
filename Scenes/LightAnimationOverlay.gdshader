shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;


void fragment() {
	
    vec4 ScreenColor = texture(SCREEN_TEXTURE, FRAGCOORD.xy / SCREEN_PIXEL_SIZE);
    vec4 textureColor = COLOR;

    ScreenColor.r = clamp((ScreenColor.r + textureColor.r) * 0.67 ,0.0, 1.0);
	ScreenColor.b = clamp((ScreenColor.b + textureColor.b)*0.6,0.0, 1.0);
	float blue_diff = ScreenColor.b - textureColor.r;
    float red_diff = ScreenColor.r - textureColor.b;
	
	// accummulate red and blue
	
//    if (blue_diff > 0.0) {
//        ScreenColor.b = 0.0;
//        ScreenColor.r = clamp(ScreenColor.r - blue_diff, 0.0, 1.0);
//    } else {
//        ScreenColor.r = 0.0;
//        ScreenColor.b = clamp(ScreenColor.b - red_diff, 0.0, 1.0);
//    }
//	ScreenColor.a = textureColor.a;

    COLOR = ScreenColor;
}

