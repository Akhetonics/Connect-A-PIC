GDPC                P                                                                         X   res://.godot/exported/133200997/export-2aeef5e0df21011397c42ede8bc5c399-DelayTunable.scn�      <      �`PK���6����Y�    `   res://.godot/exported/133200997/export-e3428c87e0b653f4a1d466be67e97529-LightOverlayShaded.res  �*      �      ��t#`G�\DG��|X    ,   res://.godot/global_script_class_cache.cfg  PX             ��Р�8���8~$}P�    H   res://.godot/imported/Delay.png-ddc30d99d825a46bfe7dca3143e359e3.ctex   P      �      �7� �	t���8y)��O    H   res://.godot/imported/Overlay.png-a128810a0d69c6124e0eaea786068b57.ctex �=      L      �Ӷ�r����9��P"    D   res://.godot/imported/icon.svg-218a8f2b3041327d8a5756f3a245f83b.ctex�I      �      �̛�*$q�*�́        res://.godot/uid_cache.bin  0\      g      �p����뢥|K�I    $   res://Properties/launchSettings.json        E      �B'�����Q���S    8   res://Scenes/Components/DelayTunable/Delay.png.import          �       F�@�y�?,���_    <   res://Scenes/Components/DelayTunable/DelayTunable.tscn.remappW      i       �ߴ��)Gb�UL&Z���    <   res://Scenes/Components/DelayTunable/DelayTunableDraft.json #      �      � �����3��J,��    D   res://Scenes/Components/DelayTunable/LightOverlayShaded.tres.remap  �W      o       V.G_��B>V�9��WQj    8   res://Scenes/Components/DelayTunable/Overlay.png.import �H      �       �b;���W�.Aڴ�K��       res://icon.svg  pX      �      C��=U���^Qu��U3       res://icon.svg.import   �V      �       _KJ�FҖc��ʩ��<       res://project.binary�]      k      �̤W��|� ��~�                {
  "profiles": {
    "DirectionalCouplerTunable PCK": {
      "commandName": "Project"
    },
    "Profil \"1\"": {
      "commandName": "Executable",
      "executablePath": "C:\\Program Files\\Godot_v4.1\\Godot_v4.1.exe",
      "commandLineArgs": "--path . --verbose",
      "workingDirectory": "."
    }
  }
}           GST2   =   =      ����               = =        r  RIFFj  WEBPVP8L]  /< ?���$88
���	C��z��0dn�O9��-������d�m�+�c��*|��A��=����E���ӈ�O�h\$k8I.�$K	O��P�Z����ZJZ o�9�̱�j:�������`�}�6k��x�4�81k��s���4�>�4�Ns������u��-Oe�즁�20��iy ��Td@$ǒt�e��E�z���H��y�u�<�Dc*����ؒ�ۼ���F�l�������'=��h�Y���4�/t�!�Zm�9�|��w|68�����F��ͬ�3]���>��ȹ=|�����o��a�6c��jZ�R��N� �B-J�%�d)�ID�"Y�I        [remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://cehr00e3sl32a"
path="res://.godot/imported/Delay.png-ddc30d99d825a46bfe7dca3143e359e3.ctex"
metadata={
"vram_texture": false
}
               RSRC                    PackedScene            ��������                                                   resource_local_to_scene    resource_name    code    script    shader    shader_parameter/laserColor    shader_parameter/lightInFlow1    shader_parameter/lightOutFlow1    shader_parameter/lightInFlow2    shader_parameter/lightOutFlow2    shader_parameter/lightInFlow3    shader_parameter/lightOutFlow3    shader_parameter/lightInFlow4    shader_parameter/lightOutFlow4    shader_parameter/lightInFlow5    shader_parameter/lightOutFlow5    shader_parameter/lightInFlow6    shader_parameter/lightOutFlow6    shader_parameter/lightInFlow7    shader_parameter/lightOutFlow7    shader_parameter/lightInFlow8    shader_parameter/lightOutFlow8 %   shader_parameter/numAnimationColumns    shader_parameter/animation1    shader_parameter/animation2    shader_parameter/animation3    shader_parameter/animation4    shader_parameter/animation5    shader_parameter/animation6    shader_parameter/animation7    shader_parameter/animation8 	   _bundled    
   Texture2D /   res://Scenes/Components/DelayTunable/Delay.png �[m[�&YF
   Texture2D 1   res://Scenes/Components/DelayTunable/Overlay.png ���L�*      local://Shader_rx45y C         local://ShaderMaterial_efdvx �         local://PackedScene_in88s ^         Shader          ]  // Laser Farbe
shader_type canvas_item;
render_mode blend_add;

uniform vec4 laserColor;

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
uniform float numAnimationColumns = 4.0;


vec4 getAnimationFrameColor(sampler2D animationTexture, vec2 uvCoord, float speed, vec4 lightInOutFlow) {
    float phaseShift = lightInOutFlow.y;
	    
    int currentFrameIndex = int(TIME * speed + phaseShift) % int(numAnimationColumns);
    float frameShift = float(currentFrameIndex) / numAnimationColumns; // frameshift shifts the UV.x so that the starting point is in the proper column
	if(speed < 0.0) // if the speed is negative, then the animation should playbackwards, so 0.75, 0.5, 0.25, 0
	{
		 frameShift = 0.75 - frameShift;
	}
	
    vec2 adjustedUV = vec2((uvCoord.x / numAnimationColumns + frameShift), uvCoord.y);
    return texture(animationTexture, adjustedUV);
}

float subtractBlueFromRedLight(vec4 currentInflowColor, float lightInflowIntensity){
	
	return currentInflowColor.a *(currentInflowColor.r - currentInflowColor.b) * lightInflowIntensity;
}
void fragment(){
	float animationspeed = 2.0f;
	
	vec4 col_baseTexture = texture(TEXTURE, UV);
	vec4 col_anim1in  = getAnimationFrameColor(animation1,UV,animationspeed, lightInFlow1);
	vec4 col_anim1out = getAnimationFrameColor(animation1,UV,-animationspeed, lightOutFlow1);
    vec4 col_anim2in  = getAnimationFrameColor(animation2,UV,animationspeed, lightInFlow2);
	vec4 col_anim2out = getAnimationFrameColor(animation2,UV,-animationspeed, lightOutFlow2);
    vec4 col_anim3in  = getAnimationFrameColor(animation3,UV,animationspeed, lightInFlow3);
	vec4 col_anim3out = getAnimationFrameColor(animation3,UV,-animationspeed, lightOutFlow3);
    vec4 col_anim4in  = getAnimationFrameColor(animation4,UV,animationspeed, lightInFlow4);
	vec4 col_anim4out = getAnimationFrameColor(animation4,UV,-animationspeed, lightOutFlow4);
    vec4 col_anim5in  = getAnimationFrameColor(animation5,UV,animationspeed, lightInFlow5);
	vec4 col_anim5out = getAnimationFrameColor(animation5,UV,-animationspeed, lightOutFlow5);
    vec4 col_anim6in  = getAnimationFrameColor(animation6,UV,animationspeed, lightInFlow6);
	vec4 col_anim6out = getAnimationFrameColor(animation6,UV,-animationspeed, lightOutFlow6);
    vec4 col_anim7in  = getAnimationFrameColor(animation7,UV,animationspeed, lightInFlow7);
	vec4 col_anim7out = getAnimationFrameColor(animation7,UV,-animationspeed, lightOutFlow7);
    vec4 col_anim8in  = getAnimationFrameColor(animation8,UV,animationspeed, lightInFlow8);
	vec4 col_anim8out = getAnimationFrameColor(animation8,UV,-animationspeed, lightOutFlow8);
    
  // Der rot Kanal = höhen, z.B.
  // Der blau Kanal = tiefen
  
  float intensity = subtractBlueFromRedLight( col_anim1in, lightInFlow1.x) +
		subtractBlueFromRedLight( col_anim2in, lightInFlow2.x) +
		subtractBlueFromRedLight( col_anim3in, lightInFlow3.x) +
		subtractBlueFromRedLight( col_anim4in, lightInFlow4.x) +
		subtractBlueFromRedLight( col_anim5in, lightInFlow5.x) +
		subtractBlueFromRedLight( col_anim6in, lightInFlow6.x) +
		subtractBlueFromRedLight( col_anim7in, lightInFlow7.x) +
		subtractBlueFromRedLight( col_anim8in, lightInFlow8.x) +
		subtractBlueFromRedLight( col_anim1out, lightOutFlow1.x) +
		subtractBlueFromRedLight( col_anim2out, lightOutFlow2.x) +
		subtractBlueFromRedLight( col_anim3out, lightOutFlow3.x) +
		subtractBlueFromRedLight( col_anim4out, lightOutFlow4.x) +
		subtractBlueFromRedLight( col_anim5out, lightOutFlow5.x) +
		subtractBlueFromRedLight( col_anim6out, lightOutFlow6.x) +
		subtractBlueFromRedLight( col_anim7out, lightOutFlow7.x) +
		subtractBlueFromRedLight( col_anim8out, lightOutFlow8.x);
	

  COLOR = laserColor * 2.5 * abs(intensity);
}          ShaderMaterial                    2   ���>          �?   2     �?               2                      2                   	   2                   
   2   �Aྏ�u�           2                      2   �Aྏ�u�           2                                                                           �@                                                               PackedScene          	         names "          DirectionalCoupler    custom_minimum_size    layout_direction    offset_right    offset_bottom    stretch_mode    TextureRect    RotationArea    unique_name_in_owner 	   position    Node2D    BackgroundImage    scale    texture 	   centered    region_rect 	   Sprite2D    Overlay 	   material    UI    DeltaLengthSlider    offset_left    offset_top 
   max_value    step    value    HSlider    SliderLabel    mouse_filter    bbcode_enabled    text    RichTextLabel    	   variants       
     �B  �B           �B            
     tB  tB
     t�  t�
      @   @                      8A  �A  $B  B                      xB   B     �@     �B     �B     �?)   {�G�z�?      ?     @@     �@     �B     B      [center]100       node_count             nodes     w   ��������       ����                                              
      ����         	                       ����   	                     	      
                    ����               	                     	                     
      ����                     ����                                                                    ����                                                             conn_count              conns               node_paths              editable_instances              version             RSRC    {
  "fileFormatVersion": 1,
  "identifier": "DelayTunable",
  "nazcaFunctionParameters": "deltaLength = SLIDER0",
  "nazcaFunctionName": "placeCell_Delay",
  "sceneResPath": "res://Scenes/Components/DelayTunable/DelayTunable.tscn",
  "widthInTiles": 2,
  "heightInTiles": 2,
  "pins": [
	{
	  "number": 0,
	  "name": "west",
	  "matterType": 1,
	  "side": 2,
	  "partX": 0,
	  "partY": 1
	},
	{
	  "number": 1,
	  "name": "east",
	  "matterType": 1,
	  "side": 0,
	  "partX": 1,
	  "partY": 1
	}
  ],
"sMatrices": [
	{
	"waveLength" : 1550,
	"connections": [
		{
		"fromPinNr": 0,
		"toPinNr": 1,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1550)"
		},
		{
		"fromPinNr": 1,
		"toPinNr": 0,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1550)"
		}
 	 ]
	},
	{
	"waveLength" : 1310,
	"connections": [
		{
		"fromPinNr": 0,
		"toPinNr": 1,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1310)"
		},
		{
		"fromPinNr": 1,
		"toPinNr": 0,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1310)"
		}
 	 ]
	},
	{
	"waveLength" : 980,
	"connections": [
		{
		"fromPinNr": 0,
		"toPinNr": 1,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 980)"
		},
		{
		"fromPinNr": 1,
		"toPinNr": 0,
		"nonLinearFormula" : "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 980)"
		}
 	 ]
	}
],
"overlays": [
	{
	  "overlayAnimTexturePath": "res://Scenes/Components/DelayTunable/Overlay.png",
	  "rectSide": 2,
	  "tileOffsetX": 0,
	  "tileOffsetY": 1
	}
  ],
	"sliders":
	[
		{
			"sliderNumber" : 0,
			"godotSliderName" : "DeltaLengthSlider",
			"godotSliderLabelName" : "SliderLabel",
			"minVal" : 0.0,
			"maxVal" : 1,
			"steps" : 100,
			"type" : 0
		}
	]
}
               RSRC                    Shader            ��������                                                  resource_local_to_scene    resource_name    code    script           local://Shader_bbate �          Shader            // Laser Farbe
shader_type canvas_item;
render_mode blend_add;

uniform vec4 laserColor;

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
uniform float numAnimationColumns = 4.0;
vec4 getAnimationFrameColor(sampler2D animationTexture, vec2 uvCoord, float speed, vec4 lightAttributes) {
	float elapsedTime = round(TIME * speed);
    float lightPhase = lightAttributes.y;
	float animationOffset = elapsedTime + round(lightPhase * speed);
	    
    int currentFrameIndex = int(animationOffset) % int(numAnimationColumns);
    float frameShift = float(currentFrameIndex) / numAnimationColumns;
	if(speed < 0.0) // if the speed is negative, then the animation should playbackwards, so 0.75, 0.5, 0.25, 0
	{
		 frameShift = 0.75 -abs(frameShift- 1.0/numAnimationColumns);
	}
    vec2 adjustedUV = vec2((uvCoord.x / numAnimationColumns + frameShift), uvCoord.y);
    return texture(animationTexture, adjustedUV);
}

float getIntensityOfLight(vec4 currentInflowColor, float lightInflowIntensity){
	
	return currentInflowColor.a *(currentInflowColor.r - currentInflowColor.b) * lightInflowIntensity;
}
void fragment(){
	float animationspeed = 2.0f;
	float animationTime = round(TIME * animationspeed);
	
	vec4 col_baseTexture = texture(TEXTURE, UV);
	vec4 col_anim1in  = getAnimationFrameColor(animation1,UV,animationspeed, lightInFlow1);
	vec4 col_anim1out = getAnimationFrameColor(animation1,UV,-animationspeed, lightOutFlow1);
    vec4 col_anim2in  = getAnimationFrameColor(animation2,UV,animationspeed, lightInFlow2);
	vec4 col_anim2out = getAnimationFrameColor(animation2,UV,-animationspeed, lightOutFlow2);
    vec4 col_anim3in  = getAnimationFrameColor(animation3,UV,animationspeed, lightInFlow3);
	vec4 col_anim3out = getAnimationFrameColor(animation3,UV,-animationspeed, lightOutFlow3);
    vec4 col_anim4in  = getAnimationFrameColor(animation4,UV,animationspeed, lightInFlow4);
	vec4 col_anim4out = getAnimationFrameColor(animation4,UV,-animationspeed, lightOutFlow4);
    vec4 col_anim5in  = getAnimationFrameColor(animation5,UV,animationspeed, lightInFlow5);
	vec4 col_anim5out = getAnimationFrameColor(animation5,UV,-animationspeed, lightOutFlow5);
    vec4 col_anim6in  = getAnimationFrameColor(animation6,UV,animationspeed, lightInFlow6);
	vec4 col_anim6out = getAnimationFrameColor(animation6,UV,-animationspeed, lightOutFlow6);
    vec4 col_anim7in  = getAnimationFrameColor(animation7,UV,animationspeed, lightInFlow7);
	vec4 col_anim7out = getAnimationFrameColor(animation7,UV,-animationspeed, lightOutFlow7);
    vec4 col_anim8in  = getAnimationFrameColor(animation8,UV,animationspeed, lightInFlow8);
	vec4 col_anim8out = getAnimationFrameColor(animation8,UV,-animationspeed, lightOutFlow8);
    
  // Der rot Kanal = höhen, z.B.
  // Der blau Kanal = tiefen
  
  float intensity = getIntensityOfLight( col_anim1in, lightInFlow1.x) +
		getIntensityOfLight( col_anim2in, lightInFlow2.x) +
		getIntensityOfLight( col_anim3in, lightInFlow3.x) +
		getIntensityOfLight( col_anim4in, lightInFlow4.x) +
		getIntensityOfLight( col_anim5in, lightInFlow5.x) +
		getIntensityOfLight( col_anim6in, lightInFlow6.x) +
		getIntensityOfLight( col_anim7in, lightInFlow7.x) +
		getIntensityOfLight( col_anim8in, lightInFlow8.x) +
		getIntensityOfLight( col_anim1out, lightOutFlow1.x) +
		getIntensityOfLight( col_anim2out, lightOutFlow2.x) +
		getIntensityOfLight( col_anim3out, lightOutFlow3.x) +
		getIntensityOfLight( col_anim4out, lightOutFlow4.x) +
		getIntensityOfLight( col_anim5out, lightOutFlow5.x) +
		getIntensityOfLight( col_anim6out, lightOutFlow6.x) +
		getIntensityOfLight( col_anim7out, lightOutFlow7.x) +
		getIntensityOfLight( col_anim8out, lightOutFlow8.x);

  COLOR = laserColor * 2.5 * abs(intensity);
}       RSRC GST2   �   =      ����               � =          RIFF  WEBPVP8L   /� �&���%�ȎM�O~�0�l��Q�jp�FMԂ¥��qE��̑y������% (�� v��]O ��q����g�j�zU�y�I��$o�t(}a���=y+'�LĠ
�x�{��i��qN?~F�_��F�$9�j'g/E�U���
���:�k�:�8 ��n�E�iI�®Ě��0��"ֲBXǊ�Y�:��B8�a�V��$�VX�ZS��5��Ou֚�-,buʫ��Y~U��K�ұb"�b�$I��� ��Y���$�s>Ƌ����l	���,�T9>VTU��(��JI5E��IU�/�cM�CPU��|>@/Ĳb$H2�$�J�XĂvD�aG�k�%��UX)),vD�UK)��q�a-��J�4]�%°��Z��9��}��Ё��A��Vը.�J֌��a`VXR�XA�N�O;��(V�F�b'�r��S�����Ǣ��Sm�Fǂ��+�$YKiUi�w�ښY0Ű�5����ꪵ��V��g�vbj�{#�}'��%�N�n����~��~���N;�:Vw�R�US�Ǫ�
�9�b�=����j�6U�HT��	ޏE�!8��װ��Z�# 9a�{f�j8&�E�{��up���d�R��f+gM�*�8�r"��s^Ŋ1�R�{G� 9o�5�N�Ϡ�
����@���Ah�Pj�v[��Tj�3蜰�Z4�V�`���Ӄ�Z����8�v[�
�L���xT��� �*`��v��%��b|E��X0�6;�Bk�kuC�bY2bJ�Jr�8�Z�֚�����Y���[ݬ�m�{�}{+�%Y5��Òra��o����h��V��s�%0L�eEU�������|v�+�����ZŊ1k�"��Rf�
T�+�������1v���p�b���{'9VNZj��.cu
�I�UUL=1p�X1N����**�WŜ�� �c�X&zر[�CLZ78�
Z��<��<�'�^5k�`͊Z�
����m+j%iٶ���^�X�����2֐���T`͊��2֐C�$R]�%��0(~q8�ʵ�
i^�:ၹ�ֱR��1�
kX'b1��K�ZI^�u�3[I���u�8���}�X���b����}�Xv��1���ٴ���u�0���}�XS�O�w���9CI��I��fELw��oA��\���������i�gO�cy��5�屼��O�`����ag�������#Y�Z��:V��m�U���'� k���CI^Ca+Ί�$����f�\��'O�����O���̫S��������?}�� ��믿^��NaŜ�X�����X�_yN��5�q\���,�I������������f�.sw�r+�,���7!x��Y:+�x룛帮b�T��<���j�SL;0;�E�N�ݣ�ăi�������%n�'c�Ǐ��[.s�C�o�Ϋ��������4�P!�����o�M�t,���	����G��?�I5kH��������g��&}~`�=5�wHs��wrj�4�!T<u��}`�����>f��dgf����'X�^Ί�'n���7�^jOO�S[�B����A��|������p̩��c�I����g�=Ή)��ͩ1�Ҳ��]8�Z;(�&[��<��;�r[x�N�?"�yk�֔�a���M���GD�jҨ���~��^UT����Us>�h?�7U��,�����&afG��[k������v���Ѿ��Ԣ�s�>Ł�݈:%�!Vw0�R��Z�}''!��:mt���ڽ9rLN��}<���xbj3�8�� nh���^��"H��ڵn5oU��`��ќv,�@��=L���%&Һ�U��"�$󇚵 ���lJ��R�pT�kkX~g��'�%.Ű��N���-��;�v���{߉���D�H8��T�}?l G������!�v�N�zo����(���]�e�.a�c�W���8�*P	&)[��T��c�@����c��E�=�
zy���6��7̧U8�j�����O�܀޵2�)v�N���|<�B@0h��Zۆ'�1�-Y\�8��|.�؉����9�8����'9�q�c��9��(&G��R�E3;rm��1$0��L_9�0Q�R�Q8+�k ph*�T�(�lq��nUa79��N�$ı�䐝ת��� ��#a2�R�o�g���A:p1%k���m0!E�pj���0�:M@��`
�������y��0쎻Wx�����bb"b��4�N��J�[�mu�tD�&�k�ڌx҃�A�R-�FO�Z��m���L��w�cw��FQˁ�!:1甒)i���qؽ@���YK-�5n�U'��u��dHfa�Zj.�h�w����k0ඦ9A������W)e�x�>�?����RJ�bG|,ﱐ��	���}�9�;G*s���SzT�AY{���	#
5p�4�xϰ|R�/+:��)=C6*� �g�N�v,�+7��Ե5<�"�+�:�Վ:u,7�d�Q��~�í[V��Q�#�`�`�I�AоM�FX]\� �l�H��s�r"�?O��G̹
��nDep��F:U�pD9*/�#�Bn8�&�'G����aؗc��R˗�?�_��a����Q\�9�e��8���!�r�YT��O�~�RW�̞9k�t���rnp].�GT8YP��-��q�]�g|1��b��tL�jA1ގ�#*wn�B�BES�ҁ[B8��`u�	���^+kr纺�|���^NB���֪�?G���vZ�עn��۲n����G}�f���疾�wQ���    [remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://bj3urk3b0d0ke"
path="res://.godot/imported/Overlay.png-a128810a0d69c6124e0eaea786068b57.ctex"
metadata={
"vram_texture": false
}
             GST2   �   �      ����               � �        �  RIFF�  WEBPVP8L�  /������!"2�H�$�n윦���z�x����դ�<����q����F��Z��?&,
ScI_L �;����In#Y��0�p~��Z��m[��N����R,��#"� )���d��mG�������ڶ�$�ʹ���۶�=���mϬm۶mc�9��z��T��7�m+�}�����v��ح�m�m������$$P�����එ#���=�]��SnA�VhE��*JG�
&����^x��&�+���2ε�L2�@��		��S�2A�/E���d"?���Dh�+Z�@:�Gk�FbWd�\�C�Ӷg�g�k��Vo��<c{��4�;M�,5��ٜ2�Ζ�yO�S����qZ0��s���r?I��ѷE{�4�Ζ�i� xK�U��F�Z�y�SL�)���旵�V[�-�1Z�-�1���z�Q�>�tH�0��:[RGň6�=KVv�X�6�L;�N\���J���/0u���_��U��]���ǫ)�9��������!�&�?W�VfY�2���༏��2kSi����1!��z+�F�j=�R�O�{�
ۇ�P-�������\����y;�[ ���lm�F2K�ޱ|��S��d)é�r�BTZ)e�� ��֩A�2�����X�X'�e1߬���p��-�-f�E�ˊU	^�����T�ZT�m�*a|	׫�:V���G�r+�/�T��@U�N׼�h�+	*�*sN1e�,e���nbJL<����"g=O��AL�WO!��߈Q���,ɉ'���lzJ���Q����t��9�F���A��g�B-����G�f|��x��5�'+��O��y��������F��2�����R�q�):VtI���/ʎ�UfěĲr'�g�g����5�t�ۛ�F���S�j1p�)�JD̻�ZR���Pq�r/jt�/sO�C�u����i�y�K�(Q��7őA�2���R�ͥ+lgzJ~��,eA��.���k�eQ�,l'Ɨ�2�,eaS��S�ԟe)��x��ood�d)����h��ZZ��`z�պ��;�Cr�rpi&��՜�Pf��+���:w��b�DUeZ��ڡ��iA>IN>���܋�b�O<�A���)�R�4��8+��k�Jpey��.���7ryc�!��M�a���v_��/�����'��t5`=��~	`�����p\�u����*>:|ٻ@�G�����wƝ�����K5�NZal������LH�]I'�^���+@q(�q2q+�g�}�o�����S߈:�R�݉C������?�1�.��
�ڈL�Fb%ħA ����Q���2�͍J]_�� A��Fb�����ݏ�4o��'2��F�  ڹ���W�L |����YK5�-�E�n�K�|�ɭvD=��p!V3gS��`�p|r�l	F�4�1{�V'&����|pj� ߫'ş�pdT�7`&�
�1g�����@D�˅ �x?)~83+	p �3W�w��j"�� '�J��CM�+ �Ĝ��"���4� ����nΟ	�0C���q'�&5.��z@�S1l5Z��]�~L�L"�"�VS��8w.����H�B|���K(�}
r%Vk$f�����8�ڹ���R�dϝx/@�_�k'�8���E���r��D���K�z3�^���Vw��ZEl%~�Vc���R� �Xk[�3��B��Ğ�Y��A`_��fa��D{������ @ ��dg�������Mƚ�R�`���s����>x=�����	`��s���H���/ū�R�U�g�r���/����n�;�SSup`�S��6��u���⟦;Z�AN3�|�oh�9f�Pg�����^��g�t����x��)Oq�Q�My55jF����t9����,�z�Z�����2��#�)���"�u���}'�*�>�����ǯ[����82һ�n���0�<v�ݑa}.+n��'����W:4TY�����P�ר���Cȫۿ�Ϗ��?����Ӣ�K�|y�@suyo�<�����{��x}~�����~�AN]�q�9ޝ�GG�����[�L}~�`�f%4�R!1�no���������v!�G����Qw��m���"F!9�vٿü�|j�����*��{Ew[Á��������u.+�<���awͮ�ӓ�Q �:�Vd�5*��p�ioaE��,�LjP��	a�/�˰!{g:���3`=`]�2��y`�"��N�N�p���� ��3�Z��䏔��9"�ʞ l�zP�G�ߙj��V�>���n�/��׷�G��[���\��T��Ͷh���ag?1��O��6{s{����!�1�Y�����91Qry��=����y=�ٮh;�����[�tDV5�chȃ��v�G ��T/'XX���~Q�7��+[�e��Ti@j��)��9��J�hJV�#�jk�A�1�^6���=<ԧg�B�*o�߯.��/�>W[M���I�o?V���s��|yu�xt��]�].��Yyx�w���`��C���pH��tu�w�J��#Ef�Y݆v�f5�e��8��=�٢�e��W��M9J�u�}]釧7k���:�o�����Ç����ս�r3W���7k���e�������ϛk��Ϳ�_��lu�۹�g�w��~�ߗ�/��ݩ�-�->�I�͒���A�	���ߥζ,�}�3�UbY?�Ӓ�7q�Db����>~8�]
� ^n׹�[�o���Z-�ǫ�N;U���E4=eȢ�vk��Z�Y�j���k�j1�/eȢK��J�9|�,UX65]W����lQ-�"`�C�.~8ek�{Xy���d��<��Gf�ō�E�Ӗ�T� �g��Y�*��.͊e��"�]�d������h��ڠ����c�qV�ǷN��6�z���kD�6�L;�N\���Y�����
�O�ʨ1*]a�SN�=	fH�JN�9%'�S<C:��:`�s��~��jKEU�#i����$�K�TQD���G0H�=�� �d�-Q�H�4�5��L�r?����}��B+��,Q�yO�H�jD�4d�����0*�]�	~�ӎ�.�"����%
��d$"5zxA:�U��H���H%jس{���kW��)�	8J��v�}�rK�F�@�t)FXu����G'.X�8�KH;���[             [remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://d0e42gfw1cr43"
path="res://.godot/imported/icon.svg-218a8f2b3041327d8a5756f3a245f83b.ctex"
metadata={
"vram_texture": false
}
                [remap]

