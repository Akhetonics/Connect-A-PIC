GDPC                                                                                         X   res://.godot/exported/133200997/export-294b3b8539157f828db755cb5c72f0fe-Termination.scn `      
      �e!��eb�V���;�    `   res://.godot/exported/133200997/export-55efda72fcd83cc54a3762240963ba22-LightOverlayShaded.res  �            ���^��tS��][�    ,   res://.godot/global_script_class_cache.cfg  6             9>�ʱ�p�˛OL�7u    L   res://.godot/imported/Background.png-0710f3bf6c51bd70471991b45858a2f2.ctex          �       ���יp�s
xfӀ��    H   res://.godot/imported/Overlay.png-9b7d5d9a1d0da5b1668bad25b7f37ea6.ctex �      �      mtB�,<z�<�z1g%    D   res://.godot/imported/icon.svg-218a8f2b3041327d8a5756f3a245f83b.ctex�'      �      �̛�*$q�*�́        res://.godot/uid_cache.bin  �9            �W�'��Y8 �3��    <   res://Scenes/Components/Termination/Background.png.import          �       �^��ˡ����\��    D   res://Scenes/Components/Termination/LightOverlayShaded.tres.remap   05      o       D�K�w��
��w s�    8   res://Scenes/Components/Termination/Overlay.png.import  �      �       P-���O��K�    <   res://Scenes/Components/Termination/Termination.tscn.remap  �5      h       (��~}�<�:~D�8*    @   res://Scenes/Components/Termination/TerminationDescription.json �$      �      ���_����tZ1�F       res://icon.svg   6      �      b�pW>���d���       res://icon.svg.import   `4      �       ��uO�M׷x��s��       res://project.binary ;      �       ���(�.�%��                GST2            ����                        �   RIFF�   WEBPVP8L�   /@7���$8���ݹ��	�P����_H�m���K�������w�y��UJ�ܺ����d�l>��u�94q6���E���f�fp7��6�M���W����U�;��D�	�	�"L�${N���=kLTv�	� F;]�3�w��;Q@�Qm5h�`0����"�/     [remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://d0qmakancnmy3"
path="res://.godot/imported/Background.png-0710f3bf6c51bd70471991b45858a2f2.ctex"
metadata={
"vram_texture": false
}
          RSRC                    Shader            ��������                                                  resource_local_to_scene    resource_name    code    script           local://Shader_qq1f6 �          Shader          !  // Laser Farbe
shader_type canvas_item;
render_mode blend_add;

uniform int version = 1;
uniform vec4 laserColor;

