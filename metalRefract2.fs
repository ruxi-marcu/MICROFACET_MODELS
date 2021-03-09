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
	
	vec3 Incident = normalize (Position - cameraPos);
	vec3 Norm = normalize (Normal);
	//vec3 Reflected = reflect(Incident, Norm);
	float ratio = 1.0 /1.52;
	vec3 Refracted = refract (Incident, Norm, ratio);
	
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
	vec3 colorFin = 
		 colorR /*MaterialAmbientColor*/ +
		 colorR * /*MaterialDiffuseColor */ LightColor * LightPower * cosTheta / (distance*distance) +
		 colorR * MaterialSpecularColor * LightColor * LightPower * pow(cosAlpha,5) / (distance*distance);
		 
	FragColor = vec4(colorFin,1.0);

}