path="res://.godot/exported/133200997/export-2aeef5e0df21011397c42ede8bc5c399-DelayTunable.scn"
       [remap]

path="res://.godot/exported/133200997/export-e3428c87e0b653f4a1d466be67e97529-LightOverlayShaded.res"
 list=Array[Dictionary]([])
     <svg height="128" width="128" xmlns="http://www.w3.org/2000/svg"><rect x="2" y="2" width="124" height="124" rx="14" fill="#363d52" stroke="#212532" stroke-width="4"/><g transform="scale(.101) translate(122 122)"><g fill="#fff"><path d="M105 673v33q407 354 814 0v-33z"/><path fill="#478cbf" d="m105 673 152 14q12 1 15 14l4 67 132 10 8-61q2-11 15-15h162q13 4 15 15l8 61 132-10 4-67q3-13 15-14l152-14V427q30-39 56-81-35-59-83-108-43 20-82 47-40-37-88-64 7-51 8-102-59-28-123-42-26 43-46 89-49-7-98 0-20-46-46-89-64 14-123 42 1 51 8 102-48 27-88 64-39-27-82-47-48 49-83 108 26 42 56 81zm0 33v39c0 276 813 276 813 0v-39l-134 12-5 69q-2 10-14 13l-162 11q-12 0-16-11l-10-65H447l-10 65q-4 11-16 11l-162-11q-12-3-14-13l-5-69z"/><path d="M483 600c3 34 55 34 58 0v-86c-3-34-55-34-58 0z"/><circle cx="725" cy="526" r="90"/><circle cx="299" cy="526" r="90"/></g><g fill="#414042"><circle cx="307" cy="532" r="60"/><circle cx="717" cy="532" r="60"/></g></g></svg>
             ���L�*=   res://Scenes/Components/DelayTunable/Delay Line_A_Overlay.png�[m[�&YF.   res://Scenes/Components/DelayTunable/Delay.png<k?���6   res://Scenes/Components/DelayTunable/DelayTunable.tscnP�@q;��M<   res://Scenes/Components/DelayTunable/LightOverlayShaded.tresj��G.��{   res://icon.svg���L�*0   res://Scenes/Components/DelayTunable/Overlay.png         ECFG      application/config/name          DirectionalCouplerPCK      application/run/main_sceneP      G   res://Scenes/Components/DirectionalCoupler/DirectionalCouplerScene.tscn    application/config/features$   "         4.2    Forward Plus       application/config/icon         res://icon.svg     dotnet/project/assembly_name         Bend Pck     