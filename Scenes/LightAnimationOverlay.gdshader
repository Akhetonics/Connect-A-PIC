shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, repeat_disable, filter_nearest;
uniform texture Texture

void fragment() {
	
    vec4 ScreenColor = textureLod(SCREEN_TEXTURE, SCREEN_UV, 0.0);
    vec4 textureColor = COLOR;
	// detect if there are red or blue balls
	// change the color to match 
    
	float blue_diff = ScreenColor.b + textureColor.b - textureColor.r - ScreenColor.r;
	
	// accummulate red and blue
	
    if (blue_diff < -0.8) {
//        ScreenColor.b = 0.0;
        ScreenColor.r = clamp(- blue_diff, 0.0, 1.0);
		COLOR = ScreenColor;
    } else if( blue_diff > 0.8){
//        ScreenColor.r = 0.0;
        ScreenColor.b = clamp(blue_diff, 0.0, 1.0);
		COLOR = ScreenColor;
    }
	ScreenColor.a = textureColor.a;

    
}
