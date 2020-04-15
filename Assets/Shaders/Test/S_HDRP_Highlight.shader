Shader "Custom/HDRP Highlight"
{
    Properties
    {
        [HDR] Color_F1591722("HighLightColor", Color) = (0, 0, 0, 0)
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
            "RenderType" = "HDUnlitShader"
            "Queue" = "Transparent+0"
        }

        Pass
        {
        // based on UnlitPass.template
        Name "ShadowCaster"
        Tags { "LightMode" = "ShadowCaster" }

        //-------------------------------------------------------------------------------------
        // Render Modes (Blend, Cull, ZTest, Stencil, etc)
        //-------------------------------------------------------------------------------------


        
        Cull Off
        ZWrite Off
        ZTest Always


        //ColorMask 0

        //-------------------------------------------------------------------------------------
        // End Render Modes
        //-------------------------------------------------------------------------------------

        HLSLPROGRAM

        #pragma target 4.5
        #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
        //#pragma enable_d3d11_debug_symbols

        //enable GPU instancing support
        #pragma multi_compile_instancing

        //-------------------------------------------------------------------------------------
        // Variant Definitions (active field translations to HDRP defines)
        //-------------------------------------------------------------------------------------
        #define _SURFACE_TYPE_TRANSPARENT 1
        // #define _BLENDMODE_ALPHA 1
        #define _BLENDMODE_ADD 1
        // #define _BLENDMODE_PRE_MULTIPLY 1
        // #define _ADD_PRECOMPUTED_VELOCITY

        //-------------------------------------------------------------------------------------
        // End Variant Definitions
        //-------------------------------------------------------------------------------------

        #pragma vertex Vert
        #pragma fragment Frag

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

        //-------------------------------------------------------------------------------------
        // Defines
        //-------------------------------------------------------------------------------------
                #define SHADERPASS SHADERPASS_SHADOWS
            // ACTIVE FIELDS:
            //   AlphaTest
            //   SurfaceType.Transparent
            //   BlendMode.Add
            //   VertexDescriptionInputs.ObjectSpaceNormal
            //   VertexDescriptionInputs.ObjectSpaceTangent
            //   VertexDescriptionInputs.ObjectSpacePosition
            //   SurfaceDescription.Alpha
            //   SurfaceDescription.AlphaClipThreshold
            //   AttributesMesh.normalOS
            //   AttributesMesh.tangentOS
            //   AttributesMesh.positionOS
            // Shared Graph Keywords

        // this translates the new dependency tracker into the old preprocessor definitions for the existing HDRP shader code
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        // #define ATTRIBUTES_NEED_TEXCOORD0
        // #define ATTRIBUTES_NEED_TEXCOORD1
        // #define ATTRIBUTES_NEED_TEXCOORD2
        // #define ATTRIBUTES_NEED_TEXCOORD3
        // #define ATTRIBUTES_NEED_COLOR
        // #define VARYINGS_NEED_POSITION_WS
        // #define VARYINGS_NEED_TANGENT_TO_WORLD
        // #define VARYINGS_NEED_TEXCOORD0
        // #define VARYINGS_NEED_TEXCOORD1
        // #define VARYINGS_NEED_TEXCOORD2
        // #define VARYINGS_NEED_TEXCOORD3
        // #define VARYINGS_NEED_COLOR
        // #define VARYINGS_NEED_CULLFACE
        // #define HAVE_MESH_MODIFICATION

        //-------------------------------------------------------------------------------------
        // End Defines
        //-------------------------------------------------------------------------------------


        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"

        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // Used by SceneSelectionPass
        int _ObjectId;
        int _PassValue;

        //-------------------------------------------------------------------------------------
        // Interpolator Packing And Struct Declarations
        //-------------------------------------------------------------------------------------
        // Generated Type: AttributesMesh
        struct AttributesMesh
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL; // optional
            float4 tangentOS : TANGENT; // optional
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif // UNITY_ANY_INSTANCING_ENABLED
        };
        // Generated Type: VaryingsMeshToPS
        struct VaryingsMeshToPS
        {
            float4 positionCS : SV_Position;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif // UNITY_ANY_INSTANCING_ENABLED
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif // defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        };

        // Generated Type: PackedVaryingsMeshToPS
        struct PackedVaryingsMeshToPS
        {
            float4 positionCS : SV_Position; // unpacked
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
            #endif // conditional
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC; // unpacked
            #endif // conditional
        };

        // Packed Type: VaryingsMeshToPS
        PackedVaryingsMeshToPS PackVaryingsMeshToPS(VaryingsMeshToPS input)
        {
            PackedVaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif // conditional
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif // conditional
            return output;
        }

        // Unpacked Type: VaryingsMeshToPS
        VaryingsMeshToPS UnpackVaryingsMeshToPS(PackedVaryingsMeshToPS input)
        {
            VaryingsMeshToPS output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif // conditional
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif // conditional
            return output;
        }
        // Generated Type: VaryingsMeshToDS
        struct VaryingsMeshToDS
        {
            float3 positionRWS;
            float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif // UNITY_ANY_INSTANCING_ENABLED
        };

        // Generated Type: PackedVaryingsMeshToDS
        struct PackedVaryingsMeshToDS
        {
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
            #endif // conditional
            float3 interp00 : TEXCOORD0; // auto-packed
            float3 interp01 : TEXCOORD1; // auto-packed
        };

        // Packed Type: VaryingsMeshToDS
        PackedVaryingsMeshToDS PackVaryingsMeshToDS(VaryingsMeshToDS input)
        {
            PackedVaryingsMeshToDS output;
            output.interp00.xyz = input.positionRWS;
            output.interp01.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif // conditional
            return output;
        }

        // Unpacked Type: VaryingsMeshToDS
        VaryingsMeshToDS UnpackVaryingsMeshToDS(PackedVaryingsMeshToDS input)
        {
            VaryingsMeshToDS output;
            output.positionRWS = input.interp00.xyz;
            output.normalWS = input.interp01.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif // conditional
            return output;
        }
        //-------------------------------------------------------------------------------------
        // End Interpolator Packing And Struct Declarations
        //-------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------
        // Graph generated code
        //-------------------------------------------------------------------------------------
                // Shared Graph Properties (uniform inputs)
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

                // Pixel Graph Inputs
                    struct SurfaceDescriptionInputs
                    {
                    };
                    // Pixel Graph Outputs
                        struct SurfaceDescription
                        {
                            float Alpha;
                            float AlphaClipThreshold;
                        };

                        // Shared Graph Node Functions
                        // Pixel Graph Evaluation
                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                surface.Alpha = 1;
                                surface.AlphaClipThreshold = 0.5;
                                return surface;
                            }

                            //-------------------------------------------------------------------------------------
                            // End graph generated code
                            //-------------------------------------------------------------------------------------

                        // $include("VertexAnimation.template.hlsl")

                        //-------------------------------------------------------------------------------------
                            // TEMPLATE INCLUDE : SharedCode.template.hlsl
                            //-------------------------------------------------------------------------------------

                                FragInputs BuildFragInputs(VaryingsMeshToPS input)
                                {
                                    FragInputs output;
                                    ZERO_INITIALIZE(FragInputs, output);

                                    // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
                                    // TODO: this is a really poor workaround, but the variable is used in a bunch of places
                                    // to compute normals which are then passed on elsewhere to compute other values...
                                    output.tangentToWorld = k_identity3x3;
                                    output.positionSS = input.positionCS;       // input.positionCS is SV_Position

                                    // output.positionRWS = input.positionRWS;
                                    // output.tangentToWorld = BuildTangentToWorld(input.tangentWS, input.normalWS);
                                    // output.texCoord0 = input.texCoord0;
                                    // output.texCoord1 = input.texCoord1;
                                    // output.texCoord2 = input.texCoord2;
                                    // output.texCoord3 = input.texCoord3;
                                    // output.color = input.color;
                                    #if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
                                    output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                    #elif SHADER_STAGE_FRAGMENT
                                    // output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                    #endif // SHADER_STAGE_FRAGMENT

                                    return output;
                                }

                                SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
                                {
                                    SurfaceDescriptionInputs output;
                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    // output.WorldSpaceNormal =            normalize(input.tangentToWorld[2].xyz);
                                    // output.ObjectSpaceNormal =           mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M);           // transposed multiplication by inverse matrix to handle normal scale
                                    // output.ViewSpaceNormal =             mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
                                    // output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                                    // output.WorldSpaceTangent =           input.tangentToWorld[0].xyz;
                                    // output.ObjectSpaceTangent =          TransformWorldToObjectDir(output.WorldSpaceTangent);
                                    // output.ViewSpaceTangent =            TransformWorldToViewDir(output.WorldSpaceTangent);
                                    // output.TangentSpaceTangent =         float3(1.0f, 0.0f, 0.0f);
                                    // output.WorldSpaceBiTangent =         input.tangentToWorld[1].xyz;
                                    // output.ObjectSpaceBiTangent =        TransformWorldToObjectDir(output.WorldSpaceBiTangent);
                                    // output.ViewSpaceBiTangent =          TransformWorldToViewDir(output.WorldSpaceBiTangent);
                                    // output.TangentSpaceBiTangent =       float3(0.0f, 1.0f, 0.0f);
                                    // output.WorldSpaceViewDirection =     normalize(viewWS);
                                    // output.ObjectSpaceViewDirection =    TransformWorldToObjectDir(output.WorldSpaceViewDirection);
                                    // output.ViewSpaceViewDirection =      TransformWorldToViewDir(output.WorldSpaceViewDirection);
                                    // float3x3 tangentSpaceTransform =     float3x3(output.WorldSpaceTangent,output.WorldSpaceBiTangent,output.WorldSpaceNormal);
                                    // output.TangentSpaceViewDirection =   mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
                                    // output.WorldSpacePosition =          input.positionRWS;
                                    // output.ObjectSpacePosition =         TransformWorldToObject(input.positionRWS);
                                    // output.ViewSpacePosition =           TransformWorldToView(input.positionRWS);
                                    // output.TangentSpacePosition =        float3(0.0f, 0.0f, 0.0f);
                                    // output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionRWS);
                                    // output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionRWS), _ProjectionParams.x);
                                    // output.uv0 =                         input.texCoord0;
                                    // output.uv1 =                         input.texCoord1;
                                    // output.uv2 =                         input.texCoord2;
                                    // output.uv3 =                         input.texCoord3;
                                    // output.VertexColor =                 input.color;
                                    // output.FaceSign =                    input.isFrontFace;
                                    // output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value

                                    return output;
                                }

                                // existing HDRP code uses the combined function to go directly from packed to frag inputs
                                FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
                                {
                                    UNITY_SETUP_INSTANCE_ID(input);
                                    VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
                                    return BuildFragInputs(unpacked);
                                }

                                //-------------------------------------------------------------------------------------
                                // END TEMPLATE INCLUDE : SharedCode.template.hlsl
                                //-------------------------------------------------------------------------------------



                                void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
                                {
                                    // setup defaults -- these are used if the graph doesn't output a value
                                    ZERO_INITIALIZE(SurfaceData, surfaceData);

                                    // copy across graph values, if defined
                                    // surfaceData.color = surfaceDescription.Color;

                            #if defined(DEBUG_DISPLAY)
                                    if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                                    {
                                        // TODO
                                    }
                            #endif
                                }

                                void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
                                {
                                    SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
                                    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

                                    // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                                    // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                                    DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);

                                    BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);

                                    // Builtin Data
                                    ZERO_INITIALIZE(BuiltinData, builtinData); // No call to InitBuiltinData as we don't have any lighting
                                    builtinData.opacity = surfaceDescription.Alpha;
                                }

                                //-------------------------------------------------------------------------------------
                                // Pass Includes
                                //-------------------------------------------------------------------------------------
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
                                //-------------------------------------------------------------------------------------
                                // End Pass Includes
                                //-------------------------------------------------------------------------------------

                                ENDHLSL
                            }

                            Pass
                            {
                                    // based on UnlitPass.template
                                    Name "META"
                                    Tags { "LightMode" = "META" }

                                    //-------------------------------------------------------------------------------------
                                    // Render Modes (Blend, Cull, ZTest, Stencil, etc)
                                    //-------------------------------------------------------------------------------------

                                    Cull Off
                                    ZWrite Off
                                    ZTest Always





                                    //-------------------------------------------------------------------------------------
                                    // End Render Modes
                                    //-------------------------------------------------------------------------------------

                                    HLSLPROGRAM

                                    #pragma target 4.5
                                    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
                                    //#pragma enable_d3d11_debug_symbols

                                    //enable GPU instancing support
                                    #pragma multi_compile_instancing

                                    //-------------------------------------------------------------------------------------
                                    // Variant Definitions (active field translations to HDRP defines)
                                    //-------------------------------------------------------------------------------------
                                    #define _SURFACE_TYPE_TRANSPARENT 1
                                    // #define _BLENDMODE_ALPHA 1
                                    #define _BLENDMODE_ADD 1
                                    // #define _BLENDMODE_PRE_MULTIPLY 1
                                    // #define _ADD_PRECOMPUTED_VELOCITY

                                    //-------------------------------------------------------------------------------------
                                    // End Variant Definitions
                                    //-------------------------------------------------------------------------------------

                                    #pragma vertex Vert
                                    #pragma fragment Frag

                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

                                    //-------------------------------------------------------------------------------------
                                    // Defines
                                    //-------------------------------------------------------------------------------------
                                            #define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
                                        // ACTIVE FIELDS:
                                        //   AlphaTest
                                        //   SurfaceType.Transparent
                                        //   BlendMode.Add
                                        //   SurfaceDescriptionInputs.ObjectSpacePosition
                                        //   SurfaceDescriptionInputs.uv0
                                        //   VertexDescriptionInputs.ObjectSpaceNormal
                                        //   VertexDescriptionInputs.ObjectSpaceTangent
                                        //   VertexDescriptionInputs.ObjectSpacePosition
                                        //   SurfaceDescription.Color
                                        //   SurfaceDescription.Alpha
                                        //   SurfaceDescription.AlphaClipThreshold
                                        //   AttributesMesh.normalOS
                                        //   AttributesMesh.tangentOS
                                        //   AttributesMesh.uv0
                                        //   AttributesMesh.uv1
                                        //   AttributesMesh.color
                                        //   AttributesMesh.uv2
                                        //   FragInputs.positionRWS
                                        //   FragInputs.texCoord0
                                        //   AttributesMesh.positionOS
                                        //   VaryingsMeshToPS.positionRWS
                                        //   VaryingsMeshToPS.texCoord0
                                        // Shared Graph Keywords

                                    // this translates the new dependency tracker into the old preprocessor definitions for the existing HDRP shader code
                                    #define ATTRIBUTES_NEED_NORMAL
                                    #define ATTRIBUTES_NEED_TANGENT
                                    #define ATTRIBUTES_NEED_TEXCOORD0
                                    #define ATTRIBUTES_NEED_TEXCOORD1
                                    #define ATTRIBUTES_NEED_TEXCOORD2
                                    // #define ATTRIBUTES_NEED_TEXCOORD3
                                    #define ATTRIBUTES_NEED_COLOR
                                    #define VARYINGS_NEED_POSITION_WS
                                    // #define VARYINGS_NEED_TANGENT_TO_WORLD
                                    #define VARYINGS_NEED_TEXCOORD0
                                    // #define VARYINGS_NEED_TEXCOORD1
                                    // #define VARYINGS_NEED_TEXCOORD2
                                    // #define VARYINGS_NEED_TEXCOORD3
                                    // #define VARYINGS_NEED_COLOR
                                    // #define VARYINGS_NEED_CULLFACE
                                    // #define HAVE_MESH_MODIFICATION

                                    //-------------------------------------------------------------------------------------
                                    // End Defines
                                    //-------------------------------------------------------------------------------------


                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"

                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                    // Used by SceneSelectionPass
                                    int _ObjectId;
                                    int _PassValue;

                                    //-------------------------------------------------------------------------------------
                                    // Interpolator Packing And Struct Declarations
                                    //-------------------------------------------------------------------------------------
                                    // Generated Type: AttributesMesh
                                    struct AttributesMesh
                                    {
                                        float3 positionOS : POSITION;
                                        float3 normalOS : NORMAL; // optional
                                        float4 tangentOS : TANGENT; // optional
                                        float4 uv0 : TEXCOORD0; // optional
                                        float4 uv1 : TEXCOORD1; // optional
                                        float4 uv2 : TEXCOORD2; // optional
                                        float4 color : COLOR; // optional
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        uint instanceID : INSTANCEID_SEMANTIC;
                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                    };
                                    // Generated Type: VaryingsMeshToPS
                                    struct VaryingsMeshToPS
                                    {
                                        float4 positionCS : SV_Position;
                                        float3 positionRWS; // optional
                                        float4 texCoord0; // optional
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                        #endif // defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    };

                                    // Generated Type: PackedVaryingsMeshToPS
                                    struct PackedVaryingsMeshToPS
                                    {
                                        float4 positionCS : SV_Position; // unpacked
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                        #endif // conditional
                                        float3 interp00 : TEXCOORD0; // auto-packed
                                        float4 interp01 : TEXCOORD1; // auto-packed
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC; // unpacked
                                        #endif // conditional
                                    };

                                    // Packed Type: VaryingsMeshToPS
                                    PackedVaryingsMeshToPS PackVaryingsMeshToPS(VaryingsMeshToPS input)
                                    {
                                        PackedVaryingsMeshToPS output;
                                        output.positionCS = input.positionCS;
                                        output.interp00.xyz = input.positionRWS;
                                        output.interp01.xyzw = input.texCoord0;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif // conditional
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif // conditional
                                        return output;
                                    }

                                    // Unpacked Type: VaryingsMeshToPS
                                    VaryingsMeshToPS UnpackVaryingsMeshToPS(PackedVaryingsMeshToPS input)
                                    {
                                        VaryingsMeshToPS output;
                                        output.positionCS = input.positionCS;
                                        output.positionRWS = input.interp00.xyz;
                                        output.texCoord0 = input.interp01.xyzw;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif // conditional
                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                        output.cullFace = input.cullFace;
                                        #endif // conditional
                                        return output;
                                    }
                                    // Generated Type: VaryingsMeshToDS
                                    struct VaryingsMeshToDS
                                    {
                                        float3 positionRWS;
                                        float3 normalWS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                    };

                                    // Generated Type: PackedVaryingsMeshToDS
                                    struct PackedVaryingsMeshToDS
                                    {
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                        #endif // conditional
                                        float3 interp00 : TEXCOORD0; // auto-packed
                                        float3 interp01 : TEXCOORD1; // auto-packed
                                    };

                                    // Packed Type: VaryingsMeshToDS
                                    PackedVaryingsMeshToDS PackVaryingsMeshToDS(VaryingsMeshToDS input)
                                    {
                                        PackedVaryingsMeshToDS output;
                                        output.interp00.xyz = input.positionRWS;
                                        output.interp01.xyz = input.normalWS;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif // conditional
                                        return output;
                                    }

                                    // Unpacked Type: VaryingsMeshToDS
                                    VaryingsMeshToDS UnpackVaryingsMeshToDS(PackedVaryingsMeshToDS input)
                                    {
                                        VaryingsMeshToDS output;
                                        output.positionRWS = input.interp00.xyz;
                                        output.normalWS = input.interp01.xyz;
                                        #if UNITY_ANY_INSTANCING_ENABLED
                                        output.instanceID = input.instanceID;
                                        #endif // conditional
                                        return output;
                                    }
                                    //-------------------------------------------------------------------------------------
                                    // End Interpolator Packing And Struct Declarations
                                    //-------------------------------------------------------------------------------------

                                    //-------------------------------------------------------------------------------------
                                    // Graph generated code
                                    //-------------------------------------------------------------------------------------
                                            // Shared Graph Properties (uniform inputs)
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

                                            // Pixel Graph Inputs
                                                struct SurfaceDescriptionInputs
                                                {
                                                    float3 ObjectSpacePosition; // optional
                                                    float4 uv0; // optional
                                                };
                                                // Pixel Graph Outputs
                                                    struct SurfaceDescription
                                                    {
                                                        float3 Color;
                                                        float Alpha;
                                                        float AlphaClipThreshold;
                                                    };

                                                    // Shared Graph Node Functions

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

                                                        // Pixel Graph Evaluation
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

                                                            //-------------------------------------------------------------------------------------
                                                            // End graph generated code
                                                            //-------------------------------------------------------------------------------------

                                                        // $include("VertexAnimation.template.hlsl")

                                                        //-------------------------------------------------------------------------------------
                                                            // TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                            //-------------------------------------------------------------------------------------

                                                                FragInputs BuildFragInputs(VaryingsMeshToPS input)
                                                                {
                                                                    FragInputs output;
                                                                    ZERO_INITIALIZE(FragInputs, output);

                                                                    // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
                                                                    // TODO: this is a really poor workaround, but the variable is used in a bunch of places
                                                                    // to compute normals which are then passed on elsewhere to compute other values...
                                                                    output.tangentToWorld = k_identity3x3;
                                                                    output.positionSS = input.positionCS;       // input.positionCS is SV_Position

                                                                    output.positionRWS = input.positionRWS;
                                                                    // output.tangentToWorld = BuildTangentToWorld(input.tangentWS, input.normalWS);
                                                                    output.texCoord0 = input.texCoord0;
                                                                    // output.texCoord1 = input.texCoord1;
                                                                    // output.texCoord2 = input.texCoord2;
                                                                    // output.texCoord3 = input.texCoord3;
                                                                    // output.color = input.color;
                                                                    #if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
                                                                    output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                    #elif SHADER_STAGE_FRAGMENT
                                                                    // output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                    #endif // SHADER_STAGE_FRAGMENT

                                                                    return output;
                                                                }

                                                                SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
                                                                {
                                                                    SurfaceDescriptionInputs output;
                                                                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                    // output.WorldSpaceNormal =            normalize(input.tangentToWorld[2].xyz);
                                                                    // output.ObjectSpaceNormal =           mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M);           // transposed multiplication by inverse matrix to handle normal scale
                                                                    // output.ViewSpaceNormal =             mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
                                                                    // output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                                                                    // output.WorldSpaceTangent =           input.tangentToWorld[0].xyz;
                                                                    // output.ObjectSpaceTangent =          TransformWorldToObjectDir(output.WorldSpaceTangent);
                                                                    // output.ViewSpaceTangent =            TransformWorldToViewDir(output.WorldSpaceTangent);
                                                                    // output.TangentSpaceTangent =         float3(1.0f, 0.0f, 0.0f);
                                                                    // output.WorldSpaceBiTangent =         input.tangentToWorld[1].xyz;
                                                                    // output.ObjectSpaceBiTangent =        TransformWorldToObjectDir(output.WorldSpaceBiTangent);
                                                                    // output.ViewSpaceBiTangent =          TransformWorldToViewDir(output.WorldSpaceBiTangent);
                                                                    // output.TangentSpaceBiTangent =       float3(0.0f, 1.0f, 0.0f);
                                                                    // output.WorldSpaceViewDirection =     normalize(viewWS);
                                                                    // output.ObjectSpaceViewDirection =    TransformWorldToObjectDir(output.WorldSpaceViewDirection);
                                                                    // output.ViewSpaceViewDirection =      TransformWorldToViewDir(output.WorldSpaceViewDirection);
                                                                    // float3x3 tangentSpaceTransform =     float3x3(output.WorldSpaceTangent,output.WorldSpaceBiTangent,output.WorldSpaceNormal);
                                                                    // output.TangentSpaceViewDirection =   mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
                                                                    // output.WorldSpacePosition =          input.positionRWS;
                                                                    output.ObjectSpacePosition = TransformWorldToObject(input.positionRWS);
                                                                    // output.ViewSpacePosition =           TransformWorldToView(input.positionRWS);
                                                                    // output.TangentSpacePosition =        float3(0.0f, 0.0f, 0.0f);
                                                                    // output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionRWS);
                                                                    // output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionRWS), _ProjectionParams.x);
                                                                    output.uv0 = input.texCoord0;
                                                                    // output.uv1 =                         input.texCoord1;
                                                                    // output.uv2 =                         input.texCoord2;
                                                                    // output.uv3 =                         input.texCoord3;
                                                                    // output.VertexColor =                 input.color;
                                                                    // output.FaceSign =                    input.isFrontFace;
                                                                    // output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value

                                                                    return output;
                                                                }

                                                                // existing HDRP code uses the combined function to go directly from packed to frag inputs
                                                                FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
                                                                {
                                                                    UNITY_SETUP_INSTANCE_ID(input);
                                                                    VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
                                                                    return BuildFragInputs(unpacked);
                                                                }

                                                                //-------------------------------------------------------------------------------------
                                                                // END TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                                //-------------------------------------------------------------------------------------



                                                                void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
                                                                {
                                                                    // setup defaults -- these are used if the graph doesn't output a value
                                                                    ZERO_INITIALIZE(SurfaceData, surfaceData);

                                                                    // copy across graph values, if defined
                                                                    surfaceData.color = surfaceDescription.Color;

                                                            #if defined(DEBUG_DISPLAY)
                                                                    if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                                                                    {
                                                                        // TODO
                                                                    }
                                                            #endif
                                                                }

                                                                void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
                                                                {
                                                                    SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
                                                                    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

                                                                    // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                                                                    // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                                                                    DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);

                                                                    BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);

                                                                    // Builtin Data
                                                                    ZERO_INITIALIZE(BuiltinData, builtinData); // No call to InitBuiltinData as we don't have any lighting
                                                                    builtinData.opacity = surfaceDescription.Alpha;
                                                                }

                                                                //-------------------------------------------------------------------------------------
                                                                // Pass Includes
                                                                //-------------------------------------------------------------------------------------
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassLightTransport.hlsl"
                                                                //-------------------------------------------------------------------------------------
                                                                // End Pass Includes
                                                                //-------------------------------------------------------------------------------------

                                                                ENDHLSL
                                                            }

                                                            Pass
                                                            {
                                                                    // based on UnlitPass.template
                                                                    Name "SceneSelectionPass"
                                                                    Tags { "LightMode" = "SceneSelectionPass" }

                                                                    //-------------------------------------------------------------------------------------
                                                                    // Render Modes (Blend, Cull, ZTest, Stencil, etc)
                                                                    //-------------------------------------------------------------------------------------



                                                                    Cull Off
                                                                    ZWrite Off
                                                                    ZTest Always


                                                                    ColorMask 0

                                                                    //-------------------------------------------------------------------------------------
                                                                    // End Render Modes
                                                                    //-------------------------------------------------------------------------------------

                                                                    HLSLPROGRAM

                                                                    #pragma target 4.5
                                                                    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
                                                                    //#pragma enable_d3d11_debug_symbols

                                                                    //enable GPU instancing support
                                                                    #pragma multi_compile_instancing

                                                                    //-------------------------------------------------------------------------------------
                                                                    // Variant Definitions (active field translations to HDRP defines)
                                                                    //-------------------------------------------------------------------------------------
                                                                    #define _SURFACE_TYPE_TRANSPARENT 1
                                                                    // #define _BLENDMODE_ALPHA 1
                                                                    #define _BLENDMODE_ADD 1
                                                                    // #define _BLENDMODE_PRE_MULTIPLY 1
                                                                    // #define _ADD_PRECOMPUTED_VELOCITY

                                                                    //-------------------------------------------------------------------------------------
                                                                    // End Variant Definitions
                                                                    //-------------------------------------------------------------------------------------

                                                                    #pragma vertex Vert
                                                                    #pragma fragment Frag

                                                                    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

                                                                    //-------------------------------------------------------------------------------------
                                                                    // Defines
                                                                    //-------------------------------------------------------------------------------------
                                                                            #define SHADERPASS SHADERPASS_DEPTH_ONLY
                                                                        #define SCENESELECTIONPASS
                                                                        #pragma editor_sync_compilation
                                                                        // ACTIVE FIELDS:
                                                                        //   AlphaTest
                                                                        //   SurfaceType.Transparent
                                                                        //   BlendMode.Add
                                                                        //   VertexDescriptionInputs.ObjectSpaceNormal
                                                                        //   VertexDescriptionInputs.ObjectSpaceTangent
                                                                        //   VertexDescriptionInputs.ObjectSpacePosition
                                                                        //   SurfaceDescription.Alpha
                                                                        //   SurfaceDescription.AlphaClipThreshold
                                                                        //   AttributesMesh.normalOS
                                                                        //   AttributesMesh.tangentOS
                                                                        //   AttributesMesh.positionOS
                                                                        // Shared Graph Keywords

                                                                    // this translates the new dependency tracker into the old preprocessor definitions for the existing HDRP shader code
                                                                    #define ATTRIBUTES_NEED_NORMAL
                                                                    #define ATTRIBUTES_NEED_TANGENT
                                                                    // #define ATTRIBUTES_NEED_TEXCOORD0
                                                                    // #define ATTRIBUTES_NEED_TEXCOORD1
                                                                    // #define ATTRIBUTES_NEED_TEXCOORD2
                                                                    // #define ATTRIBUTES_NEED_TEXCOORD3
                                                                    // #define ATTRIBUTES_NEED_COLOR
                                                                    // #define VARYINGS_NEED_POSITION_WS
                                                                    // #define VARYINGS_NEED_TANGENT_TO_WORLD
                                                                    // #define VARYINGS_NEED_TEXCOORD0
                                                                    // #define VARYINGS_NEED_TEXCOORD1
                                                                    // #define VARYINGS_NEED_TEXCOORD2
                                                                    // #define VARYINGS_NEED_TEXCOORD3
                                                                    // #define VARYINGS_NEED_COLOR
                                                                    // #define VARYINGS_NEED_CULLFACE
                                                                    // #define HAVE_MESH_MODIFICATION

                                                                    //-------------------------------------------------------------------------------------
                                                                    // End Defines
                                                                    //-------------------------------------------------------------------------------------


                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"

                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
                                                                    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                                                    // Used by SceneSelectionPass
                                                                    int _ObjectId;
                                                                    int _PassValue;

                                                                    //-------------------------------------------------------------------------------------
                                                                    // Interpolator Packing And Struct Declarations
                                                                    //-------------------------------------------------------------------------------------
                                                                    // Generated Type: AttributesMesh
                                                                    struct AttributesMesh
                                                                    {
                                                                        float3 positionOS : POSITION;
                                                                        float3 normalOS : NORMAL; // optional
                                                                        float4 tangentOS : TANGENT; // optional
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        uint instanceID : INSTANCEID_SEMANTIC;
                                                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                    };
                                                                    // Generated Type: VaryingsMeshToPS
                                                                    struct VaryingsMeshToPS
                                                                    {
                                                                        float4 positionCS : SV_Position;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                        #endif // defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                    };

                                                                    // Generated Type: PackedVaryingsMeshToPS
                                                                    struct PackedVaryingsMeshToPS
                                                                    {
                                                                        float4 positionCS : SV_Position; // unpacked
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                                                        #endif // conditional
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC; // unpacked
                                                                        #endif // conditional
                                                                    };

                                                                    // Packed Type: VaryingsMeshToPS
                                                                    PackedVaryingsMeshToPS PackVaryingsMeshToPS(VaryingsMeshToPS input)
                                                                    {
                                                                        PackedVaryingsMeshToPS output;
                                                                        output.positionCS = input.positionCS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif // conditional
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        output.cullFace = input.cullFace;
                                                                        #endif // conditional
                                                                        return output;
                                                                    }

                                                                    // Unpacked Type: VaryingsMeshToPS
                                                                    VaryingsMeshToPS UnpackVaryingsMeshToPS(PackedVaryingsMeshToPS input)
                                                                    {
                                                                        VaryingsMeshToPS output;
                                                                        output.positionCS = input.positionCS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif // conditional
                                                                        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                        output.cullFace = input.cullFace;
                                                                        #endif // conditional
                                                                        return output;
                                                                    }
                                                                    // Generated Type: VaryingsMeshToDS
                                                                    struct VaryingsMeshToDS
                                                                    {
                                                                        float3 positionRWS;
                                                                        float3 normalWS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        uint instanceID : CUSTOM_INSTANCE_ID;
                                                                        #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                    };

                                                                    // Generated Type: PackedVaryingsMeshToDS
                                                                    struct PackedVaryingsMeshToDS
                                                                    {
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                                                        #endif // conditional
                                                                        float3 interp00 : TEXCOORD0; // auto-packed
                                                                        float3 interp01 : TEXCOORD1; // auto-packed
                                                                    };

                                                                    // Packed Type: VaryingsMeshToDS
                                                                    PackedVaryingsMeshToDS PackVaryingsMeshToDS(VaryingsMeshToDS input)
                                                                    {
                                                                        PackedVaryingsMeshToDS output;
                                                                        output.interp00.xyz = input.positionRWS;
                                                                        output.interp01.xyz = input.normalWS;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif // conditional
                                                                        return output;
                                                                    }

                                                                    // Unpacked Type: VaryingsMeshToDS
                                                                    VaryingsMeshToDS UnpackVaryingsMeshToDS(PackedVaryingsMeshToDS input)
                                                                    {
                                                                        VaryingsMeshToDS output;
                                                                        output.positionRWS = input.interp00.xyz;
                                                                        output.normalWS = input.interp01.xyz;
                                                                        #if UNITY_ANY_INSTANCING_ENABLED
                                                                        output.instanceID = input.instanceID;
                                                                        #endif // conditional
                                                                        return output;
                                                                    }
                                                                    //-------------------------------------------------------------------------------------
                                                                    // End Interpolator Packing And Struct Declarations
                                                                    //-------------------------------------------------------------------------------------

                                                                    //-------------------------------------------------------------------------------------
                                                                    // Graph generated code
                                                                    //-------------------------------------------------------------------------------------
                                                                            // Shared Graph Properties (uniform inputs)
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

                                                                            // Pixel Graph Inputs
                                                                                struct SurfaceDescriptionInputs
                                                                                {
                                                                                };
                                                                                // Pixel Graph Outputs
                                                                                    struct SurfaceDescription
                                                                                    {
                                                                                        float Alpha;
                                                                                        float AlphaClipThreshold;
                                                                                    };

                                                                                    // Shared Graph Node Functions
                                                                                    // Pixel Graph Evaluation
                                                                                        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                                                                        {
                                                                                            SurfaceDescription surface = (SurfaceDescription)0;
                                                                                            surface.Alpha = 1;
                                                                                            surface.AlphaClipThreshold = 0.5;
                                                                                            return surface;
                                                                                        }

                                                                                        //-------------------------------------------------------------------------------------
                                                                                        // End graph generated code
                                                                                        //-------------------------------------------------------------------------------------

                                                                                    // $include("VertexAnimation.template.hlsl")

                                                                                    //-------------------------------------------------------------------------------------
                                                                                        // TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                                                        //-------------------------------------------------------------------------------------

                                                                                            FragInputs BuildFragInputs(VaryingsMeshToPS input)
                                                                                            {
                                                                                                FragInputs output;
                                                                                                ZERO_INITIALIZE(FragInputs, output);

                                                                                                // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
                                                                                                // TODO: this is a really poor workaround, but the variable is used in a bunch of places
                                                                                                // to compute normals which are then passed on elsewhere to compute other values...
                                                                                                output.tangentToWorld = k_identity3x3;
                                                                                                output.positionSS = input.positionCS;       // input.positionCS is SV_Position

                                                                                                // output.positionRWS = input.positionRWS;
                                                                                                // output.tangentToWorld = BuildTangentToWorld(input.tangentWS, input.normalWS);
                                                                                                // output.texCoord0 = input.texCoord0;
                                                                                                // output.texCoord1 = input.texCoord1;
                                                                                                // output.texCoord2 = input.texCoord2;
                                                                                                // output.texCoord3 = input.texCoord3;
                                                                                                // output.color = input.color;
                                                                                                #if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
                                                                                                output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                #elif SHADER_STAGE_FRAGMENT
                                                                                                // output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                #endif // SHADER_STAGE_FRAGMENT

                                                                                                return output;
                                                                                            }

                                                                                            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
                                                                                            {
                                                                                                SurfaceDescriptionInputs output;
                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                // output.WorldSpaceNormal =            normalize(input.tangentToWorld[2].xyz);
                                                                                                // output.ObjectSpaceNormal =           mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M);           // transposed multiplication by inverse matrix to handle normal scale
                                                                                                // output.ViewSpaceNormal =             mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
                                                                                                // output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                                                                                                // output.WorldSpaceTangent =           input.tangentToWorld[0].xyz;
                                                                                                // output.ObjectSpaceTangent =          TransformWorldToObjectDir(output.WorldSpaceTangent);
                                                                                                // output.ViewSpaceTangent =            TransformWorldToViewDir(output.WorldSpaceTangent);
                                                                                                // output.TangentSpaceTangent =         float3(1.0f, 0.0f, 0.0f);
                                                                                                // output.WorldSpaceBiTangent =         input.tangentToWorld[1].xyz;
                                                                                                // output.ObjectSpaceBiTangent =        TransformWorldToObjectDir(output.WorldSpaceBiTangent);
                                                                                                // output.ViewSpaceBiTangent =          TransformWorldToViewDir(output.WorldSpaceBiTangent);
                                                                                                // output.TangentSpaceBiTangent =       float3(0.0f, 1.0f, 0.0f);
                                                                                                // output.WorldSpaceViewDirection =     normalize(viewWS);
                                                                                                // output.ObjectSpaceViewDirection =    TransformWorldToObjectDir(output.WorldSpaceViewDirection);
                                                                                                // output.ViewSpaceViewDirection =      TransformWorldToViewDir(output.WorldSpaceViewDirection);
                                                                                                // float3x3 tangentSpaceTransform =     float3x3(output.WorldSpaceTangent,output.WorldSpaceBiTangent,output.WorldSpaceNormal);
                                                                                                // output.TangentSpaceViewDirection =   mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
                                                                                                // output.WorldSpacePosition =          input.positionRWS;
                                                                                                // output.ObjectSpacePosition =         TransformWorldToObject(input.positionRWS);
                                                                                                // output.ViewSpacePosition =           TransformWorldToView(input.positionRWS);
                                                                                                // output.TangentSpacePosition =        float3(0.0f, 0.0f, 0.0f);
                                                                                                // output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionRWS);
                                                                                                // output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionRWS), _ProjectionParams.x);
                                                                                                // output.uv0 =                         input.texCoord0;
                                                                                                // output.uv1 =                         input.texCoord1;
                                                                                                // output.uv2 =                         input.texCoord2;
                                                                                                // output.uv3 =                         input.texCoord3;
                                                                                                // output.VertexColor =                 input.color;
                                                                                                // output.FaceSign =                    input.isFrontFace;
                                                                                                // output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value

                                                                                                return output;
                                                                                            }

                                                                                            // existing HDRP code uses the combined function to go directly from packed to frag inputs
                                                                                            FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
                                                                                            {
                                                                                                UNITY_SETUP_INSTANCE_ID(input);
                                                                                                VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
                                                                                                return BuildFragInputs(unpacked);
                                                                                            }

                                                                                            //-------------------------------------------------------------------------------------
                                                                                            // END TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                                                            //-------------------------------------------------------------------------------------



                                                                                            void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
                                                                                            {
                                                                                                // setup defaults -- these are used if the graph doesn't output a value
                                                                                                ZERO_INITIALIZE(SurfaceData, surfaceData);

                                                                                                // copy across graph values, if defined
                                                                                                // surfaceData.color = surfaceDescription.Color;

                                                                                        #if defined(DEBUG_DISPLAY)
                                                                                                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                                                                                                {
                                                                                                    // TODO
                                                                                                }
                                                                                        #endif
                                                                                            }

                                                                                            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
                                                                                            {
                                                                                                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
                                                                                                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

                                                                                                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                                                                                                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                                                                                                DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);

                                                                                                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);

                                                                                                // Builtin Data
                                                                                                ZERO_INITIALIZE(BuiltinData, builtinData); // No call to InitBuiltinData as we don't have any lighting
                                                                                                builtinData.opacity = surfaceDescription.Alpha;
                                                                                            }

                                                                                            //-------------------------------------------------------------------------------------
                                                                                            // Pass Includes
                                                                                            //-------------------------------------------------------------------------------------
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
                                                                                            //-------------------------------------------------------------------------------------
                                                                                            // End Pass Includes
                                                                                            //-------------------------------------------------------------------------------------

                                                                                            ENDHLSL
                                                                                        }

                                                                                        Pass
                                                                                        {
                                                                                                // based on UnlitPass.template
                                                                                                Name "ForwardOnly"
                                                                                                Tags { "LightMode" = "ForwardOnly" }

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Render Modes (Blend, Cull, ZTest, Stencil, etc)
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                Blend One One, One One



                                                                                                Cull Off
                                                                                                ZWrite Off
                                                                                                ZTest Always

                                                                                                // Stencil setup
                                                                                            Stencil
                                                                                            {
                                                                                               WriteMask 3
                                                                                               Ref  0
                                                                                               Comp Always
                                                                                               Pass Replace
                                                                                            }


                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // End Render Modes
                                                                                                //-------------------------------------------------------------------------------------

                                                                                                HLSLPROGRAM

                                                                                                #pragma target 4.5
                                                                                                #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
                                                                                                //#pragma enable_d3d11_debug_symbols

                                                                                                //enable GPU instancing support
                                                                                                #pragma multi_compile_instancing

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Variant Definitions (active field translations to HDRP defines)
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                #define _SURFACE_TYPE_TRANSPARENT 1
                                                                                                // #define _BLENDMODE_ALPHA 1
                                                                                                #define _BLENDMODE_ADD 1
                                                                                                // #define _BLENDMODE_PRE_MULTIPLY 1
                                                                                                // #define _ADD_PRECOMPUTED_VELOCITY

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // End Variant Definitions
                                                                                                //-------------------------------------------------------------------------------------

                                                                                                #pragma vertex Vert
                                                                                                #pragma fragment Frag

                                                                                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Defines
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                        #define SHADERPASS SHADERPASS_FORWARD_UNLIT
                                                                                                    #pragma multi_compile _ DEBUG_DISPLAY
                                                                                                    // ACTIVE FIELDS:
                                                                                                    //   AlphaTest
                                                                                                    //   SurfaceType.Transparent
                                                                                                    //   BlendMode.Add
                                                                                                    //   SurfaceDescriptionInputs.ObjectSpacePosition
                                                                                                    //   SurfaceDescriptionInputs.uv0
                                                                                                    //   VertexDescriptionInputs.ObjectSpaceNormal
                                                                                                    //   VertexDescriptionInputs.ObjectSpaceTangent
                                                                                                    //   VertexDescriptionInputs.ObjectSpacePosition
                                                                                                    //   SurfaceDescription.Color
                                                                                                    //   SurfaceDescription.Alpha
                                                                                                    //   SurfaceDescription.AlphaClipThreshold
                                                                                                    //   FragInputs.positionRWS
                                                                                                    //   FragInputs.texCoord0
                                                                                                    //   AttributesMesh.normalOS
                                                                                                    //   AttributesMesh.tangentOS
                                                                                                    //   AttributesMesh.positionOS
                                                                                                    //   VaryingsMeshToPS.positionRWS
                                                                                                    //   VaryingsMeshToPS.texCoord0
                                                                                                    //   AttributesMesh.uv0
                                                                                                    // Shared Graph Keywords

                                                                                                // this translates the new dependency tracker into the old preprocessor definitions for the existing HDRP shader code
                                                                                                #define ATTRIBUTES_NEED_NORMAL
                                                                                                #define ATTRIBUTES_NEED_TANGENT
                                                                                                #define ATTRIBUTES_NEED_TEXCOORD0
                                                                                                // #define ATTRIBUTES_NEED_TEXCOORD1
                                                                                                // #define ATTRIBUTES_NEED_TEXCOORD2
                                                                                                // #define ATTRIBUTES_NEED_TEXCOORD3
                                                                                                // #define ATTRIBUTES_NEED_COLOR
                                                                                                #define VARYINGS_NEED_POSITION_WS
                                                                                                // #define VARYINGS_NEED_TANGENT_TO_WORLD
                                                                                                #define VARYINGS_NEED_TEXCOORD0
                                                                                                // #define VARYINGS_NEED_TEXCOORD1
                                                                                                // #define VARYINGS_NEED_TEXCOORD2
                                                                                                // #define VARYINGS_NEED_TEXCOORD3
                                                                                                // #define VARYINGS_NEED_COLOR
                                                                                                // #define VARYINGS_NEED_CULLFACE
                                                                                                // #define HAVE_MESH_MODIFICATION

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // End Defines
                                                                                                //-------------------------------------------------------------------------------------


                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"

                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

                                                                                                // Used by SceneSelectionPass
                                                                                                int _ObjectId;
                                                                                                int _PassValue;

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Interpolator Packing And Struct Declarations
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Generated Type: AttributesMesh
                                                                                                struct AttributesMesh
                                                                                                {
                                                                                                    float3 positionOS : POSITION;
                                                                                                    float3 normalOS : NORMAL; // optional
                                                                                                    float4 tangentOS : TANGENT; // optional
                                                                                                    float4 uv0 : TEXCOORD0; // optional
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    uint instanceID : INSTANCEID_SEMANTIC;
                                                                                                    #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                                                };
                                                                                                // Generated Type: VaryingsMeshToPS
                                                                                                struct VaryingsMeshToPS
                                                                                                {
                                                                                                    float4 positionCS : SV_Position;
                                                                                                    float3 positionRWS; // optional
                                                                                                    float4 texCoord0; // optional
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                    #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                                                                                    #endif // defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                };

                                                                                                // Generated Type: PackedVaryingsMeshToPS
                                                                                                struct PackedVaryingsMeshToPS
                                                                                                {
                                                                                                    float4 positionCS : SV_Position; // unpacked
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                                                                                    #endif // conditional
                                                                                                    float3 interp00 : TEXCOORD0; // auto-packed
                                                                                                    float4 interp01 : TEXCOORD1; // auto-packed
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC; // unpacked
                                                                                                    #endif // conditional
                                                                                                };

                                                                                                // Packed Type: VaryingsMeshToPS
                                                                                                PackedVaryingsMeshToPS PackVaryingsMeshToPS(VaryingsMeshToPS input)
                                                                                                {
                                                                                                    PackedVaryingsMeshToPS output;
                                                                                                    output.positionCS = input.positionCS;
                                                                                                    output.interp00.xyz = input.positionRWS;
                                                                                                    output.interp01.xyzw = input.texCoord0;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif // conditional
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    output.cullFace = input.cullFace;
                                                                                                    #endif // conditional
                                                                                                    return output;
                                                                                                }

                                                                                                // Unpacked Type: VaryingsMeshToPS
                                                                                                VaryingsMeshToPS UnpackVaryingsMeshToPS(PackedVaryingsMeshToPS input)
                                                                                                {
                                                                                                    VaryingsMeshToPS output;
                                                                                                    output.positionCS = input.positionCS;
                                                                                                    output.positionRWS = input.interp00.xyz;
                                                                                                    output.texCoord0 = input.interp01.xyzw;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif // conditional
                                                                                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                                                                                    output.cullFace = input.cullFace;
                                                                                                    #endif // conditional
                                                                                                    return output;
                                                                                                }
                                                                                                // Generated Type: VaryingsMeshToDS
                                                                                                struct VaryingsMeshToDS
                                                                                                {
                                                                                                    float3 positionRWS;
                                                                                                    float3 normalWS;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    uint instanceID : CUSTOM_INSTANCE_ID;
                                                                                                    #endif // UNITY_ANY_INSTANCING_ENABLED
                                                                                                };

                                                                                                // Generated Type: PackedVaryingsMeshToDS
                                                                                                struct PackedVaryingsMeshToDS
                                                                                                {
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    uint instanceID : CUSTOM_INSTANCE_ID; // unpacked
                                                                                                    #endif // conditional
                                                                                                    float3 interp00 : TEXCOORD0; // auto-packed
                                                                                                    float3 interp01 : TEXCOORD1; // auto-packed
                                                                                                };

                                                                                                // Packed Type: VaryingsMeshToDS
                                                                                                PackedVaryingsMeshToDS PackVaryingsMeshToDS(VaryingsMeshToDS input)
                                                                                                {
                                                                                                    PackedVaryingsMeshToDS output;
                                                                                                    output.interp00.xyz = input.positionRWS;
                                                                                                    output.interp01.xyz = input.normalWS;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif // conditional
                                                                                                    return output;
                                                                                                }

                                                                                                // Unpacked Type: VaryingsMeshToDS
                                                                                                VaryingsMeshToDS UnpackVaryingsMeshToDS(PackedVaryingsMeshToDS input)
                                                                                                {
                                                                                                    VaryingsMeshToDS output;
                                                                                                    output.positionRWS = input.interp00.xyz;
                                                                                                    output.normalWS = input.interp01.xyz;
                                                                                                    #if UNITY_ANY_INSTANCING_ENABLED
                                                                                                    output.instanceID = input.instanceID;
                                                                                                    #endif // conditional
                                                                                                    return output;
                                                                                                }
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // End Interpolator Packing And Struct Declarations
                                                                                                //-------------------------------------------------------------------------------------

                                                                                                //-------------------------------------------------------------------------------------
                                                                                                // Graph generated code
                                                                                                //-------------------------------------------------------------------------------------
                                                                                                        // Shared Graph Properties (uniform inputs)
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

                                                                                                        // Pixel Graph Inputs
                                                                                                            struct SurfaceDescriptionInputs
                                                                                                            {
                                                                                                                float3 ObjectSpacePosition; // optional
                                                                                                                float4 uv0; // optional
                                                                                                            };
                                                                                                            // Pixel Graph Outputs
                                                                                                                struct SurfaceDescription
                                                                                                                {
                                                                                                                    float3 Color;
                                                                                                                    float Alpha;
                                                                                                                    float AlphaClipThreshold;
                                                                                                                };

                                                                                                                // Shared Graph Node Functions

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

                                                                                                                    // Pixel Graph Evaluation
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

                                                                                                                        //-------------------------------------------------------------------------------------
                                                                                                                        // End graph generated code
                                                                                                                        //-------------------------------------------------------------------------------------

                                                                                                                    // $include("VertexAnimation.template.hlsl")

                                                                                                                    //-------------------------------------------------------------------------------------
                                                                                                                        // TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                                                                                        //-------------------------------------------------------------------------------------

                                                                                                                            FragInputs BuildFragInputs(VaryingsMeshToPS input)
                                                                                                                            {
                                                                                                                                FragInputs output;
                                                                                                                                ZERO_INITIALIZE(FragInputs, output);

                                                                                                                                // Init to some default value to make the computer quiet (else it output 'divide by zero' warning even if value is not used).
                                                                                                                                // TODO: this is a really poor workaround, but the variable is used in a bunch of places
                                                                                                                                // to compute normals which are then passed on elsewhere to compute other values...
                                                                                                                                output.tangentToWorld = k_identity3x3;
                                                                                                                                output.positionSS = input.positionCS;       // input.positionCS is SV_Position

                                                                                                                                output.positionRWS = input.positionRWS;
                                                                                                                                // output.tangentToWorld = BuildTangentToWorld(input.tangentWS, input.normalWS);
                                                                                                                                output.texCoord0 = input.texCoord0;
                                                                                                                                // output.texCoord1 = input.texCoord1;
                                                                                                                                // output.texCoord2 = input.texCoord2;
                                                                                                                                // output.texCoord3 = input.texCoord3;
                                                                                                                                // output.color = input.color;
                                                                                                                                #if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
                                                                                                                                output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                                #elif SHADER_STAGE_FRAGMENT
                                                                                                                                // output.isFrontFace = IS_FRONT_VFACE(input.cullFace, true, false);
                                                                                                                                #endif // SHADER_STAGE_FRAGMENT

                                                                                                                                return output;
                                                                                                                            }

                                                                                                                            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
                                                                                                                            {
                                                                                                                                SurfaceDescriptionInputs output;
                                                                                                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                                                                                                                // output.WorldSpaceNormal =            normalize(input.tangentToWorld[2].xyz);
                                                                                                                                // output.ObjectSpaceNormal =           mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_M);           // transposed multiplication by inverse matrix to handle normal scale
                                                                                                                                // output.ViewSpaceNormal =             mul(output.WorldSpaceNormal, (float3x3) UNITY_MATRIX_I_V);         // transposed multiplication by inverse matrix to handle normal scale
                                                                                                                                // output.TangentSpaceNormal =          float3(0.0f, 0.0f, 1.0f);
                                                                                                                                // output.WorldSpaceTangent =           input.tangentToWorld[0].xyz;
                                                                                                                                // output.ObjectSpaceTangent =          TransformWorldToObjectDir(output.WorldSpaceTangent);
                                                                                                                                // output.ViewSpaceTangent =            TransformWorldToViewDir(output.WorldSpaceTangent);
                                                                                                                                // output.TangentSpaceTangent =         float3(1.0f, 0.0f, 0.0f);
                                                                                                                                // output.WorldSpaceBiTangent =         input.tangentToWorld[1].xyz;
                                                                                                                                // output.ObjectSpaceBiTangent =        TransformWorldToObjectDir(output.WorldSpaceBiTangent);
                                                                                                                                // output.ViewSpaceBiTangent =          TransformWorldToViewDir(output.WorldSpaceBiTangent);
                                                                                                                                // output.TangentSpaceBiTangent =       float3(0.0f, 1.0f, 0.0f);
                                                                                                                                // output.WorldSpaceViewDirection =     normalize(viewWS);
                                                                                                                                // output.ObjectSpaceViewDirection =    TransformWorldToObjectDir(output.WorldSpaceViewDirection);
                                                                                                                                // output.ViewSpaceViewDirection =      TransformWorldToViewDir(output.WorldSpaceViewDirection);
                                                                                                                                // float3x3 tangentSpaceTransform =     float3x3(output.WorldSpaceTangent,output.WorldSpaceBiTangent,output.WorldSpaceNormal);
                                                                                                                                // output.TangentSpaceViewDirection =   mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
                                                                                                                                // output.WorldSpacePosition =          input.positionRWS;
                                                                                                                                output.ObjectSpacePosition = TransformWorldToObject(input.positionRWS);
                                                                                                                                // output.ViewSpacePosition =           TransformWorldToView(input.positionRWS);
                                                                                                                                // output.TangentSpacePosition =        float3(0.0f, 0.0f, 0.0f);
                                                                                                                                // output.AbsoluteWorldSpacePosition =  GetAbsolutePositionWS(input.positionRWS);
                                                                                                                                // output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionRWS), _ProjectionParams.x);
                                                                                                                                output.uv0 = input.texCoord0;
                                                                                                                                // output.uv1 =                         input.texCoord1;
                                                                                                                                // output.uv2 =                         input.texCoord2;
                                                                                                                                // output.uv3 =                         input.texCoord3;
                                                                                                                                // output.VertexColor =                 input.color;
                                                                                                                                // output.FaceSign =                    input.isFrontFace;
                                                                                                                                // output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value

                                                                                                                                return output;
                                                                                                                            }

                                                                                                                            // existing HDRP code uses the combined function to go directly from packed to frag inputs
                                                                                                                            FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
                                                                                                                            {
                                                                                                                                UNITY_SETUP_INSTANCE_ID(input);
                                                                                                                                VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
                                                                                                                                return BuildFragInputs(unpacked);
                                                                                                                            }

                                                                                                                            //-------------------------------------------------------------------------------------
                                                                                                                            // END TEMPLATE INCLUDE : SharedCode.template.hlsl
                                                                                                                            //-------------------------------------------------------------------------------------



                                                                                                                            void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData)
                                                                                                                            {
                                                                                                                                // setup defaults -- these are used if the graph doesn't output a value
                                                                                                                                ZERO_INITIALIZE(SurfaceData, surfaceData);

                                                                                                                                // copy across graph values, if defined
                                                                                                                                surfaceData.color = surfaceDescription.Color;

                                                                                                                        #if defined(DEBUG_DISPLAY)
                                                                                                                                if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
                                                                                                                                {
                                                                                                                                    // TODO
                                                                                                                                }
                                                                                                                        #endif
                                                                                                                            }

                                                                                                                            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
                                                                                                                            {
                                                                                                                                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
                                                                                                                                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

                                                                                                                                // Perform alpha test very early to save performance (a killed pixel will not sample textures)
                                                                                                                                // TODO: split graph evaluation to grab just alpha dependencies first? tricky..
                                                                                                                                DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);

                                                                                                                                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);

                                                                                                                                // Builtin Data
                                                                                                                                ZERO_INITIALIZE(BuiltinData, builtinData); // No call to InitBuiltinData as we don't have any lighting
                                                                                                                                builtinData.opacity = surfaceDescription.Alpha;
                                                                                                                            }

                                                                                                                            //-------------------------------------------------------------------------------------
                                                                                                                            // Pass Includes
                                                                                                                            //-------------------------------------------------------------------------------------
                                                                                                                                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForwardUnlit.hlsl"
                                                                                                                            //-------------------------------------------------------------------------------------
                                                                                                                            // End Pass Includes
                                                                                                                            //-------------------------------------------------------------------------------------

                                                                                                                            ENDHLSL
                                                                                                                        }

    }
        CustomEditor "UnityEditor.Rendering.HighDefinition.UnlitUI"
                                                                                                                                FallBack "Hidden/Shader Graph/FallbackError"
}
