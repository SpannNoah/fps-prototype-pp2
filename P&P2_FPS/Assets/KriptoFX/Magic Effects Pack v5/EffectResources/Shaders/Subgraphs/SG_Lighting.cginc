
struct MagicFX5_LightData
{
	float4 color;
	float3 position;
	float range;
};

float3 MagicFX5_DirLightColor;
float3 MagicFX5_DirLightVector;

uint MagicFX5_AdditionalLightsCount;
StructuredBuffer<MagicFX5_LightData> MagicFX5_AdditionalLightsBuffer;


inline float DistanceAttenuation(float distanceSqr, float range)
{
	float lightRange = rcp(range * range);
	float atten = distanceSqr * lightRange;
	atten = rcp(1.0 + 25.0 * atten) * saturate((1.0 - atten) * 2.0);
	return atten;
}

//inline half PointLightAttenuation(uint lightIndex, float3 worldPos)
//{
//	LightData light = MagicFX5_AdditionalLightsCount[lightIndex];

//	float3 lightVector = worldPos - light.position.xyz;
//	float distanceSqr = max(dot(lightVector, lightVector), HalfMin);
//	half attenuation = DistanceAttenuation(distanceSqr, light.range);

//	return attenuation;
//}


inline void GetMainLight_float(out float3 mainLightColor, out float3 mainLightVector)
{
	#ifdef SHADERGRAPH_PREVIEW
		mainLightColor = 0;
		mainLightVector = 0;
	#else
		mainLightColor = MagicFX5_DirLightColor;
		mainLightVector = MagicFX5_DirLightVector;
	#endif
}



float3 CustomLightHandling(float3 worldPos, float3 worldSpaceTangent, float3 worldSpaceBiTangent, float3 worldSpaceNormal, float3 sixMap0, float3 sixMap1)
{
	#ifndef SHADERGRAPH_PREVIEW
		//right left top sixMap0
		//bot, front, back sixMap1
		float3 radiance = 0;

		float3x3 tangentTransform_World = float3x3(worldSpaceTangent, worldSpaceBiTangent, worldSpaceNormal);
		float3 color = 0;

		UNITY_LOOP
		for (int lightIdx = 0; lightIdx < MagicFX5_AdditionalLightsCount; lightIdx++)
		{
			MagicFX5_LightData light = MagicFX5_AdditionalLightsBuffer[lightIdx];
			float3 lightPos = light.position;
			float3 lightVector = worldPos - light.position.xyz;
			float distanceSqr = max(dot(lightVector, lightVector), 6.103515625e-5);
			half attenuation = DistanceAttenuation(distanceSqr, light.range);

			float3 lightDirectionTangent = TransformWorldToTangent(normalize(lightVector), tangentTransform_World);

			float light_X = (lightDirectionTangent.x > 0.0 ?     sixMap0.y : sixMap0.x).x;
			float light_Y = (lightDirectionTangent.y > 0.0 ?     sixMap1.x : sixMap0.z).x;
			float light_Z = (lightDirectionTangent.z > 0.0 ?     sixMap1.z : sixMap1.y).x;

			light_X *= smoothstep(0.0, 1.0, abs(lightDirectionTangent.x));
			light_Y *= smoothstep(0.0, 1.0, abs(lightDirectionTangent.y));
			light_Z *= smoothstep(0.0, 1.0, abs(lightDirectionTangent.z));
			float combined_light = light_X + light_Y + light_Z;

			color += combined_light * attenuation * light.color;
		}
		
		//color *= radiance;


		return max(0, color);
	#endif
}

inline void CalculateSixWayLighting_float(float3 worldPos, float3 worldSpaceTangent, float3 worldSpaceBiTangent, float3 worldSpaceNormal, float3 sixMap0, float3 sixMap1, out float3 color)
{
	#ifdef SHADERGRAPH_PREVIEW
		color = 0.5;
	#else
		color = CustomLightHandling(worldPos, worldSpaceTangent, worldSpaceBiTangent, worldSpaceNormal, sixMap0, sixMap1);
	#endif
}

inline void GetExposure_float(out float exposure)
{
	exposure = 1;
	#ifndef SHADERGRAPH_PREVIEW
		#ifdef SHADEROPTIONS_PRE_EXPOSITION
			exposure = GetCurrentExposureMultiplier();
		#endif
	#endif
}

inline void GetPlatformSpecificEmissionMultiplier_float(out float multiplier)
{
	multiplier = 1;
	#ifndef SHADERGRAPH_PREVIEW
		#ifdef SHADEROPTIONS_PRE_EXPOSITION
			multiplier = 10;
		#endif
	#endif
}