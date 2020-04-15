Shader "Unlit Master"
{
	Properties
	{
		[HDR]Color_F1591722("HighLightColor", Color) = (0, 0, 0, 0)
		Vector1_48CA5DF4("Height", Float) = 0.45
		Vector1_5956BAA2("Width", Float) = 0.45
		Vector2_412F2170("Rectangle Dimensions", Vector) = (0.95, 0.952, 0, 0)
		[ToggleUI]Boolean_97C11442("UseCorner", Float) = 1
		Vector1_16AE01E2("CornerSlider", Range(1, 8)) = 1
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "HDRenderPipeline"
			"RenderType" = "Transparent"
			"Queue" = "Transparent+0"
		}

		Pass
		{
			Name "Pass"
			Tags
			{
		// LightMode: <None>
	}

	// Render State
	Blend One One, One One
	Cull Back
	ZWrite Off
	ZTest Always
		// ColorMask: <None>


		HLSLPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x
		#pragma target 2.0
		#pragma multi_compile_instancing

		// Keywords
		#pragma multi_compile _ LIGHTMAP_ON
		#pragma multi_compile _ DIRLIGHTMAP_COMBINED
		#pragma shader_feature _ _SAMPLE_GI
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _BLENDMODE_ADD 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define VARYINGS_NEED_POSITION_WS 
		#define VARYINGS_NEED_TEXCOORD0
		#define SHADERPASS_UNLIT

		// Includes
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float4 Color_F1591722;
		float Vector1_48CA5DF4;
		float Vector1_5956BAA2;
		float2 Vector2_412F2170;
		float Boolean_97C11442;
		float Vector1_16AE01E2;
		CBUFFER_END
		float Vector1_E8723F1B;
		float Vector1_D49A0B3B;
		float Vector1_3C76144B;
		float Vector1_D8765B00;
		float Vector1_587036E;
		float Vector1_181C21C5;
		float Vector1_792DF504;
		float Vector1_B3865456;

		// Graph Functions

		void Unity_Rectangle_float(float2 UV, float Width, float Height, out float Out)
		{
			float2 d = abs(UV * 2 - 1) - float2(Width, Height);
			d = 1 - d / fwidth(d);
			Out = saturate(min(d.x, d.y));
		}

		void Unity_RoundedRectangle_float(float2 UV, float Width, float Height, float Radius, out float Out)
		{
			Radius = max(min(min(abs(Radius * 2), abs(Width)), abs(Height)), 1e-5);
			float2 uv = abs(UV * 2 - 1) - float2(Width, Height) + Radius;
			float d = length(max(0, uv)) / Radius;
			Out = saturate((1 - d) / fwidth(d));
		}

		void Unity_Subtract_float(float A, float B, out float Out)
		{
			Out = A - B;
		}

		void Unity_OneMinus_float(float In, out float Out)
		{
			Out = 1 - In;
		}

		void Unity_Multiply_float(float A, float B, out float Out)
		{
			Out = A * B;
		}

		void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
		{
			Out = UV * Tiling + Offset;
		}

		void Unity_Step_float(float Edge, float In, out float Out)
		{
			Out = step(Edge, In);
		}

		void Matcher_float(float Slider, float In1, float In2, float In3, float In4, float In5, float In6, float In7, float In8, out float Out)
		{
			if (Slider == 1) { Out = In1; }
			if (Slider == 2) { Out = In2; }
			if (Slider == 3) { Out = In3; }
			if (Slider == 4) { Out = In4; }
			if (Slider == 5) { Out = In5; }
			if (Slider == 6) { Out = In6; }
			if (Slider == 7) { Out = In7; }
			if (Slider == 8) { Out = In8; }



		}

		void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
		{
			Out = A * B;
		}

		void Unity_And_float(float A, float B, out float Out)
		{
			Out = A && B;
		}

		void Unity_Branch_float(float Predicate, float True, float False, out float Out)
		{
			Out = lerp(False, True, Predicate);
		}

		// Graph Vertex
		// GraphVertex: <None>

		// Graph Pixel
		struct SurfaceDescriptionInputs
		{
			float3 ObjectSpacePosition;
			float4 uv0;
		};

		struct SurfaceDescription
		{
			float3 Color;
			float Alpha;
			float AlphaClipThreshold;
		};

		SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
		{
			SurfaceDescription surface = (SurfaceDescription)0;
			float4 _Property_2563D0F8_Out_0 = Color_F1591722;
			float _Property_B05FCA1E_Out_0 = Vector1_16AE01E2;
			float _Rectangle_D5C0ED10_Out_3;
			Unity_Rectangle_float(IN.uv0.xy, 1, 1, _Rectangle_D5C0ED10_Out_3);
			float2 _Property_D95451B1_Out_0 = Vector2_412F2170;
			float _Split_735273A7_R_1 = _Property_D95451B1_Out_0[0];
			float _Split_735273A7_G_2 = _Property_D95451B1_Out_0[1];
			float _Split_735273A7_B_3 = 0;
			float _Split_735273A7_A_4 = 0;
			float _RoundedRectangle_C030A4DC_Out_4;
			Unity_RoundedRectangle_float(IN.uv0.xy, _Split_735273A7_R_1, _Split_735273A7_G_2, 0.1, _RoundedRectangle_C030A4DC_Out_4);
			float _Subtract_CF71609A_Out_2;
			Unity_Subtract_float(_Rectangle_D5C0ED10_Out_3, _RoundedRectangle_C030A4DC_Out_4, _Subtract_CF71609A_Out_2);
			float _Property_E36AA62E_Out_0 = Vector1_48CA5DF4;
			float _Rectangle_4628FC9F_Out_3;
			Unity_Rectangle_float(IN.uv0.xy, 1.14, _Property_E36AA62E_Out_0, _Rectangle_4628FC9F_Out_3);
			float _OneMinus_E1E52B90_Out_1;
			Unity_OneMinus_float(_Rectangle_4628FC9F_Out_3, _OneMinus_E1E52B90_Out_1);
			float _Multiply_7B4493B4_Out_2;
			Unity_Multiply_float(_Subtract_CF71609A_Out_2, _OneMinus_E1E52B90_Out_1, _Multiply_7B4493B4_Out_2);
			float _Property_439A916F_Out_0 = Vector1_5956BAA2;
			float _Rectangle_6DF7974F_Out_3;
			Unity_Rectangle_float(IN.uv0.xy, _Property_439A916F_Out_0, 1.44, _Rectangle_6DF7974F_Out_3);
			float _OneMinus_E62F1B47_Out_1;
			Unity_OneMinus_float(_Rectangle_6DF7974F_Out_3, _OneMinus_E62F1B47_Out_1);
			float _Multiply_5DBBCA5_Out_2;
			Unity_Multiply_float(_Multiply_7B4493B4_Out_2, _OneMinus_E62F1B47_Out_1, _Multiply_5DBBCA5_Out_2);
			float2 _TilingAndOffset_47237646_Out_3;
			Unity_TilingAndOffset_float((IN.ObjectSpacePosition.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_47237646_Out_3);
			float _Split_2322A2AF_R_1 = _TilingAndOffset_47237646_Out_3[0];
			float _Split_2322A2AF_G_2 = _TilingAndOffset_47237646_Out_3[1];
			float _Split_2322A2AF_B_3 = 0;
			float _Split_2322A2AF_A_4 = 0;
			float _Step_35C8FE2_Out_2;
			Unity_Step_float(0.15, _Split_2322A2AF_G_2, _Step_35C8FE2_Out_2);
			float _Multiply_B049C982_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _Step_35C8FE2_Out_2, _Multiply_B049C982_Out_2);
			float _Split_F2B2CEBF_R_1 = _TilingAndOffset_47237646_Out_3[0];
			float _Split_F2B2CEBF_G_2 = _TilingAndOffset_47237646_Out_3[1];
			float _Split_F2B2CEBF_B_3 = 0;
			float _Split_F2B2CEBF_A_4 = 0;
			float _Step_29BBE241_Out_2;
			Unity_Step_float(0.15, _Split_F2B2CEBF_R_1, _Step_29BBE241_Out_2);
			float _Multiply_9ACDE70C_Out_2;
			Unity_Multiply_float(_Multiply_B049C982_Out_2, _Step_29BBE241_Out_2, _Multiply_9ACDE70C_Out_2);
			float _Split_E5E8559D_R_1 = IN.ObjectSpacePosition[0];
			float _Split_E5E8559D_G_2 = IN.ObjectSpacePosition[1];
			float _Split_E5E8559D_B_3 = IN.ObjectSpacePosition[2];
			float _Split_E5E8559D_A_4 = 0;
			float _Step_2AA668BE_Out_2;
			Unity_Step_float(0.15, _Split_E5E8559D_B_3, _Step_2AA668BE_Out_2);
			float _Multiply_8EEF6B30_Out_2;
			Unity_Multiply_float(_Multiply_9ACDE70C_Out_2, _Step_2AA668BE_Out_2, _Multiply_8EEF6B30_Out_2);
			float _Multiply_12099DDE_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _Step_35C8FE2_Out_2, _Multiply_12099DDE_Out_2);
			float _OneMinus_1C8C922F_Out_1;
			Unity_OneMinus_float(_Step_29BBE241_Out_2, _OneMinus_1C8C922F_Out_1);
			float _Multiply_E050188D_Out_2;
			Unity_Multiply_float(_Multiply_12099DDE_Out_2, _OneMinus_1C8C922F_Out_1, _Multiply_E050188D_Out_2);
			float _Multiply_EDEC5652_Out_2;
			Unity_Multiply_float(_Multiply_E050188D_Out_2, _Step_2AA668BE_Out_2, _Multiply_EDEC5652_Out_2);
			float _Multiply_36692CE_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _Step_35C8FE2_Out_2, _Multiply_36692CE_Out_2);
			float _Multiply_3D7D8FCB_Out_2;
			Unity_Multiply_float(_Multiply_36692CE_Out_2, _Step_29BBE241_Out_2, _Multiply_3D7D8FCB_Out_2);
			float _OneMinus_4B3DB447_Out_1;
			Unity_OneMinus_float(_Step_2AA668BE_Out_2, _OneMinus_4B3DB447_Out_1);
			float _Multiply_E6854EA2_Out_2;
			Unity_Multiply_float(_Multiply_3D7D8FCB_Out_2, _OneMinus_4B3DB447_Out_1, _Multiply_E6854EA2_Out_2);
			float _Multiply_4D9FCDC_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _Step_35C8FE2_Out_2, _Multiply_4D9FCDC_Out_2);
			float _Multiply_6DD10D5D_Out_2;
			Unity_Multiply_float(_Multiply_4D9FCDC_Out_2, _OneMinus_1C8C922F_Out_1, _Multiply_6DD10D5D_Out_2);
			float _Multiply_EE3F8269_Out_2;
			Unity_Multiply_float(_Multiply_6DD10D5D_Out_2, _OneMinus_4B3DB447_Out_1, _Multiply_EE3F8269_Out_2);
			float _OneMinus_A855C13B_Out_1;
			Unity_OneMinus_float(_Step_35C8FE2_Out_2, _OneMinus_A855C13B_Out_1);
			float _Multiply_7624822A_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _OneMinus_A855C13B_Out_1, _Multiply_7624822A_Out_2);
			float _Multiply_553968F_Out_2;
			Unity_Multiply_float(_Multiply_7624822A_Out_2, _Step_29BBE241_Out_2, _Multiply_553968F_Out_2);
			float _Multiply_C963C657_Out_2;
			Unity_Multiply_float(_Multiply_553968F_Out_2, _Step_2AA668BE_Out_2, _Multiply_C963C657_Out_2);
			float _Multiply_71C51696_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _OneMinus_A855C13B_Out_1, _Multiply_71C51696_Out_2);
			float _Multiply_92E2A7F1_Out_2;
			Unity_Multiply_float(_Multiply_71C51696_Out_2, _OneMinus_1C8C922F_Out_1, _Multiply_92E2A7F1_Out_2);
			float _Multiply_EF9FFD28_Out_2;
			Unity_Multiply_float(_Multiply_92E2A7F1_Out_2, _Step_2AA668BE_Out_2, _Multiply_EF9FFD28_Out_2);
			float _Multiply_7B71C069_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _OneMinus_A855C13B_Out_1, _Multiply_7B71C069_Out_2);
			float _Multiply_B6DBC8E8_Out_2;
			Unity_Multiply_float(_Multiply_7B71C069_Out_2, _Step_29BBE241_Out_2, _Multiply_B6DBC8E8_Out_2);
			float _Multiply_5F1B5EDA_Out_2;
			Unity_Multiply_float(_Multiply_B6DBC8E8_Out_2, _OneMinus_4B3DB447_Out_1, _Multiply_5F1B5EDA_Out_2);
			float _Multiply_20D918FE_Out_2;
			Unity_Multiply_float(_Multiply_5DBBCA5_Out_2, _OneMinus_A855C13B_Out_1, _Multiply_20D918FE_Out_2);
			float _Multiply_DD36CAAC_Out_2;
			Unity_Multiply_float(_Multiply_20D918FE_Out_2, _OneMinus_1C8C922F_Out_1, _Multiply_DD36CAAC_Out_2);
			float _Multiply_48EB94F3_Out_2;
			Unity_Multiply_float(_Multiply_DD36CAAC_Out_2, _OneMinus_4B3DB447_Out_1, _Multiply_48EB94F3_Out_2);
			float _CustomFunction_FE369D7E_Out_0;
			Matcher_float(_Property_B05FCA1E_Out_0, _Multiply_8EEF6B30_Out_2, _Multiply_EDEC5652_Out_2, _Multiply_E6854EA2_Out_2, _Multiply_EE3F8269_Out_2, _Multiply_C963C657_Out_2, _Multiply_EF9FFD28_Out_2, _Multiply_5F1B5EDA_Out_2, _Multiply_48EB94F3_Out_2, _CustomFunction_FE369D7E_Out_0);
			float4 _Multiply_49980D96_Out_2;
			Unity_Multiply_float(_Property_2563D0F8_Out_0, (_CustomFunction_FE369D7E_Out_0.xxxx), _Multiply_49980D96_Out_2);
			float _Property_9DBC8307_Out_0 = Boolean_97C11442;
			float Boolean_9566F2B9 = 1;
			float _And_1D6DED6A_Out_2;
			Unity_And_float(_Property_9DBC8307_Out_0, Boolean_9566F2B9, _And_1D6DED6A_Out_2);
			float _Branch_4B23A897_Out_3;
			Unity_Branch_float(_And_1D6DED6A_Out_2, 1, 0, _Branch_4B23A897_Out_3);
			float4 _Multiply_D6317CE2_Out_2;
			Unity_Multiply_float(_Multiply_49980D96_Out_2, (_Branch_4B23A897_Out_3.xxxx), _Multiply_D6317CE2_Out_2);
			surface.Color = (_Multiply_D6317CE2_Out_2.xyz);
			surface.Alpha = 1;
			surface.AlphaClipThreshold = 0.5;
			return surface;
		}

		// --------------------------------------------------
		// Structs and Packing

		// Generated Type: Attributes
		struct Attributes
		{
			float3 positionOS : POSITION;
			float3 normalOS : NORMAL;
			float4 tangentOS : TANGENT;
			float4 uv0 : TEXCOORD0;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : INSTANCEID_SEMANTIC;
			#endif
		};

		// Generated Type: Varyings
		struct Varyings
		{
			float4 positionCS : SV_Position;
			float3 positionWS;
			float4 texCoord0;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
		};

		// Generated Type: PackedVaryings
		struct PackedVaryings
		{
			float4 positionCS : SV_Position;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			float3 interp00 : TEXCOORD0;
			float4 interp01 : TEXCOORD1;
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};

		// Packed Type: Varyings
		PackedVaryings PackVaryings(Varyings input)
		{
			PackedVaryings output;
			output.positionCS = input.positionCS;
			output.interp00.xyz = input.positionWS;
			output.interp01.xyzw = input.texCoord0;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			return output;
		}

		// Unpacked Type: Varyings
		Varyings UnpackVaryings(PackedVaryings input)
		{
			Varyings output;
			output.positionCS = input.positionCS;
			output.positionWS = input.interp00.xyz;
			output.texCoord0 = input.interp01.xyzw;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			return output;
		}

		// --------------------------------------------------
		// Build Graph Inputs

		SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
		{
			SurfaceDescriptionInputs output;
			ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

			output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
			output.uv0 = input.texCoord0;
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
		#else
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
		#endif
		#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

			return output;
		}


		// --------------------------------------------------
		// Main

		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

		ENDHLSL
	}

	Pass
	{
		Name "ShadowCaster"
		Tags
		{
			"LightMode" = "ShadowCaster"
		}

			// Render State
			Blend One One, One One
			Cull Back
			ZTest LEqual
			ZWrite On
			// ColorMask: <None>


			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			// Pragmas
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma multi_compile_instancing

			// Keywords
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			// GraphKeywords: <None>

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _AlphaClip 1
			#define _BLENDMODE_ADD 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS_SHADOWCASTER

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 Color_F1591722;
			float Vector1_48CA5DF4;
			float Vector1_5956BAA2;
			float2 Vector2_412F2170;
			float Boolean_97C11442;
			float Vector1_16AE01E2;
			CBUFFER_END
			float Vector1_E8723F1B;
			float Vector1_D49A0B3B;
			float Vector1_3C76144B;
			float Vector1_D8765B00;
			float Vector1_587036E;
			float Vector1_181C21C5;
			float Vector1_792DF504;
			float Vector1_B3865456;

			// Graph Functions
			// GraphFunctions: <None>

			// Graph Vertex
			// GraphVertex: <None>

			// Graph Pixel
			struct SurfaceDescriptionInputs
			{
			};

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				surface.Alpha = 1;
				surface.AlphaClipThreshold = 0.5;
				return surface;
			}

			// --------------------------------------------------
			// Structs and Packing

			// Generated Type: Attributes
			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			// Generated Type: Varyings
			struct Varyings
			{
				float4 positionCS : SV_Position;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
			};

			// Generated Type: PackedVaryings
			struct PackedVaryings
			{
				float4 positionCS : SV_Position;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			// Packed Type: Varyings
			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// Unpacked Type: Varyings
			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}


			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

				// Render State
				Blend One One, One One
				Cull Back
				ZTest LEqual
				ZWrite On
				ColorMask 0


				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// Debug
				// <None>

				// --------------------------------------------------
				// Pass

				// Pragmas
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0
				#pragma multi_compile_instancing

				// Keywords
				// PassKeywords: <None>
				// GraphKeywords: <None>

				// Defines
				#define _SURFACE_TYPE_TRANSPARENT 1
				#define _AlphaClip 1
				#define _BLENDMODE_ADD 1
				#define ATTRIBUTES_NEED_NORMAL
				#define ATTRIBUTES_NEED_TANGENT
				#define SHADERPASS_DEPTHONLY

				// Includes
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

				// --------------------------------------------------
				// Graph

				// Graph Properties
				CBUFFER_START(UnityPerMaterial)
				float4 Color_F1591722;
				float Vector1_48CA5DF4;
				float Vector1_5956BAA2;
				float2 Vector2_412F2170;
				float Boolean_97C11442;
				float Vector1_16AE01E2;
				CBUFFER_END
				float Vector1_E8723F1B;
				float Vector1_D49A0B3B;
				float Vector1_3C76144B;
				float Vector1_D8765B00;
				float Vector1_587036E;
				float Vector1_181C21C5;
				float Vector1_792DF504;
				float Vector1_B3865456;

				// Graph Functions
				// GraphFunctions: <None>

				// Graph Vertex
				// GraphVertex: <None>

				// Graph Pixel
				struct SurfaceDescriptionInputs
				{
				};

				struct SurfaceDescription
				{
					float Alpha;
					float AlphaClipThreshold;
				};

				SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
				{
					SurfaceDescription surface = (SurfaceDescription)0;
					surface.Alpha = 1;
					surface.AlphaClipThreshold = 0.5;
					return surface;
				}

				// --------------------------------------------------
				// Structs and Packing

				// Generated Type: Attributes
				struct Attributes
				{
					float3 positionOS : POSITION;
					float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				};

				// Generated Type: Varyings
				struct Varyings
				{
					float4 positionCS : SV_Position;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				};

				// Generated Type: PackedVaryings
				struct PackedVaryings
				{
					float4 positionCS : SV_Position;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				// Packed Type: Varyings
				PackedVaryings PackVaryings(Varyings input)
				{
					PackedVaryings output;
					output.positionCS = input.positionCS;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// Unpacked Type: Varyings
				Varyings UnpackVaryings(PackedVaryings input)
				{
					Varyings output;
					output.positionCS = input.positionCS;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// --------------------------------------------------
				// Build Graph Inputs

				SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
				{
					SurfaceDescriptionInputs output;
					ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
				}


				// --------------------------------------------------
				// Main

				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

				ENDHLSL
			}

	}
		FallBack "Hidden/Shader Graph/FallbackError"
}