uniform vec4 lightInFlow1; // x = intensity, y = phase in RAD, z = offsetx, w = offsety
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
uniform  float numAnimationColumns = 4.0;

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
	
	return currentInflowColor.a *(currentInflowColor.r - currentInflowColor.b) * abs(lightInflowIntensity);
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
}       RSRC      GST2   x         ����               x         f  RIFF^  WEBPVP8LR  /w@G�$��j#��8�d�����`�V��X���5Q
I	��c�V�3TP�@$z�<
�JA�6L���#yp�p��H�g>��0h���Ľ�,+�@�=�����)G$`�į�E\{����E�`P���Ub�,Eҟ�-ܟ�Rd�zJ����U���	n#9�����w��G� `�AD�H˓�Zk�Ec����Z�ˈ:h�ux;�	�S+ּ��E������}Hc�Cun�46u��Qr��em��YuJu�'��-qV-j}Lq��^�Ed.�Qe.��9Z�G���7��Į��\M�ŏQW�%�@�i!�����d6�� \P��[j�F�#�e#I��1 ���slK�g�;�Š�z��cZ
$��uٌ�S%����>:�&�ӳtɰyX��Ie1GC�\��b�Rs�}��>�&\U����P��פ/��C���:������?��bv��8M�!�O��n�q&6I&����z�j����z.W�>���[8����nN������'NEP���Ir.�dN�~aZ�7�b[�rЇy�/��4�����nWI�t�,�������$�}N=yrm���+��
w��i���n�ۥgo�����Ir�$���\o����o!�����<���`?����9��c?������}}}�u.Ȝ��gr��83r�8%/����؟k��:��`��|�] �Ş$������y(��4���]��6���L���������=]��>�6�����?�i�ɓ�ˤ�>��-��b�s�˝(B#�G���Jk~���z�����&+z���s��`�d}��W��7�V��GxT^H��U��A6���ӟ|Mly��d����j�����Y�zk Mz�.�<V��Z�b������㲸~�\lj.�O?�=�GG�2�Z.1��h����۴\����� �qe� z�b�� vG#�A�y�\Fe꥛�}��`�\
D��uQ�C�E.��y �����q$���9w��	Lq��I�<�\*���>%��;�>�P����X�|23�uH.k�pA�к]�=���2_.���肮0λ�"`���}  [remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://dw5fh43pqyep"
path="res://.godot/imported/Overlay.png-9b7d5d9a1d0da5b1668bad25b7f37ea6.ctex"
metadata={
"vram_texture": false
}
              RSRC                    PackedScene            ��������                                                   resource_local_to_scene    resource_name    shader    shader_parameter/version    shader_parameter/laserColor    shader_parameter/lightInFlow1    shader_parameter/lightOutFlow1    shader_parameter/lightInFlow2    shader_parameter/lightOutFlow2    shader_parameter/lightInFlow3    shader_parameter/lightOutFlow3    shader_parameter/lightInFlow4    shader_parameter/lightOutFlow4    shader_parameter/lightInFlow5    shader_parameter/lightOutFlow5    shader_parameter/lightInFlow6    shader_parameter/lightOutFlow6    shader_parameter/lightInFlow7    shader_parameter/lightOutFlow7    shader_parameter/lightInFlow8    shader_parameter/lightOutFlow8 %   shader_parameter/numAnimationColumns    shader_parameter/animation1    shader_parameter/animation2    shader_parameter/animation3    shader_parameter/animation4    shader_parameter/animation5    shader_parameter/animation6    shader_parameter/animation7    shader_parameter/animation8    script 	   _bundled    
   Texture2D 3   res://Scenes/Components/Termination/Background.png $�:�,|   Shader <   res://Scenes/Components/Termination/LightOverlayShaded.tres ��3Q
   Texture2D 0   res://Scenes/Components/Termination/Overlay.png ���Q0�      local://ShaderMaterial_1ai6y �         local://PackedScene_jved3 �         ShaderMaterial                            2     �?          �?   2     �?                              	      
                                                                          �@                                                               PackedScene          	         names "         TextureRect    offset_right    offset_bottom    stretch_mode    RotationArea 	   position    Node2D    Background    scale    texture 	   centered 	   Sprite2D    Overlay    unique_name_in_owner 	   material    UI    	   variants    	        pB      
     �A  �A
     ��  ��
      @   @                                       node_count             nodes     A   ��������        ����                                         ����                          ����               	      
                       ����                           	      
                        ����                   conn_count              conns               node_paths              editable_instances              version             RSRC     {
  "fileFormatVersion": 1,
  "identifier": "Termination",
  "nazcaFunctionParameters": "",
  "nazcaFunctionName": "placeCell_Termination",
  "sceneResPath": "res://Scenes/Components/Termination/Termination.tscn",
  "deltaLength": 0,
  "widthInTiles": 1,
  "heightInTiles": 1,
  "pins": [
	{
	  "number": 0,
	  "name": "west",
	  "matterType": 1,
	  "side": 2,
	  "partX": 0,
	  "partY": 0
	}
  ],
  "sMatrices": [
	{
	  "waveLength" : 1550,
	  "connections": [
	  ]
	},
	{
	  "waveLength" : 1310,
	  "connections": [
		
	  ]
	},
	{
	  "waveLength" : 980,
	  "connections": [
	  ]
	}
  ],
  "overlays": [
	{
	  "overlayAnimTexturePath": "res://Scenes/Components/Termination/Overlay.png",
	  "rectSide": 2,
	  "tileOffsetX": 0,
	  "tileOffsetY": 0
	}
  ]
}
            GST2   �   �      ����               � �        �  RIFF�  WEBPVP8L�  /������!"2�H�$�n윦���z�x����դ�<����q����F��Z��?&,
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
uid="uid://bofqeljil3hrs"
path="res://.godot/imported/icon.svg-218a8f2b3041327d8a5756f3a245f83b.ctex"
metadata={
"vram_texture": false
}
                [remap]

path="res://.godot/exported/133200997/export-55efda72fcd83cc54a3762240963ba22-LightOverlayShaded.res"
 [remap]

path="res://.godot/exported/133200997/export-294b3b8539157f828db755cb5c72f0fe-Termination.scn"
        list=[]
        <svg height="128" width="128" xmlns="http://www.w3.org/2000/svg"><rect x="2" y="2" width="124" height="124" rx="14" fill="#363d52" stroke="#212532" stroke-width="4"/><g transform="scale(.101) translate(122 122)"><g fill="#fff"><path d="M105 673v33q407 354 814 0v-33z"/><path fill="#478cbf" d="m105 673 152 14q12 1 15 14l4 67 132 10 8-61q2-11 15-15h162q13 4 15 15l8 61 132-10 4-67q3-13 15-14l152-14V427q30-39 56-81-35-59-83-108-43 20-82 47-40-37-88-64 7-51 8-102-59-28-123-42-26 43-46 89-49-7-98 0-20-46-46-89-64 14-123 42 1 51 8 102-48 27-88 64-39-27-82-47-48 49-83 108 26 42 56 81zm0 33v39c0 276 813 276 813 0v-39l-134 12-5 69q-2 10-14 13l-162 11q-12 0-16-11l-10-65H447l-10 65q-4 11-16 11l-162-11q-12-3-14-13l-5-69z"/><path d="M483 600c3 34 55 34 58 0v-86c-3-34-55-34-58 0z"/><circle cx="725" cy="526" r="90"/><circle cx="299" cy="526" r="90"/></g><g fill="#414042"><circle cx="307" cy="532" r="60"/><circle cx="717" cy="532" r="60"/></g></g></svg>
            $�:�,|2   res://Scenes/Components/Termination/Background.png��3Q;   res://Scenes/Components/Termination/LightOverlayShaded.tres���Q0�/   res://Scenes/Components/Termination/Overlay.pngm[��t�-4   res://Scenes/Components/Termination/Termination.tscn@Jj8�x�.   res://icon.svg  ECFG      application/config/name      
   GratingPCK     application/config/features$   "         4.2    Forward Plus       application/config/icon         res://icon.svg     dotnet/project/assembly_name         Connect A Pic Test         