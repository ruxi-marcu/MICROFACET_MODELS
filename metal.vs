#version 330 core

layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec2 vertexUV;
layout(location = 2) in vec3 vertexNormal_modelspace;
layout(location = 3) in vec3 vertexTangent_modelspace;
layout(location = 4) in vec3 vertexBitangent_modelspace;

out vec2 UV;
out vec3 Position_worldspace;
out vec3 EyeDirection_cameraspace;
out vec3 LightDirection_cameraspace;

out vec3 LightDirection_tangentspace;
out vec3 EyeDirection_tangentspace;

out vec3 Normal;
out vec3 Position;

uniform mat4 mvp;
uniform mat4 view;
uniform mat4 model;
uniform mat4 proj;
uniform mat3 mv3x3;
uniform vec3 LightPosW;

void main(){
	
	Normal = mat3(transpose(inverse(model))) * vertexNormal_modelspace;
    Position = vec3(model * vec4(vertexPosition_modelspace, 1.0));
    //gl_Position = proj * view * model * vec4(vertexPosition_modelspace, 1.0);

	gl_Position =  mvp * vec4(vertexPosition_modelspace,1);
	
	Position_worldspace = (model * vec4(vertexPosition_modelspace,1)).xyz;
	
	vec3 vertexPosition_cameraspace = ( view * model * vec4(vertexPosition_modelspace,1)).xyz;
	EyeDirection_cameraspace = vec3(0,0,0) - vertexPosition_cameraspace;

	vec3 LightPosition_cameraspace = ( view * vec4(LightPosW,1)).xyz;
	LightDirection_cameraspace = LightPosition_cameraspace + EyeDirection_cameraspace;
	
	UV = vertexUV;
	
	vec3 vertexTangent_cameraspace = mv3x3 * vertexTangent_modelspace;
	vec3 vertexBitangent_cameraspace = mv3x3 * vertexBitangent_modelspace;
	vec3 vertexNormal_cameraspace = mv3x3 * vertexNormal_modelspace;
	
	mat3 TBN = transpose(mat3(
		 vertexTangent_cameraspace,
		 vertexBitangent_cameraspace,
		 vertexNormal_cameraspace	
	)); 

	LightDirection_tangentspace = TBN * LightDirection_cameraspace;
	EyeDirection_tangentspace =  TBN * EyeDirection_cameraspace;
	
	
}