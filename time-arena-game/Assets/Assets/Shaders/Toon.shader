Shader "Custom/Toon"
{
	Properties
	{		
		_Color("Color", Color) = (0.5, 0.65, 1, 1) //base (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}	//base "white"
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1) //base (0.4,0.4,0.4,1)
		_ShadowAmount("Shadow amount [.1]", Float) = 0.01 //base 0.1
		_ShadowAmount2("Shadow amount2 [.1]", Float) = 0.01 //base 0.1

		//specular colour manipulation
		[HDR]
		_SpecularColor("Specular Color 1", Color) = (0.9,0.9,0.9,1) //base (0.9,0.9,0.9,1)
		_Glossiness("Glossiness 1 [32]", Float) = 32 //base 32
		[HDR]
		_SpecularColor2("Specular Color 2", Color) = (0.9,0.9,0.9,1) //base (0.9,0.9,0.9,1)		
		_GlossinessSecond("Glossiness 2 [32]", Float) = 32 //base 32
		[HDR]
		_SpecularColor3("Specular Color 3", Color) = (0,0,0,1) //base (0,0,0,1)
		_GlossinessThird("Glossiness 3 [32]", Float) = 32 //base 32

		_SpecularSmooth("Specular lower [.005]", Float) = 0.005 //base 0.005
		_SpecularSmoothUpper("Specular upper [.01]", Float) = 0.01 //base 0.01

		//rim manipulation
		_RimColor("Rim Color", Color) = (1,1,1,1) //base (1,1,1,1)
		_RimAmount("Rim Amount [.716]", Float) = 0.716 //base 0.716
		_RimThreshold("Rim Threshold [.1]", Float) = 0.1 //base 0.1
		//rime dark edge manipulation
		_RimEdgeColor("Rim Edge Color", Color) = (1,1,1,1) //base (1,1,1,1) | maintain at white for a black edge
		_RimEdgeAmount("Rim Edge Amount [.85]", Float) = 0.85 //base 0.716 | set to 1 for no edge
		_RimEdgeThreshold("Rim Edge Threshold [.05]", Float) = 0.05 //base 0.1 | set to 1 for no edge
		_RimEdgeTotal("Complete rim edge [0]", Float) = 0 // base 0

		
		
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float3 normal : NORMAL;
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 worldNormal : NORMAL;
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 _AmbientColor;

			float _Glossiness;

			float _GlossinessSecond;

			float _GlossinessThird;

			float4 _SpecularColor;

			float4 _SpecularColor2;

			float4 _SpecularColor3;

			float _SpecularSmooth;

			float _SpecularSmoothUpper;

			float4 _RimColor;

			float _RimAmount;

			float _RimThreshold;

			float4 _RimEdgeColor;

			float _RimEdgeAmount;

			float _RimEdgeThreshold;

			float _RimEdgeTotal;

			float _ShadowAmount;

			float _ShadowAmount2;


			float4 frag (v2f i) : SV_Target
			{
				//aquire world normal and normal dot product
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				
				//shadow map
				float shadow = SHADOW_ATTENUATION(i);

				//aquire light vector
				float lightIntensity = smoothstep(_ShadowAmount - 0.01, _ShadowAmount + 0.01, NdotL* shadow);
				float4 light = lightIntensity * _LightColor0;

				//aquire light vector
				float lightIntensity2 = smoothstep(_ShadowAmount2 - 0.01, _ShadowAmount2 + 0.01, NdotL* shadow);
				float4 light2 = lightIntensity2 * _LightColor0;
				
				//aquire view vector
				float3 viewDir = normalize(i.viewDir);
				
				//aquire half vector and half vector dot product
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				//add specular highlights
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(_SpecularSmooth, _SpecularSmoothUpper, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;

				float specularIntensity2 = pow(NdotH * lightIntensity, _GlossinessSecond * _GlossinessSecond);
				float specularIntensitySmooth2 = smoothstep(_SpecularSmooth, _SpecularSmoothUpper, specularIntensity2);
				float4 specular2 = specularIntensitySmooth2 * _SpecularColor2;

				float specularIntensity3 = pow(NdotH * lightIntensity, _GlossinessThird * _GlossinessThird);
				float specularIntensitySmooth3 = smoothstep(_SpecularSmooth, _SpecularSmoothUpper, specularIntensity3);
				float4 specular3 = specularIntensitySmooth3 * _SpecularColor3;
				
				//add rim highlights
				float4 rimDot = 1 - dot(viewDir, normal);
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				//add rim edge highlights
				float rimEdgeIntensity = rimDot * pow(NdotL, _RimEdgeThreshold);
				rimEdgeIntensity = smoothstep(_RimEdgeAmount - 0.01, _RimEdgeAmount + 0.01, rimEdgeIntensity);
				if(_RimEdgeTotal == 1) {
					rimEdgeIntensity = smoothstep(_RimEdgeAmount - 0.01, _RimEdgeAmount + 0.01, rimDot);
				}				
				float4 rimEdge = rimEdgeIntensity * _RimEdgeColor;

				//aquire surface base colour
				float4 sample = tex2D(_MainTex, i.uv);

				//return surface colour
				float4 lightEffects = (_AmbientColor + light + light2 + specular + specular2 - 10*specular3 + rim - 10*rimEdge);
				return _Color * sample * lightEffects;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
