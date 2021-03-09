#version 330 core

in vec2 UV;
in vec3 Position_worldspace;
in vec3 EyeDirection_cameraspace;
in vec3 LightDirection_cameraspace;

in vec3 LightDirection_tangentspace;
in vec3 EyeDirection_tangentspace;

in vec3 Normal;
in vec3 Position;

out vec4 FragColor;

uniform sampler2D DiffuseTex;
uniform sampler2D NormalTex;
uniform sampler2D SpecularTex;
uniform mat4 view;
uniform mat4 model;
uniform mat3 MV3x3;
uniform vec3 LightPosW;

uniform vec3 cameraPos; 

uniform samplerCube skybox;


void main(){
	//------------------------------------------------------------------------------
	//float specBase = saturate(dot(reflect(-normalize(viewVec), normal),lightDir));
// Perturb a grid pattern with some noise and with the view-vector
// to let the glittering change with view.
	// float3 fp = frac(0.7 * pos + 9 * Noise3D( pos * 0.04).r + 0.1 * viewVec);
	// fp *= (1 - fp);
	// float glitter = saturate(1 - 7 * (fp.x + fp.y + fp.z));
	// float sparkle = glitter * pow(specBase, 1.5);
	//-------------------------------------------------------------------------------
	
	float specBase = dot(reflect(normalize(EyeDirection_tangentspace), texture( NormalTex, vec2(UV.x,-UV.y)).rgb),EyeDirection_tangentspace);
	specBase = clamp(specBase,0,1);
	vec3 fp = fract(0.7 * Position_worldspace + 9 * noise3( Position_worldspace * 0.04).r + 0.1 * EyeDirection_tangentspace);
	fp *= (1 - fp);
	float glitter = clamp((1 - 7 * (fp.x + fp.y + fp.z)),0.0,1.0);
	float sparkle = glitter * pow(specBase, 1.5);
	
	vec3 Incident = normalize (Position - cameraPos);
	vec3 Norm = normalize (Normal);
	//vec3 Reflected = reflect(Incident, Norm);
	//vec3 Reflected = reflect(Incident,texture( NormalTex, vec2(UV.x,-UV.y) ).rgb);
	
	float ratio = 1.0 /1.52;
	//vec3 Refracted = refract (Incident, Norm, ratio);
	vec3 Refracted = refract (Incident, texture( NormalTex, vec2(UV.x,-UV.y) ).rgb, ratio);
	
	//FragColor = texture (skybox, Reflected);

	vec3 LightColor = vec3(1,1,1);
	float LightPower = 40.0;
	
	vec3 MaterialDiffuseColor = texture( DiffuseTex, UV ).rgb;
	vec3 MaterialAmbientColor = vec3(0.1,0.1,0.1)/*vec3(0.01,0.01,0.01)*/ * MaterialDiffuseColor;
	vec3 MaterialSpecularColor = texture( SpecularTex, UV ).rgb * 0.3;

	
	vec3 TextureNormal_tangentspace = normalize(texture( NormalTex, vec2(UV.x,-UV.y) ).rgb*2.0 - 1.0);
	
	float distance = length( LightPosW - Position_worldspace );

	vec3 n = TextureNormal_tangentspace;
	vec3 l = normalize(LightDirection_tangentspace);
	float cosTheta = clamp( dot( n,l ), 0,1 );

	vec3 E = normalize(EyeDirection_tangentspace);
	vec3 R = reflect(-l,n);
	float cosAlpha = clamp( dot( E,R ), 0,1 );
	
    vec3 colorR = (texture (skybox, Refracted)).rgb;
	//vec3 colorR = (texture (skybox, Reflected)).rgb;
	vec3 colorFin = 
		 glitter * sparkle + colorR /*MaterialAmbientColor*/ +
		 colorR * (glitter * sparkle + MaterialDiffuseColor) * LightColor * LightPower * cosTheta / (distance*distance) +
		 colorR * (glitter * sparkle + MaterialSpecularColor) * LightColor * LightPower * pow(cosAlpha,5) / (distance*distance);
		 
	FragColor = vec4(colorFin,1.0);

}