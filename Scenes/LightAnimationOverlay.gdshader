// Laser Farbe
shader_type canvas_item;
render_mode blend_add;

uniform vec4 laserColor;
uniform vec2 sizeInTiles;

uniform vec4 lightInFlow1; // x = intensity, y = phase, z = offsetx, w = offsety
uniform vec4 lightOutFlow1;
uniform vec4 lightInFlow2; 
uniform vec4 lightOutFlow2;
uniform vec4 lightInFlow3; 
uniform vec4 lightOutFlow3;
uniform vec4 lightInFlow4;
uniform vec4 lightOutFlow4;
uniform vec4 lightInFlow5;
uniform vec4 lightOutFlow5;
uniform vec4 lightInFlow6;
uniform vec4 lightOutFlow6;
uniform vec4 lightInFlow7;
uniform vec4 lightOutFlow7;
uniform vec4 lightInFlow8;
uniform vec4 lightOutFlow8;
// ... so viele wie man lustig ist. Am Besten 16 (8 in, 8 out) oder sowas, damit man nur einen einzigen Shader braucht für alle

uniform sampler2D animation1;
uniform sampler2D animation2;
uniform sampler2D animation3;
uniform sampler2D animation4;
uniform sampler2D animation5;
uniform sampler2D animation6;
uniform sampler2D animation7;
uniform sampler2D animation8;
// ... die hälfte von oben, also 8 (in/out ist ja nur umgedreht in der Zeit)

vec4 getColorOfAnimation(sampler2D animation, vec2 curUV, float animationTime, float animationspeed , vec4 lightFlowAndPos){
    float phaseShiftDisplacement = float(int(animationTime + round(lightFlowAndPos.y * animationspeed)) % 4) /4.0;
    vec2 animatedUvPosition = vec2( (curUV.x /4.0 + phaseShiftDisplacement ) , curUV.y);
    
    return texture(animation, animatedUvPosition );
}

float getIntensityOfLight(vec4 currentInflowColor, float lightInflowIntensity){
    
    return currentInflowColor.a *(currentInflowColor.r - currentInflowColor.b) * lightInflowIntensity;
}
void fragment(){
    float animationspeed = 2.0f;
    float animationTime = round(TIME * animationspeed);
    
    vec4 col_baseTexture = texture(TEXTURE, UV);
    vec4 col_anim1in  = getColorOfAnimation(animation1,UV,animationTime,animationspeed, lightInFlow1);
    vec4 col_anim1out = getColorOfAnimation(animation1,UV,animationTime,-animationspeed, lightOutFlow1);
    vec4 col_anim2in  = getColorOfAnimation(animation2,UV,animationTime,animationspeed, lightInFlow2);
    vec4 col_anim2out = getColorOfAnimation(animation2,UV,animationTime,-animationspeed, lightOutFlow2);
    vec4 col_anim3in  = getColorOfAnimation(animation3,UV,animationTime,animationspeed, lightInFlow3);
    vec4 col_anim3out = getColorOfAnimation(animation3,UV,animationTime,-animationspeed, lightOutFlow3);
    vec4 col_anim4in  = getColorOfAnimation(animation4,UV,animationTime,animationspeed, lightInFlow4);
    vec4 col_anim4out = getColorOfAnimation(animation4,UV,animationTime,-animationspeed, lightOutFlow4);
    vec4 col_anim5in  = getColorOfAnimation(animation5,UV,animationTime,animationspeed, lightInFlow5);
    vec4 col_anim5out = getColorOfAnimation(animation5,UV,animationTime,-animationspeed, lightOutFlow5);
    vec4 col_anim6in  = getColorOfAnimation(animation6,UV,animationTime,animationspeed, lightInFlow6);
    vec4 col_anim6out = getColorOfAnimation(animation6,UV,animationTime,-animationspeed, lightOutFlow6);
    vec4 col_anim7in  = getColorOfAnimation(animation7,UV,animationTime,animationspeed, lightInFlow7);
    vec4 col_anim7out = getColorOfAnimation(animation7,UV,animationTime,-animationspeed, lightOutFlow7);
    vec4 col_anim8in  = getColorOfAnimation(animation8,UV,animationTime,animationspeed, lightInFlow8);
    vec4 col_anim8out = getColorOfAnimation(animation8,UV,animationTime,-animationspeed, lightOutFlow8);
    
  // Der rot Kanal = höhen, z.B.
  // Der blau Kanal = tiefen
  float intensity = getIntensityOfLight( col_anim1in, lightInFlow1.x) + getIntensityOfLight( col_anim1out, lightOutFlow1.x) +
        getIntensityOfLight( col_anim2in, lightInFlow2.x) + getIntensityOfLight( col_anim2out, lightOutFlow2.x) + 
        getIntensityOfLight( col_anim3in, lightInFlow3.x) + getIntensityOfLight( col_anim3out, lightOutFlow3.x) + 
        getIntensityOfLight( col_anim4in, lightInFlow4.x) + getIntensityOfLight( col_anim4out, lightOutFlow4.x) + 
        getIntensityOfLight( col_anim5in, lightInFlow5.x) + getIntensityOfLight( col_anim5out, lightOutFlow5.x) + 
        getIntensityOfLight( col_anim6in, lightInFlow6.x) + getIntensityOfLight( col_anim6out, lightOutFlow6.x) + 
        getIntensityOfLight( col_anim7in, lightInFlow7.x) + getIntensityOfLight( col_anim7out, lightOutFlow7.x) + 
        getIntensityOfLight( col_anim8in, lightInFlow8.x) + getIntensityOfLight( col_anim8out, lightOutFlow8.x);


  COLOR = laserColor * abs(2.0* intensity);
}