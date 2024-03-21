// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Distance Field (Surface)"
{
  Properties
  {
    _FaceTex ("Fill Texture", 2D) = "white" {}
    _FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0
    _FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0
    _FaceColor ("Fill Color", Color) = (1,1,1,1)
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
    _BumpMap ("Normalmap", 2D) = "bump" {}
    _BumpOutline ("Bump Outline", Range(0, 1)) = 0.5
    _BumpFace ("Bump Face", Range(0, 1)) = 0.5
    _ReflectFaceColor ("Face Color", Color) = (0,0,0,1)
    _ReflectOutlineColor ("Outline Color", Color) = (0,0,0,1)
    _Cube ("Reflection Cubemap", Cube) = "black" {}
    _EnvMatrixRotation ("Texture Rotation", Vector) = (0,0,0,0)
    _SpecColor ("Specular Color", Color) = (0,0,0,1)
    _FaceShininess ("Face Shininess", Range(0, 1)) = 0
    _OutlineShininess ("Outline Shininess", Range(0, 1)) = 0
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
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 300
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 300
      ZClip Off
      ZWrite Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ScreenParams;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 UNITY_MATRIX_P;
      uniform float _FaceDilate;
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
      uniform float4 _MainTex_ST;
      uniform float4 _FaceTex_ST;
      //uniform float4 _Time;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      uniform float4 _LightColor0;
      uniform float4 _SpecColor;
      uniform sampler2D _FaceTex;
      uniform float _FaceUVSpeedX;
      uniform float _FaceUVSpeedY;
      uniform float4 _FaceColor;
      uniform float _OutlineSoftness;
      uniform sampler2D _OutlineTex;
      uniform float _OutlineUVSpeedX;
      uniform float _OutlineUVSpeedY;
      uniform float4 _OutlineColor;
      uniform float _OutlineWidth;
      uniform float _Bevel;
      uniform float _BevelOffset;
      uniform float _BevelWidth;
      uniform float _BevelClamp;
      uniform float _BevelRoundness;
      uniform sampler2D _BumpMap;
      uniform float _BumpOutline;
      uniform float _BumpFace;
      uniform samplerCUBE _Cube;
      uniform float4 _ReflectFaceColor;
      uniform float4 _ReflectOutlineColor;
      uniform float _ShaderFlags;
      uniform sampler2D _MainTex;
      uniform float _TextureWidth;
      uniform float _TextureHeight;
      uniform float _FaceShininess;
      uniform float _OutlineShininess;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD4 :TEXCOORD4;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 xlv_TEXCOORD3 :TEXCOORD3;
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD4 :TEXCOORD4;
          float3 xlv_TEXCOORD5 :TEXCOORD5;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float tangentSign_1;
          float3 worldTangent_2;
          float3 worldNormal_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          float3 tmpvar_6;
          tmpvar_5.zw = in_v.vertex.zw;
          float2 tmpvar_7;
          float scale_8;
          float2 pixelSize_9;
          tmpvar_5.x = (in_v.vertex.x + _VertexOffsetX);
          tmpvar_5.y = (in_v.vertex.y + _VertexOffsetY);
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = float3(_WorldSpaceCameraPos);
          tmpvar_6 = (in_v.normal * sign(dot(in_v.normal, (mul(unity_WorldToObject, tmpvar_10).xyz - tmpvar_5.xyz))));
          float2 tmpvar_11;
          tmpvar_11.x = _ScaleX;
          tmpvar_11.y = _ScaleY;
          float2x2 tmpvar_12;
          tmpvar_12[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_12[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          pixelSize_9 = (UnityObjectToClipPos(tmpvar_5).ww / (tmpvar_11 * mul(tmpvar_12, _ScreenParams.xy)));
          scale_8 = (rsqrt(dot(pixelSize_9, pixelSize_9)) * ((abs(in_v.texcoord1.y) * _GradientScale) * 1.5));
          float3x3 tmpvar_13;
          tmpvar_13[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_13[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_13[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float tmpvar_14;
          tmpvar_14 = lerp((scale_8 * (1 - _PerspectiveFilter)), scale_8, abs(dot(normalize(mul(tmpvar_6, tmpvar_13)), normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, tmpvar_5).xyz)))));
          scale_8 = tmpvar_14;
          tmpvar_7.y = tmpvar_14;
          tmpvar_7.x = ((lerp(_WeightNormal, _WeightBold, float((0>=in_v.texcoord1.y))) / _GradientScale) + ((_FaceDilate * _ScaleRatioA) * 0.5));
          float2 tmpvar_15;
          tmpvar_15.x = ((floor(in_v.texcoord1.x) * 5) / 4096);
          tmpvar_15.y = (frac(in_v.texcoord1.x) * 5);
          float3x3 tmpvar_16;
          tmpvar_16[0] = conv_mxt4x4_0(_EnvMatrix).xyz;
          tmpvar_16[1] = conv_mxt4x4_1(_EnvMatrix).xyz;
          tmpvar_16[2] = conv_mxt4x4_2(_EnvMatrix).xyz;
          float4 tmpvar_17;
          tmpvar_17.w = 1;
          tmpvar_17.xyz = tmpvar_5.xyz;
          tmpvar_4.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          tmpvar_4.zw = TRANSFORM_TEX(tmpvar_15, _FaceTex);
          float3 tmpvar_18;
          tmpvar_18 = mul(unity_ObjectToWorld, tmpvar_5).xyz;
          float3x3 tmpvar_19;
          tmpvar_19[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_19[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_19[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_20;
          tmpvar_20 = normalize(mul(tmpvar_6, tmpvar_19));
          worldNormal_3 = tmpvar_20;
          float3x3 tmpvar_21;
          tmpvar_21[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_21[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_21[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_22;
          tmpvar_22 = normalize(mul(tmpvar_21, in_v.tangent.xyz));
          worldTangent_2 = tmpvar_22;
          float tmpvar_23;
          tmpvar_23 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_1 = tmpvar_23;
          float3 tmpvar_24;
          tmpvar_24 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
          float4 tmpvar_25;
          tmpvar_25.x = worldTangent_2.x;
          tmpvar_25.y = tmpvar_24.x;
          tmpvar_25.z = worldNormal_3.x;
          tmpvar_25.w = tmpvar_18.x;
          float4 tmpvar_26;
          tmpvar_26.x = worldTangent_2.y;
          tmpvar_26.y = tmpvar_24.y;
          tmpvar_26.z = worldNormal_3.y;
          tmpvar_26.w = tmpvar_18.y;
          float4 tmpvar_27;
          tmpvar_27.x = worldTangent_2.z;
          tmpvar_27.y = tmpvar_24.z;
          tmpvar_27.z = worldNormal_3.z;
          tmpvar_27.w = tmpvar_18.z;
          float3 normal_28;
          normal_28 = worldNormal_3;
          float3 x1_29;
          float4 tmpvar_30;
          tmpvar_30 = (normal_28.xyzz * normal_28.yzzx);
          x1_29.x = dot(unity_SHBr, tmpvar_30);
          x1_29.y = dot(unity_SHBg, tmpvar_30);
          x1_29.z = dot(unity_SHBb, tmpvar_30);
          out_v.vertex = UnityObjectToClipPos(tmpvar_17);
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_25;
          out_v.xlv_TEXCOORD2 = tmpvar_26;
          out_v.xlv_TEXCOORD3 = tmpvar_27;
          out_v.xlv_COLOR0 = in_v.color;
          out_v.xlv_TEXCOORD4 = tmpvar_7;
          out_v.xlv_TEXCOORD5 = mul(tmpvar_16, (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, tmpvar_5).xyz));
          out_v.xlv_TEXCOORD6 = (x1_29 + (unity_SHC.xyz * ((normal_28.x * normal_28.x) - (normal_28.y * normal_28.y))));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 tmpvar_1;
          float3 tmpvar_2;
          float3 worldN_3;
          float4 c_4;
          float3 tmpvar_5;
          float3 tmpvar_6;
          float3 tmpvar_7;
          float tmpvar_8;
          float tmpvar_9;
          float3 worldViewDir_10;
          float3 lightDir_11;
          float3 tmpvar_12;
          tmpvar_12.x = in_f.xlv_TEXCOORD1.w;
          tmpvar_12.y = in_f.xlv_TEXCOORD2.w;
          tmpvar_12.z = in_f.xlv_TEXCOORD3.w;
          float3 tmpvar_13;
          tmpvar_13 = _WorldSpaceLightPos0.xyz;
          lightDir_11 = tmpvar_13;
          float3 tmpvar_14;
          tmpvar_14 = normalize((_WorldSpaceCameraPos - tmpvar_12));
          worldViewDir_10 = tmpvar_14;
          tmpvar_5 = float3(0, 0, 0);
          tmpvar_7 = float3(0, 0, 0);
          tmpvar_9 = 0;
          tmpvar_8 = 0;
          float3 tmpvar_15;
          float3 tmpvar_16;
          float3 tmpvar_17;
          float tmpvar_18;
          float tmpvar_19;
          tmpvar_15 = tmpvar_5;
          tmpvar_16 = tmpvar_6;
          tmpvar_17 = tmpvar_7;
          tmpvar_18 = tmpvar_8;
          tmpvar_19 = tmpvar_9;
          float3 bump_20;
          float4 outlineColor_21;
          float4 faceColor_22;
          float c_23;
          float4 smp4x_24;
          float3 tmpvar_25;
          tmpvar_25.z = 0;
          tmpvar_25.x = (1 / _TextureWidth);
          tmpvar_25.y = (1 / _TextureHeight);
          float2 P_26;
          P_26 = (in_f.xlv_TEXCOORD0.xy - tmpvar_25.xz);
          float2 P_27;
          P_27 = (in_f.xlv_TEXCOORD0.xy + tmpvar_25.xz);
          float2 P_28;
          P_28 = (in_f.xlv_TEXCOORD0.xy - tmpvar_25.zy);
          float2 P_29;
          P_29 = (in_f.xlv_TEXCOORD0.xy + tmpvar_25.zy);
          float4 tmpvar_30;
          tmpvar_30.x = tex2D(_MainTex, P_26).w;
          tmpvar_30.y = tex2D(_MainTex, P_27).w;
          tmpvar_30.z = tex2D(_MainTex, P_28).w;
          tmpvar_30.w = tex2D(_MainTex, P_29).w;
          smp4x_24 = tmpvar_30;
          float tmpvar_31;
          tmpvar_31 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).w;
          c_23 = tmpvar_31;
          float tmpvar_32;
          tmpvar_32 = ((((0.5 - c_23) - in_f.xlv_TEXCOORD4.x) * in_f.xlv_TEXCOORD4.y) + 0.5);
          float tmpvar_33;
          tmpvar_33 = ((_OutlineWidth * _ScaleRatioA) * in_f.xlv_TEXCOORD4.y);
          float tmpvar_34;
          tmpvar_34 = ((_OutlineSoftness * _ScaleRatioA) * in_f.xlv_TEXCOORD4.y);
          faceColor_22 = _FaceColor;
          outlineColor_21 = _OutlineColor;
          faceColor_22 = (faceColor_22 * in_f.xlv_COLOR0);
          outlineColor_21.w = (outlineColor_21.w * in_f.xlv_COLOR0.w);
          float2 tmpvar_35;
          tmpvar_35.x = (in_f.xlv_TEXCOORD0.z + (_FaceUVSpeedX * _Time.y));
          tmpvar_35.y = (in_f.xlv_TEXCOORD0.w + (_FaceUVSpeedY * _Time.y));
          float4 tmpvar_36;
          tmpvar_36 = tex2D(_FaceTex, tmpvar_35);
          faceColor_22 = (faceColor_22 * tmpvar_36);
          float2 tmpvar_37;
          tmpvar_37.x = (in_f.xlv_TEXCOORD0.z + (_OutlineUVSpeedX * _Time.y));
          tmpvar_37.y = (in_f.xlv_TEXCOORD0.w + (_OutlineUVSpeedY * _Time.y));
          float4 tmpvar_38;
          tmpvar_38 = tex2D(_OutlineTex, tmpvar_37);
          outlineColor_21 = (outlineColor_21 * tmpvar_38);
          float d_39;
          d_39 = tmpvar_32;
          float4 faceColor_40;
          faceColor_40 = faceColor_22;
          float4 outlineColor_41;
          outlineColor_41 = outlineColor_21;
          float outline_42;
          outline_42 = tmpvar_33;
          float softness_43;
          softness_43 = tmpvar_34;
          float tmpvar_44;
          tmpvar_44 = (1 - clamp((((d_39 - (outline_42 * 0.5)) + (softness_43 * 0.5)) / (1 + softness_43)), 0, 1));
          faceColor_40.xyz = (faceColor_40.xyz * faceColor_40.w);
          outlineColor_41.xyz = (outlineColor_41.xyz * outlineColor_41.w);
          float4 tmpvar_45;
          float _tmp_dvx_30 = (clamp((d_39 + (outline_42 * 0.5)), 0, 1) * sqrt(min(1, outline_42)));
          tmpvar_45 = lerp(faceColor_40, outlineColor_41, float4(_tmp_dvx_30, _tmp_dvx_30, _tmp_dvx_30, _tmp_dvx_30));
          faceColor_40 = tmpvar_45;
          faceColor_40 = (faceColor_40 * tmpvar_44);
          faceColor_22 = faceColor_40;
          faceColor_22.xyz = (faceColor_22.xyz / faceColor_22.w);
          float4 h_46;
          h_46 = smp4x_24;
          float tmpvar_47;
          tmpvar_47 = (_ShaderFlags / 2);
          float tmpvar_48;
          tmpvar_48 = (frac(abs(tmpvar_47)) * 2);
          float tmpvar_49;
          if((tmpvar_47>=0))
          {
              tmpvar_49 = tmpvar_48;
          }
          else
          {
              tmpvar_49 = (-tmpvar_48);
          }
          h_46 = (smp4x_24 + (in_f.xlv_TEXCOORD4.x + _BevelOffset));
          float tmpvar_50;
          tmpvar_50 = max(0.01, (_OutlineWidth + _BevelWidth));
          h_46 = (h_46 - 0.5);
          h_46 = (h_46 / tmpvar_50);
          float4 tmpvar_51;
          tmpvar_51 = clamp((h_46 + 0.5), 0, 1);
          h_46 = tmpvar_51;
          if(int(float((tmpvar_49>=1))))
          {
              float _tmp_dvx_31 = (1 - abs(((tmpvar_51 * 2) - 1)));
              h_46 = float4(_tmp_dvx_31, _tmp_dvx_31, _tmp_dvx_31, _tmp_dvx_31);
          }
          float _tmp_dvx_32 = (1 - _BevelClamp);
          h_46 = (min(lerp(h_46, sin(((h_46 * 3.141592) / 2)), float4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)), float4(_tmp_dvx_32, _tmp_dvx_32, _tmp_dvx_32, _tmp_dvx_32)) * ((_Bevel * tmpvar_50) * (_GradientScale * (-2))));
          float3 tmpvar_52;
          tmpvar_52.xy = float2(1, 0);
          tmpvar_52.z = (h_46.y - h_46.x);
          float3 tmpvar_53;
          tmpvar_53 = normalize(tmpvar_52);
          float3 tmpvar_54;
          tmpvar_54.xy = float2(0, (-1));
          tmpvar_54.z = (h_46.w - h_46.z);
          float3 tmpvar_55;
          tmpvar_55 = normalize(tmpvar_54);
          float3 tmpvar_56;
          tmpvar_56 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
          bump_20 = tmpvar_56;
          bump_20 = (bump_20 * lerp(_BumpFace, _BumpOutline, clamp((tmpvar_32 + (tmpvar_33 * 0.5)), 0, 1)));
          float3 tmpvar_57;
          tmpvar_57 = lerp(float3(0, 0, 1), bump_20, faceColor_22.www);
          bump_20 = tmpvar_57;
          float3 tmpvar_58;
          tmpvar_58 = normalize((((tmpvar_53.yzx * tmpvar_55.zxy) - (tmpvar_53.zxy * tmpvar_55.yzx)) - tmpvar_57));
          float3x3 tmpvar_59;
          tmpvar_59[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_59[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_59[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_60;
          float3 N_61;
          N_61 = mul(tmpvar_59, tmpvar_58);
          tmpvar_60 = (in_f.xlv_TEXCOORD5 - (2 * (dot(N_61, in_f.xlv_TEXCOORD5) * N_61)));
          float4 tmpvar_62;
          float _tmp_dvx_33 = texCUBE(_Cube, tmpvar_60);
          tmpvar_62 = float4(_tmp_dvx_33, _tmp_dvx_33, _tmp_dvx_33, _tmp_dvx_33);
          float tmpvar_63;
          tmpvar_63 = clamp((tmpvar_32 + (tmpvar_33 * 0.5)), 0, 1);
          float3 tmpvar_64;
          tmpvar_64 = lerp(_ReflectFaceColor.xyz, _ReflectOutlineColor.xyz, float3(tmpvar_63, tmpvar_63, tmpvar_63));
          float3 tmpvar_65;
          tmpvar_65 = ((tmpvar_62.xyz * tmpvar_64) * faceColor_22.w);
          tmpvar_15 = faceColor_22.xyz;
          tmpvar_16 = (-tmpvar_58);
          tmpvar_17 = tmpvar_65;
          float tmpvar_66;
          tmpvar_66 = clamp((tmpvar_32 + (tmpvar_33 * 0.5)), 0, 1);
          tmpvar_18 = 1;
          tmpvar_19 = faceColor_22.w;
          tmpvar_5 = tmpvar_15;
          tmpvar_7 = tmpvar_17;
          tmpvar_8 = tmpvar_18;
          tmpvar_9 = tmpvar_19;
          float tmpvar_67;
          tmpvar_67 = dot(in_f.xlv_TEXCOORD1.xyz, tmpvar_16);
          worldN_3.x = tmpvar_67;
          float tmpvar_68;
          tmpvar_68 = dot(in_f.xlv_TEXCOORD2.xyz, tmpvar_16);
          worldN_3.y = tmpvar_68;
          float tmpvar_69;
          tmpvar_69 = dot(in_f.xlv_TEXCOORD3.xyz, tmpvar_16);
          worldN_3.z = tmpvar_69;
          tmpvar_6 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_11;
          float3 normalWorld_70;
          normalWorld_70 = worldN_3;
          float4 tmpvar_71;
          tmpvar_71.w = 1;
          tmpvar_71.xyz = float3(normalWorld_70);
          float3 x_72;
          x_72.x = dot(unity_SHAr, tmpvar_71);
          x_72.y = dot(unity_SHAg, tmpvar_71);
          x_72.z = dot(unity_SHAb, tmpvar_71);
          float3 tmpvar_73;
          tmpvar_73 = max(((1.055 * pow(max(float3(0, 0, 0), (in_f.xlv_TEXCOORD6 + x_72)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          float3 viewDir_74;
          viewDir_74 = worldViewDir_10;
          float4 c_75;
          float4 c_76;
          float nh_77;
          float diff_78;
          float tmpvar_79;
          tmpvar_79 = max(0, dot(worldN_3, tmpvar_2));
          diff_78 = tmpvar_79;
          float tmpvar_80;
          tmpvar_80 = max(0, dot(worldN_3, normalize((tmpvar_2 + viewDir_74))));
          nh_77 = tmpvar_80;
          float y_81;
          y_81 = (lerp(_FaceShininess, _OutlineShininess, tmpvar_66) * 128);
          float tmpvar_82;
          tmpvar_82 = pow(nh_77, y_81);
          c_76.xyz = (((tmpvar_15 * tmpvar_1) * diff_78) + ((tmpvar_1 * _SpecColor.xyz) * tmpvar_82));
          c_76.w = tmpvar_19;
          c_75.w = c_76.w;
          c_75.xyz = (c_76.xyz + (tmpvar_15 * tmpvar_73));
          c_4.w = c_75.w;
          c_4.xyz = (c_75.xyz + tmpvar_17);
          out_f.color = c_4;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "ForwardAdd"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 300
      ZClip Off
      ZWrite Off
      Cull Off
      Blend SrcAlpha One
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile POINT
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ScreenParams;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 UNITY_MATRIX_P;
      uniform float _FaceDilate;
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
      uniform float4 _MainTex_ST;
      uniform float4 _FaceTex_ST;
      //uniform float4 _Time;
      //uniform float4 _WorldSpaceLightPos0;
      uniform float4 _LightColor0;
      uniform float4 _SpecColor;
      uniform sampler2D _LightTexture0;
      uniform float4x4 unity_WorldToLight;
      uniform sampler2D _FaceTex;
      uniform float _FaceUVSpeedX;
      uniform float _FaceUVSpeedY;
      uniform float4 _FaceColor;
      uniform float _OutlineSoftness;
      uniform sampler2D _OutlineTex;
      uniform float _OutlineUVSpeedX;
      uniform float _OutlineUVSpeedY;
      uniform float4 _OutlineColor;
      uniform float _OutlineWidth;
      uniform float _Bevel;
      uniform float _BevelOffset;
      uniform float _BevelWidth;
      uniform float _BevelClamp;
      uniform float _BevelRoundness;
      uniform sampler2D _BumpMap;
      uniform float _BumpOutline;
      uniform float _BumpFace;
      uniform samplerCUBE _Cube;
      uniform float4 _ReflectFaceColor;
      uniform float4 _ReflectOutlineColor;
      uniform float _ShaderFlags;
      uniform sampler2D _MainTex;
      uniform float _TextureWidth;
      uniform float _TextureHeight;
      uniform float _FaceShininess;
      uniform float _OutlineShininess;
      struct appdata_t
      {
          float4 tangent :TANGENT;
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD5 :TEXCOORD5;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float3 xlv_TEXCOORD3 :TEXCOORD3;
          float3 xlv_TEXCOORD4 :TEXCOORD4;
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD5 :TEXCOORD5;
          float3 xlv_TEXCOORD6 :TEXCOORD6;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float tangentSign_1;
          float3 worldTangent_2;
          float3 worldNormal_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          float3 tmpvar_6;
          tmpvar_5.zw = in_v.vertex.zw;
          float2 tmpvar_7;
          float scale_8;
          float2 pixelSize_9;
          tmpvar_5.x = (in_v.vertex.x + _VertexOffsetX);
          tmpvar_5.y = (in_v.vertex.y + _VertexOffsetY);
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = float3(_WorldSpaceCameraPos);
          tmpvar_6 = (in_v.normal * sign(dot(in_v.normal, (mul(unity_WorldToObject, tmpvar_10).xyz - tmpvar_5.xyz))));
          float2 tmpvar_11;
          tmpvar_11.x = _ScaleX;
          tmpvar_11.y = _ScaleY;
          float2x2 tmpvar_12;
          tmpvar_12[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_12[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          pixelSize_9 = (UnityObjectToClipPos(tmpvar_5).ww / (tmpvar_11 * mul(tmpvar_12, _ScreenParams.xy)));
          scale_8 = (rsqrt(dot(pixelSize_9, pixelSize_9)) * ((abs(in_v.texcoord1.y) * _GradientScale) * 1.5));
          float3x3 tmpvar_13;
          tmpvar_13[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_13[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_13[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float tmpvar_14;
          tmpvar_14 = lerp((scale_8 * (1 - _PerspectiveFilter)), scale_8, abs(dot(normalize(mul(tmpvar_6, tmpvar_13)), normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, tmpvar_5).xyz)))));
          scale_8 = tmpvar_14;
          tmpvar_7.y = tmpvar_14;
          tmpvar_7.x = ((lerp(_WeightNormal, _WeightBold, float((0>=in_v.texcoord1.y))) / _GradientScale) + ((_FaceDilate * _ScaleRatioA) * 0.5));
          float2 tmpvar_15;
          tmpvar_15.x = ((floor(in_v.texcoord1.x) * 5) / 4096);
          tmpvar_15.y = (frac(in_v.texcoord1.x) * 5);
          float3x3 tmpvar_16;
          tmpvar_16[0] = conv_mxt4x4_0(_EnvMatrix).xyz;
          tmpvar_16[1] = conv_mxt4x4_1(_EnvMatrix).xyz;
          tmpvar_16[2] = conv_mxt4x4_2(_EnvMatrix).xyz;
          float4 tmpvar_17;
          tmpvar_17.w = 1;
          tmpvar_17.xyz = tmpvar_5.xyz;
          tmpvar_4.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          tmpvar_4.zw = TRANSFORM_TEX(tmpvar_15, _FaceTex);
          float3x3 tmpvar_18;
          tmpvar_18[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_18[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_18[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_19;
          tmpvar_19 = normalize(mul(tmpvar_6, tmpvar_18));
          worldNormal_3 = tmpvar_19;
          float3x3 tmpvar_20;
          tmpvar_20[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_20[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_20[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_21;
          tmpvar_21 = normalize(mul(tmpvar_20, in_v.tangent.xyz));
          worldTangent_2 = tmpvar_21;
          float tmpvar_22;
          tmpvar_22 = (in_v.tangent.w * unity_WorldTransformParams.w);
          tangentSign_1 = tmpvar_22;
          float3 tmpvar_23;
          tmpvar_23 = (((worldNormal_3.yzx * worldTangent_2.zxy) - (worldNormal_3.zxy * worldTangent_2.yzx)) * tangentSign_1);
          float3 tmpvar_24;
          tmpvar_24.x = worldTangent_2.x;
          tmpvar_24.y = tmpvar_23.x;
          tmpvar_24.z = worldNormal_3.x;
          float3 tmpvar_25;
          tmpvar_25.x = worldTangent_2.y;
          tmpvar_25.y = tmpvar_23.y;
          tmpvar_25.z = worldNormal_3.y;
          float3 tmpvar_26;
          tmpvar_26.x = worldTangent_2.z;
          tmpvar_26.y = tmpvar_23.z;
          tmpvar_26.z = worldNormal_3.z;
          out_v.vertex = UnityObjectToClipPos(tmpvar_17);
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          out_v.xlv_TEXCOORD1 = tmpvar_24;
          out_v.xlv_TEXCOORD2 = tmpvar_25;
          out_v.xlv_TEXCOORD3 = tmpvar_26;
          out_v.xlv_TEXCOORD4 = mul(unity_ObjectToWorld, tmpvar_5).xyz;
          out_v.xlv_COLOR0 = in_v.color;
          out_v.xlv_TEXCOORD5 = tmpvar_7;
          out_v.xlv_TEXCOORD6 = mul(tmpvar_16, (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, tmpvar_5).xyz));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float3 tmpvar_1;
          float3 tmpvar_2;
          float3 worldN_3;
          float3 tmpvar_4;
          float3 tmpvar_5;
          float3 tmpvar_6;
          float tmpvar_7;
          float tmpvar_8;
          float3 worldViewDir_9;
          float3 lightDir_10;
          float3 tmpvar_11;
          tmpvar_11 = normalize((_WorldSpaceLightPos0.xyz - in_f.xlv_TEXCOORD4));
          lightDir_10 = tmpvar_11;
          float3 tmpvar_12;
          tmpvar_12 = normalize((_WorldSpaceCameraPos - in_f.xlv_TEXCOORD4));
          worldViewDir_9 = tmpvar_12;
          tmpvar_4 = float3(0, 0, 0);
          tmpvar_6 = float3(0, 0, 0);
          tmpvar_8 = 0;
          tmpvar_7 = 0;
          float3 tmpvar_13;
          float3 tmpvar_14;
          float3 tmpvar_15;
          float tmpvar_16;
          float tmpvar_17;
          tmpvar_13 = tmpvar_4;
          tmpvar_14 = tmpvar_5;
          tmpvar_15 = tmpvar_6;
          tmpvar_16 = tmpvar_7;
          tmpvar_17 = tmpvar_8;
          float3 bump_18;
          float4 outlineColor_19;
          float4 faceColor_20;
          float c_21;
          float4 smp4x_22;
          float3 tmpvar_23;
          tmpvar_23.z = 0;
          tmpvar_23.x = (1 / _TextureWidth);
          tmpvar_23.y = (1 / _TextureHeight);
          float2 P_24;
          P_24 = (in_f.xlv_TEXCOORD0.xy - tmpvar_23.xz);
          float2 P_25;
          P_25 = (in_f.xlv_TEXCOORD0.xy + tmpvar_23.xz);
          float2 P_26;
          P_26 = (in_f.xlv_TEXCOORD0.xy - tmpvar_23.zy);
          float2 P_27;
          P_27 = (in_f.xlv_TEXCOORD0.xy + tmpvar_23.zy);
          float4 tmpvar_28;
          tmpvar_28.x = tex2D(_MainTex, P_24).w;
          tmpvar_28.y = tex2D(_MainTex, P_25).w;
          tmpvar_28.z = tex2D(_MainTex, P_26).w;
          tmpvar_28.w = tex2D(_MainTex, P_27).w;
          smp4x_22 = tmpvar_28;
          float tmpvar_29;
          tmpvar_29 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).w;
          c_21 = tmpvar_29;
          float tmpvar_30;
          tmpvar_30 = ((((0.5 - c_21) - in_f.xlv_TEXCOORD5.x) * in_f.xlv_TEXCOORD5.y) + 0.5);
          float tmpvar_31;
          tmpvar_31 = ((_OutlineWidth * _ScaleRatioA) * in_f.xlv_TEXCOORD5.y);
          float tmpvar_32;
          tmpvar_32 = ((_OutlineSoftness * _ScaleRatioA) * in_f.xlv_TEXCOORD5.y);
          faceColor_20 = _FaceColor;
          outlineColor_19 = _OutlineColor;
          faceColor_20 = (faceColor_20 * in_f.xlv_COLOR0);
          outlineColor_19.w = (outlineColor_19.w * in_f.xlv_COLOR0.w);
          float2 tmpvar_33;
          tmpvar_33.x = (in_f.xlv_TEXCOORD0.z + (_FaceUVSpeedX * _Time.y));
          tmpvar_33.y = (in_f.xlv_TEXCOORD0.w + (_FaceUVSpeedY * _Time.y));
          float4 tmpvar_34;
          tmpvar_34 = tex2D(_FaceTex, tmpvar_33);
          faceColor_20 = (faceColor_20 * tmpvar_34);
          float2 tmpvar_35;
          tmpvar_35.x = (in_f.xlv_TEXCOORD0.z + (_OutlineUVSpeedX * _Time.y));
          tmpvar_35.y = (in_f.xlv_TEXCOORD0.w + (_OutlineUVSpeedY * _Time.y));
          float4 tmpvar_36;
          tmpvar_36 = tex2D(_OutlineTex, tmpvar_35);
          outlineColor_19 = (outlineColor_19 * tmpvar_36);
          float d_37;
          d_37 = tmpvar_30;
          float4 faceColor_38;
          faceColor_38 = faceColor_20;
          float4 outlineColor_39;
          outlineColor_39 = outlineColor_19;
          float outline_40;
          outline_40 = tmpvar_31;
          float softness_41;
          softness_41 = tmpvar_32;
          float tmpvar_42;
          tmpvar_42 = (1 - clamp((((d_37 - (outline_40 * 0.5)) + (softness_41 * 0.5)) / (1 + softness_41)), 0, 1));
          faceColor_38.xyz = (faceColor_38.xyz * faceColor_38.w);
          outlineColor_39.xyz = (outlineColor_39.xyz * outlineColor_39.w);
          float4 tmpvar_43;
          float _tmp_dvx_34 = (clamp((d_37 + (outline_40 * 0.5)), 0, 1) * sqrt(min(1, outline_40)));
          tmpvar_43 = lerp(faceColor_38, outlineColor_39, float4(_tmp_dvx_34, _tmp_dvx_34, _tmp_dvx_34, _tmp_dvx_34));
          faceColor_38 = tmpvar_43;
          faceColor_38 = (faceColor_38 * tmpvar_42);
          faceColor_20 = faceColor_38;
          faceColor_20.xyz = (faceColor_20.xyz / faceColor_20.w);
          float4 h_44;
          h_44 = smp4x_22;
          float tmpvar_45;
          tmpvar_45 = (_ShaderFlags / 2);
          float tmpvar_46;
          tmpvar_46 = (frac(abs(tmpvar_45)) * 2);
          float tmpvar_47;
          if((tmpvar_45>=0))
          {
              tmpvar_47 = tmpvar_46;
          }
          else
          {
              tmpvar_47 = (-tmpvar_46);
          }
          h_44 = (smp4x_22 + (in_f.xlv_TEXCOORD5.x + _BevelOffset));
          float tmpvar_48;
          tmpvar_48 = max(0.01, (_OutlineWidth + _BevelWidth));
          h_44 = (h_44 - 0.5);
          h_44 = (h_44 / tmpvar_48);
          float4 tmpvar_49;
          tmpvar_49 = clamp((h_44 + 0.5), 0, 1);
          h_44 = tmpvar_49;
          if(int(float((tmpvar_47>=1))))
          {
              float _tmp_dvx_35 = (1 - abs(((tmpvar_49 * 2) - 1)));
              h_44 = float4(_tmp_dvx_35, _tmp_dvx_35, _tmp_dvx_35, _tmp_dvx_35);
          }
          float _tmp_dvx_36 = (1 - _BevelClamp);
          h_44 = (min(lerp(h_44, sin(((h_44 * 3.141592) / 2)), float4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)), float4(_tmp_dvx_36, _tmp_dvx_36, _tmp_dvx_36, _tmp_dvx_36)) * ((_Bevel * tmpvar_48) * (_GradientScale * (-2))));
          float3 tmpvar_50;
          tmpvar_50.xy = float2(1, 0);
          tmpvar_50.z = (h_44.y - h_44.x);
          float3 tmpvar_51;
          tmpvar_51 = normalize(tmpvar_50);
          float3 tmpvar_52;
          tmpvar_52.xy = float2(0, (-1));
          tmpvar_52.z = (h_44.w - h_44.z);
          float3 tmpvar_53;
          tmpvar_53 = normalize(tmpvar_52);
          float3 tmpvar_54;
          tmpvar_54 = ((tex2D(_BumpMap, in_f.xlv_TEXCOORD0.zw).xyz * 2) - 1);
          bump_18 = tmpvar_54;
          bump_18 = (bump_18 * lerp(_BumpFace, _BumpOutline, clamp((tmpvar_30 + (tmpvar_31 * 0.5)), 0, 1)));
          float3 tmpvar_55;
          tmpvar_55 = lerp(float3(0, 0, 1), bump_18, faceColor_20.www);
          bump_18 = tmpvar_55;
          float3 tmpvar_56;
          tmpvar_56 = normalize((((tmpvar_51.yzx * tmpvar_53.zxy) - (tmpvar_51.zxy * tmpvar_53.yzx)) - tmpvar_55));
          float3x3 tmpvar_57;
          tmpvar_57[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_57[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_57[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          float3 tmpvar_58;
          float3 N_59;
          N_59 = mul(tmpvar_57, tmpvar_56);
          tmpvar_58 = (in_f.xlv_TEXCOORD6 - (2 * (dot(N_59, in_f.xlv_TEXCOORD6) * N_59)));
          float4 tmpvar_60;
          float _tmp_dvx_37 = texCUBE(_Cube, tmpvar_58);
          tmpvar_60 = float4(_tmp_dvx_37, _tmp_dvx_37, _tmp_dvx_37, _tmp_dvx_37);
          float tmpvar_61;
          tmpvar_61 = clamp((tmpvar_30 + (tmpvar_31 * 0.5)), 0, 1);
          float3 tmpvar_62;
          tmpvar_62 = lerp(_ReflectFaceColor.xyz, _ReflectOutlineColor.xyz, float3(tmpvar_61, tmpvar_61, tmpvar_61));
          float3 tmpvar_63;
          tmpvar_63 = ((tmpvar_60.xyz * tmpvar_62) * faceColor_20.w);
          tmpvar_13 = faceColor_20.xyz;
          tmpvar_14 = (-tmpvar_56);
          tmpvar_15 = tmpvar_63;
          float tmpvar_64;
          tmpvar_64 = clamp((tmpvar_30 + (tmpvar_31 * 0.5)), 0, 1);
          tmpvar_16 = 1;
          tmpvar_17 = faceColor_20.w;
          tmpvar_4 = tmpvar_13;
          tmpvar_6 = tmpvar_15;
          tmpvar_7 = tmpvar_16;
          tmpvar_8 = tmpvar_17;
          float4 tmpvar_65;
          tmpvar_65.w = 1;
          tmpvar_65.xyz = in_f.xlv_TEXCOORD4;
          float3 tmpvar_66;
          tmpvar_66 = mul(unity_WorldToLight, tmpvar_65).xyz;
          float tmpvar_67;
          tmpvar_67 = dot(tmpvar_66, tmpvar_66);
          float tmpvar_68;
          tmpvar_68 = tex2D(_LightTexture0, float2(tmpvar_67, tmpvar_67)).w;
          worldN_3.x = dot(in_f.xlv_TEXCOORD1, tmpvar_14);
          worldN_3.y = dot(in_f.xlv_TEXCOORD2, tmpvar_14);
          worldN_3.z = dot(in_f.xlv_TEXCOORD3, tmpvar_14);
          tmpvar_5 = worldN_3;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_10;
          tmpvar_1 = (tmpvar_1 * tmpvar_68);
          float3 viewDir_69;
          viewDir_69 = worldViewDir_9;
          float4 c_70;
          float4 c_71;
          float nh_72;
          float diff_73;
          float tmpvar_74;
          tmpvar_74 = max(0, dot(worldN_3, tmpvar_2));
          diff_73 = tmpvar_74;
          float tmpvar_75;
          tmpvar_75 = max(0, dot(worldN_3, normalize((tmpvar_2 + viewDir_69))));
          nh_72 = tmpvar_75;
          float y_76;
          y_76 = (lerp(_FaceShininess, _OutlineShininess, tmpvar_64) * 128);
          float tmpvar_77;
          tmpvar_77 = pow(nh_72, y_76);
          c_71.xyz = (((tmpvar_13 * tmpvar_1) * diff_73) + ((tmpvar_1 * _SpecColor.xyz) * tmpvar_77));
          c_71.w = tmpvar_17;
          c_70.w = c_71.w;
          c_70.xyz = c_71.xyz;
          out_f.color = c_70;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 3, name: CASTER
    {
      Name "CASTER"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      LOD 300
      ZClip Off
      Cull Off
      Offset 1, 1
      Fog
      { 
        Mode  Off
      } 
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      uniform float _OutlineWidth;
      uniform float _FaceDilate;
      uniform float _ScaleRatioA;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2.w = 1;
          tmpvar_2.xyz = in_v.vertex.xyz;
          tmpvar_1 = UnityObjectToClipPos(tmpvar_2);
          float4 clipPos_3;
          clipPos_3.xyw = tmpvar_1.xyw;
          clipPos_3.z = (tmpvar_1.z + clamp((unity_LightShadowBias.x / tmpvar_1.w), 0, 1));
          clipPos_3.z = lerp(clipPos_3.z, max(clipPos_3.z, (-tmpvar_1.w)), unity_LightShadowBias.y);
          out_v.vertex = clipPos_3;
          out_v.xlv_TEXCOORD1 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD2 = (((1 - (_OutlineWidth * _ScaleRatioA)) - (_FaceDilate * _ScaleRatioA)) / 2);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = tex2D(_MainTex, in_f.xlv_TEXCOORD1).wwww;
          float x_2;
          x_2 = (tmpvar_1.w - in_f.xlv_TEXCOORD2);
          if((x_2<0))
          {
              discard;
          }
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
