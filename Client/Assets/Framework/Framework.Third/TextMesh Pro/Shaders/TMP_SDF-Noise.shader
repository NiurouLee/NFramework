Shader "TextMeshPro/Distance Field Noise" {
    // 全局UV功能使用说明:
    // 1. 此shader专门用于全局UV效果，纹理将应用于整个文本区域，而不是单独的字符
    // 2. 可以通过_FaceUVSpeedX和_FaceUVSpeedY调整纹理流动速度
    // 3. 使用_GlobalUVScale调整UV缩放，较小的值会使纹理看起来更大
    // 4. 此功能使用世界空间坐标计算UV，确保纹理在整个文本上连续流动
    // 5. FaceTex应使用黑白灰的噪声图，噪声图中越白的部分最终颜色越接近白色
    // 6. 使用_NoiseStrength调整噪声效果的强度，值越大，效果越明显
    // 7. 此功能适用于创建整体文本特效，如火焰、水波等动态纹理效果
    
    Properties {
        _FaceTex			("Face Texture", 2D) = "white" {}
        _FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
        _FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
        _FaceColor		    ("Face Color", Color) = (1,1,1,1)
        _FaceDilate			("Face Dilate", Range(-1,1)) = 0
        [Header(Global UV Settings)]
        [Space(10)]
        _GlobalUVScale      ("Global UV Scale", Float) = 0.01
        _NoiseStrength      ("Noise Strength", Range(0, 1)) = 0.5
        [Space(10)]
    
        _OutlineColor	    ("Outline Color", Color) = (0,0,0,1)
        _OutlineTex			("Outline Texture", 2D) = "white" {}
        _OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
        _OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
        _OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
        _OutlineSoftness	("Outline Softness", Range(0,1)) = 0
    
        _Bevel				("Bevel", Range(0,1)) = 0.5
        _BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
        _BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
        _BevelClamp			("Bevel Clamp", Range(0,1)) = 0
        _BevelRoundness		("Bevel Roundness", Range(0,1)) = 0
    
        _LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
        _SpecularColor	    ("Specular", Color) = (1,1,1,1)
        _SpecularPower		("Specular", Range(0,4)) = 2.0
        _Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
        _Diffuse			("Diffuse", Range(0,1)) = 0.5
        _Ambient			("Ambient", Range(1,0)) = 0.5
    
        _BumpMap 			("Normal map", 2D) = "bump" {}
        _BumpOutline		("Bump Outline", Range(0,1)) = 0
        _BumpFace			("Bump Face", Range(0,1)) = 0
    
        _ReflectFaceColor	("Reflection Color", Color) = (0,0,0,1)
        _ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
        _Cube 				("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
        _EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)
    
    
        _UnderlayColor	    ("Border Color", Color) = (0,0,0, 0.5)
        _UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
        _UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
        _UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
        _UnderlaySoftness	("Border Softness", Range(0,1)) = 0
    
        _GlowColor		    ("Color", Color) = (0, 1, 0, 0.5)
        _GlowOffset			("Offset", Range(-1,1)) = 0
        _GlowInner			("Inner", Range(0,1)) = 0.05
        _GlowOuter			("Outer", Range(0,1)) = 0.05
        _GlowPower			("Falloff", Range(1, 0)) = 0.75
    
        _WeightNormal		("Weight Normal", float) = 0
        _WeightBold			("Weight Bold", float) = 0.5
    
        _ShaderFlags		("Flags", float) = 0
        _ScaleRatioA		("Scale RatioA", float) = 1
        _ScaleRatioB		("Scale RatioB", float) = 1
        _ScaleRatioC		("Scale RatioC", float) = 1
    
        _MainTex			("Font Atlas", 2D) = "white" {}
        _TextureWidth		("Texture Width", float) = 512
        _TextureHeight		("Texture Height", float) = 512
        _GradientScale		("Gradient Scale", float) = 5.0
        _ScaleX				("Scale X", float) = 1.0
        _ScaleY				("Scale Y", float) = 1.0
        _PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
        _Sharpness			("Sharpness", Range(-1,1)) = 0
    
        _VertexOffsetX		("Vertex OffsetX", float) = 0
        _VertexOffsetY		("Vertex OffsetY", float) = 0
    
        _MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
        _ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
        _MaskSoftnessX		("Mask SoftnessX", float) = 0
        _MaskSoftnessY		("Mask SoftnessY", float) = 0
    
        _StencilComp		("Stencil Comparison", Float) = 8
        _Stencil			("Stencil ID", Float) = 0
        _StencilOp			("Stencil Operation", Float) = 0
        _StencilWriteMask	("Stencil Write Mask", Float) = 255
        _StencilReadMask	("Stencil Read Mask", Float) = 255
    
        _CullMode			("Cull Mode", Float) = 0
        _ColorMask			("Color Mask", Float) = 15
    }
    
    SubShader {
    
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
    
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
    
        Cull [_CullMode]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]
    
        Pass {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma shader_feature __ BEVEL_ON
            #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
            #pragma shader_feature __ GLOW_ON
    
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
    
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "TMPro_Properties.cginc"
            #include "TMPro.cginc"
    
            struct vertex_t
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4	position		: POSITION;
                float3	normal			: NORMAL;
                fixed4	color			: COLOR;
                float4	texcoord0		: TEXCOORD0;
                float2	texcoord1		: TEXCOORD1;
            };
    
            struct pixel_t
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                float4	position		: SV_POSITION;
                fixed4	color			: COLOR;
                float2	atlas			: TEXCOORD0;		// Atlas
                float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
                float4	mask			: TEXCOORD2;		// Position in object space(xy), pixel Size(zw)
                float3	viewDir			: TEXCOORD3;
    
                #if (UNDERLAY_ON || UNDERLAY_INNER)
                float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
                fixed4	underlayColor	: COLOR1;
                #endif
    
                float4 textures			: TEXCOORD5;
            };
    
            // Used by Unity internally to handle Texture Tiling and Offset.
            float4 _FaceTex_ST;
            float4 _OutlineTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;
            float _GlobalUVScale;
            float _NoiseStrength;
    
            pixel_t VertShader(vertex_t input)
            {
                pixel_t output;
    
                UNITY_INITIALIZE_OUTPUT(pixel_t, output);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input,output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
                float bold = step(input.texcoord0.w, 0);
    
                float4 vert = input.position;
                vert.x += _VertexOffsetX;
                vert.y += _VertexOffsetY;
    
                float4 vPosition = UnityObjectToClipPos(vert);
    
                float2 pixelSize = vPosition.w;
                pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float scale = rsqrt(dot(pixelSize, pixelSize));
                scale *= abs(input.texcoord0.w) * _GradientScale * (_Sharpness + 1);
                if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));
    
                float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
                weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;
    
                float bias =(.5 - weight) + (.5 / scale);
    
                float alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA);
    
                #if GLOW_ON
                alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
                #endif
    
                alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;
    
                #if (UNDERLAY_ON || UNDERLAY_INNER)
                float4 underlayColor = _UnderlayColor;
                underlayColor.rgb *= underlayColor.a;
    
                float bScale = scale;
                bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
                float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);
    
                float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
                float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
                float2 bOffset = float2(x, y);
                #endif
    
                // Generate UV for the Masking Texture
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
    
                // 使用世界空间坐标计算全局UV
                float3 worldPos = mul(unity_ObjectToWorld, vert).xyz;
                float2 globalUV = worldPos.xy * _GlobalUVScale;
                
                // Apply texture tiling and offset to global UV
                float2 faceUV = TRANSFORM_TEX(globalUV, _FaceTex);
                float2 outlineUV = TRANSFORM_TEX(globalUV, _OutlineTex);
    
                if (_UIVertexColorAlwaysGammaSpace && !IsGammaSpace())
                {
                    input.color.rgb = UIGammaToLinear(input.color.rgb);
                }
                output.position = vPosition;
                output.color = input.color;
                output.atlas =	input.texcoord0;
                output.param =	float4(alphaClip, scale, bias, weight);
                const half2 maskSoftness = half2(max(_UIMaskSoftnessX, _MaskSoftnessX), max(_UIMaskSoftnessY, _MaskSoftnessY));
                output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * maskSoftness + pixelSize.xy));
                output.viewDir =	mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
                #if (UNDERLAY_ON || UNDERLAY_INNER)
                output.texcoord2 = float4(input.texcoord0 + bOffset, bScale, bBias);
                output.underlayColor =	underlayColor;
                #endif
                output.textures = float4(faceUV, outlineUV);
    
                return output;
            }
    
    
            fixed4 PixShader(pixel_t input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
    
                float c = tex2D(_MainTex, input.atlas).a;
    
                #ifndef UNDERLAY_ON
                clip(c - input.param.x);
                #endif
    
                float	scale	= input.param.y;
                float	bias	= input.param.z;
                float	weight	= input.param.w;
                float	sd = (bias - c) * scale;
    
                float outline = (_OutlineWidth * _ScaleRatioA) * scale;
                float softness = (_OutlineSoftness * _ScaleRatioA) * scale;
    
                half4 faceColor = _FaceColor;
                half4 outlineColor = _OutlineColor;
    
                faceColor.rgb *= input.color.rgb;
    
                // 采样噪声纹理
                half4 noiseTex = tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
                
                // 简化的噪声处理：直接使用噪声纹理的值
                float noiseValue = noiseTex.r;
                
                // 将原始颜色与白色进行混合，混合比例由噪声值和强度决定
                // 这样噪声图中越白的部分最终颜色越接近白色
                faceColor.rgb = lerp(faceColor.rgb, float3(1, 1, 1), noiseValue * _NoiseStrength);
                
                outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);
    
                faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);
    
                #if BEVEL_ON
                float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
                float3 n = GetSurfaceNormal(input.atlas, weight, dxy);
    
                float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
                bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
                n = normalize(n- bump);
    
                float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));
    
                float3 col = GetSpecular(n, light);
                faceColor.rgb += col*faceColor.a;
                faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
                faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);
    
                fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
                faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
                #endif
    
                #if UNDERLAY_ON
                float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
                faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
                #endif
    
                #if UNDERLAY_INNER
                float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
                faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
                #endif
    
                #if GLOW_ON
                float4 glowColor = GetGlowColor(sd, scale);
                faceColor.rgb += glowColor.rgb * glowColor.a;
                #endif
    
            // Alternative implementation to UnityGet2DClipping with support for softness.
                #if UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
                faceColor *= m.x * m.y;
                #endif
    
                #if UNITY_UI_ALPHACLIP
                clip(faceColor.a - 0.001);
                #endif
    
                  return faceColor * input.color.a;
            }
            ENDCG
        }
    }
    
    Fallback "TextMeshPro/Mobile/Distance Field"
    CustomEditor "TMP_SDF_NoiseShaderGUI"
    }
    