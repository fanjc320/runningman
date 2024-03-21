// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Distance Field"
{
  Properties
  {
    _FaceTex ("Face Texture", 2D) = "white" {}
    _FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0
    _FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0
    _FaceColor ("Face Color", Color) = (1,1,1,1)
    _FaceDilate ("Face Dilate", Range(-1, 1)) = 0
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _OutlineTex ("Outline Texture", 2D) = "white" {}
    _OutlineUVSpeedX ("Outline UV Speed X", Range(-5, 5)) = 0
    _OutlineUVSpeedY ("Outline UV Speed Y", Range(-5, 5)) = 0
    _OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
    _OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
    _Bevel ("Bevel", Range(0, 1)) = 0.5
    _BevelOffset ("Bevel Offset", Range(-0.5, 0.5)) = 0
    _BevelWidth ("Bevel Width", Range(-0.5, 0.5)) = 0
    _BevelClamp ("Bevel Clamp", Range(0, 1)) = 0
    _BevelRoundness ("Bevel Roundness", Range(0, 1)) = 0
    _LightAngle ("Light Angle", Range(0, 6.283185)) = 3.1416
    _SpecularColor ("Specular", Color) = (1,1,1,1)
    _SpecularPower ("Specular", Range(0, 4)) = 2
    _Reflectivity ("Reflectivity", Range(5, 15)) = 10
    _Diffuse ("Diffuse", Range(0, 1)) = 0.5
    _Ambient ("Ambient", Range(1, 0)) = 0.5
    _BumpMap ("Normal map", 2D) = "bump" {}
    _BumpOutline ("Bump Outline", Range(0, 1)) = 0
    _BumpFace ("Bump Face", Range(0, 1)) = 0
    _ReflectFaceColor ("Reflection Color", Color) = (0,0,0,1)
    _ReflectOutlineColor ("Reflection Color", Color) = (0,0,0,1)
    _Cube ("Reflection Cubemap", Cube) = "black" {}
    _EnvMatrixRotation ("Texture Rotation", Vector) = (0,0,0,0)
    _UnderlayColor ("Border Color", Color) = (0,0,0,0.5)
    _UnderlayOffsetX ("Border OffsetX", Range(-1, 1)) = 0
    _UnderlayOffsetY ("Border OffsetY", Range(-1, 1)) = 0
    _UnderlayDilate ("Border Dilate", Range(-1, 1)) = 0
    _UnderlaySoftness ("Border Softness", Range(0, 1)) = 0
    _GlowColor ("Color", Color) = (0,1,0,0.5)
    _GlowOffset ("Offset", Range(-1, 1)) = 0
    _GlowInner ("Inner", Range(0, 1)) = 0.05
    _GlowOuter ("Outer", Range(0, 1)) = 0.05
    _GlowPower ("Falloff", Range(1, 0)) = 0.75
    _WeightNormal ("Weight Normal", float) = 0
    _WeightBold ("Weight Bold", float) = 0.5
    _ShaderFlags ("Flags", float) = 0
    _ScaleRatioA ("Scale RatioA", float) = 1
    _ScaleRatioB ("Scale RatioB", float) = 1
    _ScaleRatioC ("Scale RatioC", float) = 1
    _MainTex ("Font Atlas", 2D) = "white" {}
    _TextureWidth ("Texture Width", float) = 512
    _TextureHeight ("Texture Height", float) = 512
    _GradientScale ("Gradient Scale", float) = 5
    _ScaleX ("Scale X", float) = 1
    _ScaleY ("Scale Y", float) = 1
    _PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
    _VertexOffsetX ("Vertex OffsetX", float) = 0
    _VertexOffsetY ("Vertex OffsetY", float) = 0
    _ClipRect ("Mask Coords", Vector) = (0,0,100000,100000)
    _MaskSoftnessX ("Mask SoftnessX", float) = 0
    _MaskSoftnessY ("Mask SoftnessY", float) = 0
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Stencil
      { 
        Ref 0
        ReadMask 0
        WriteMask 0
        Pass Keep
        Fail Keep
        ZFail Keep
        PassFront Keep
        FailFront Keep
        ZFailFront Keep
        PassBack Keep
        FailBack Keep
        ZFailBack Keep
      } 
      Fog
      { 
        Mode  Off
      } 
      Blend One OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ScreenParams;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 UNITY_MATRIX_P;
      uniform float4 _FaceColor;
      uniform float _FaceDilate;
      uniform float _OutlineSoftness;
      uniform float4 _OutlineColor;
      uniform float _OutlineWidth;
      uniform float4x4 _EnvMatrix;
      uniform float _WeightNormal;
      uniform float _WeightBold;
      uniform float _ScaleRatioA;
      uniform float _VertexOffsetX;
      uniform float _VertexOffsetY;
      uniform float _GradientScale;
      uniform float _ScaleX;
      uniform float _ScaleY;
      uniform float _PerspectiveFilter;
      //uniform float4 _Time;
      uniform sampler2D _FaceTex;
      uniform float _FaceUVSpeedX;
      uniform float _FaceUVSpeedY;
      uniform sampler2D _OutlineTex;
      uniform float _OutlineUVSpeedX;
      uniform float _OutlineUVSpeedY;
      uniform float4 _ClipRect;
      uniform float _MaskSoftnessX;
      uniform float _MaskSoftnessY;
      uniform sampler2D _MainTex;
      uniform int _UseClipRect;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_COLOR1 :COLOR1;
          float xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_COLOR1 :COLOR1;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = in_v.color;
          float2 tmpvar_2;
          tmpvar_2 = in_v.texcoord.xy;
          float4 outlineColor_3;
          float4 faceColor_4;
          float opacity_5;
          float weight_6;
          float scale_7;
          float2 pixelSize_8;
          float4 vert_9;
          float tmpvar_10;
          tmpvar_10 = float((0>=in_v.texcoord1.y));
          vert_9.zw = in_v.vertex.zw;
          vert_9.x = (in_v.vertex.x + _VertexOffsetX);
          vert_9.y = (in_v.vertex.y + _VertexOffsetY);
          float4 tmpvar_11;
          tmpvar_11 = UnityObjectToClipPos(vert_9);
          float2 tmpvar_12;
          tmpvar_12.x = _ScaleX;
          tmpvar_12.y = _ScaleY;
          float2x2 tmpvar_13;
          tmpvar_13[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_13[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          pixelSize_8 = (tmpvar_11.ww / (tmpvar_12 * abs(mul(tmpvar_13, _ScreenParams.xy))));
          scale_7 = (rsqrt(dot(pixelSize_8, pixelSize_8)) * ((abs(in_v.texcoord1.y) * _GradientScale) * 1.5));
          if((conv_mxt4x4_3(UNITY_MATRIX_P).w==0))
          {
              float3x3 tmpvar_14;
              tmpvar_14[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
              tmpvar_14[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
              tmpvar_14[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
              scale_7 = lerp((scale_7 * (1 - _PerspectiveFilter)), scale_7, abs(dot(normalize(mul(in_v.normal, tmpvar_14)), normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, vert_9).xyz)))));
          }
          weight_6 = ((lerp(_WeightNormal, _WeightBold, tmpvar_10) / _GradientScale) + ((_FaceDilate * _ScaleRatioA) * 0.5));
          float tmpvar_15;
          tmpvar_15 = tmpvar_1.w;
          opacity_5 = tmpvar_15;
          faceColor_4 = _FaceColor;
          faceColor_4.xyz = (faceColor_4.xyz * in_v.color.xyz);
          faceColor_4.w = (faceColor_4.w * opacity_5);
          outlineColor_3 = _OutlineColor;
          outlineColor_3.w = (outlineColor_3.w * opacity_5);
          float2 tmpvar_16;
          tmpvar_16.x = ((floor(in_v.texcoord1.x) * 5) / 4096);
          tmpvar_16.y = (frac(in_v.texcoord1.x) * 5);
          float4 tmpvar_17;
          tmpvar_17.xy = float2(tmpvar_2);
          tmpvar_17.zw = tmpvar_16;
          float4 tmpvar_18;
          tmpvar_18.x = (((((1 - (_OutlineWidth * _ScaleRatioA)) - (_OutlineSoftness * _ScaleRatioA)) / 2) - (0.5 / scale_7)) - weight_6);
          tmpvar_18.y = scale_7;
          tmpvar_18.z = ((0.5 - weight_6) + (0.5 / scale_7));
          tmpvar_18.w = weight_6;
          float4 tmpvar_19;
          tmpvar_19.xy = vert_9.xy;
          float _tmp_dvx_42 = (0.5 / pixelSize_8);
          tmpvar_19.zw = float2(_tmp_dvx_42, _tmp_dvx_42);
          float3x3 tmpvar_20;
          tmpvar_20[0] = conv_mxt4x4_0(_EnvMatrix).xyz;
          tmpvar_20[1] = conv_mxt4x4_1(_EnvMatrix).xyz;
          tmpvar_20[2] = conv_mxt4x4_2(_EnvMatrix).xyz;
          float4 tmpvar_21;
          float4 tmpvar_22;
          tmpvar_21 = faceColor_4;
          tmpvar_22 = outlineColor_3;
          out_v.vertex = tmpvar_11;
          out_v.xlv_COLOR = tmpvar_21;
          out_v.xlv_COLOR1 = tmpvar_22;
          out_v.xlv_TEXCOORD0 = tmpvar_1.w;
          out_v.xlv_TEXCOORD1 = tmpvar_17;
          out_v.xlv_TEXCOORD2 = tmpvar_18;
          out_v.xlv_TEXCOORD3 = tmpvar_19;
          out_v.xlv_TEXCOORD4 = mul(tmpvar_20, (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, vert_9).xyz));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float2 clipCenter_2;
          float2 clipSize_3;
          float4 outlineColor_4;
          float4 faceColor_5;
          float c_6;
          float tmpvar_7;
          tmpvar_7 = tex2D(_MainTex, in_f.xlv_TEXCOORD1.xy).w;
          c_6 = tmpvar_7;
          float x_8;
          x_8 = (c_6 - in_f.xlv_TEXCOORD2.x);
          if((x_8<0))
          {
              discard;
          }
          float tmpvar_9;
          tmpvar_9 = ((in_f.xlv_TEXCOORD2.z - c_6) * in_f.xlv_TEXCOORD2.y);
          float tmpvar_10;
          tmpvar_10 = ((_OutlineWidth * _ScaleRatioA) * in_f.xlv_TEXCOORD2.y);
          float tmpvar_11;
          tmpvar_11 = ((_OutlineSoftness * _ScaleRatioA) * in_f.xlv_TEXCOORD2.y);
          faceColor_5 = in_f.xlv_COLOR;
          outlineColor_4 = in_f.xlv_COLOR1;
          float2 tmpvar_12;
          tmpvar_12.x = (in_f.xlv_TEXCOORD1.z + (_FaceUVSpeedX * _Time.y));
          tmpvar_12.y = (in_f.xlv_TEXCOORD1.w + (_FaceUVSpeedY * _Time.y));
          float4 tmpvar_13;
          tmpvar_13 = tex2D(_FaceTex, tmpvar_12);
          faceColor_5 = (faceColor_5 * tmpvar_13);
          float2 tmpvar_14;
          tmpvar_14.x = (in_f.xlv_TEXCOORD1.z + (_OutlineUVSpeedX * _Time.y));
          tmpvar_14.y = (in_f.xlv_TEXCOORD1.w + (_OutlineUVSpeedY * _Time.y));
          float4 tmpvar_15;
          tmpvar_15 = tex2D(_OutlineTex, tmpvar_14);
          outlineColor_4 = (outlineColor_4 * tmpvar_15);
          float d_16;
          d_16 = tmpvar_9;
          float4 faceColor_17;
          faceColor_17 = faceColor_5;
          float4 outlineColor_18;
          outlineColor_18 = outlineColor_4;
          float outline_19;
          outline_19 = tmpvar_10;
          float softness_20;
          softness_20 = tmpvar_11;
          float tmpvar_21;
          tmpvar_21 = (1 - clamp((((d_16 - (outline_19 * 0.5)) + (softness_20 * 0.5)) / (1 + softness_20)), 0, 1));
          faceColor_17.xyz = (faceColor_17.xyz * faceColor_17.w);
          outlineColor_18.xyz = (outlineColor_18.xyz * outlineColor_18.w);
          float4 tmpvar_22;
          float _tmp_dvx_43 = (clamp((d_16 + (outline_19 * 0.5)), 0, 1) * sqrt(min(1, outline_19)));
          tmpvar_22 = lerp(faceColor_17, outlineColor_18, float4(_tmp_dvx_43, _tmp_dvx_43, _tmp_dvx_43, _tmp_dvx_43));
          faceColor_17 = tmpvar_22;
          faceColor_17 = (faceColor_17 * tmpvar_21);
          faceColor_5 = faceColor_17;
          float2 tmpvar_23;
          tmpvar_23 = ((_ClipRect.zw - _ClipRect.xy) * 0.5);
          clipSize_3 = tmpvar_23;
          float2 tmpvar_24;
          tmpvar_24 = (_ClipRect.xy + clipSize_3);
          clipCenter_2 = tmpvar_24;
          if(_UseClipRect)
          {
              float2 m_25;
              float2 s_26;
              float2 tmpvar_27;
              tmpvar_27.x = _MaskSoftnessX;
              tmpvar_27.y = _MaskSoftnessY;
              float2 tmpvar_28;
              tmpvar_28 = (tmpvar_27 * in_f.xlv_TEXCOORD3.zw);
              s_26 = tmpvar_28;
              float2 tmpvar_29;
              tmpvar_29 = clamp(((((abs((in_f.xlv_TEXCOORD3.xy - clipCenter_2)) - clipSize_3) * in_f.xlv_TEXCOORD3.zw) + s_26) / (1 + s_26)), 0, 1);
              float2 tmpvar_30;
              float _tmp_dvx_44 = (1 - tmpvar_29);
              tmpvar_30 = float2(_tmp_dvx_44, _tmp_dvx_44);
              m_25 = (tmpvar_30 * tmpvar_30);
              faceColor_5 = (faceColor_5 * (m_25.x * m_25.y));
          }
          tmpvar_1 = faceColor_5;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "TMPro/Mobile/Distance Field"
}
