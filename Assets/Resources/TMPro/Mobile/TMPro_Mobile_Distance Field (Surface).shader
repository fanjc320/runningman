// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Mobile/Distance Field (Surface)"
{
  Properties
  {
    _FaceTex ("Fill Texture", 2D) = "white" {}
    _FaceColor ("Fill Color", Color) = (1,1,1,1)
    _FaceDilate ("Face Dilate", Range(-1, 1)) = 0
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _OutlineTex ("Outline Texture", 2D) = "white" {}
    _OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
    _OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
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
      uniform sampler2D _MainTex;
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
          float3 lightDir_5;
          float3 tmpvar_6;
          tmpvar_6 = _WorldSpaceLightPos0.xyz;
          lightDir_5 = tmpvar_6;
          float3 tmpvar_7;
          float tmpvar_8;
          float4 outlineColor_9;
          float4 faceColor_10;
          float c_11;
          float tmpvar_12;
          tmpvar_12 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy).w;
          c_11 = tmpvar_12;
          float tmpvar_13;
          tmpvar_13 = ((((0.5 - c_11) - in_f.xlv_TEXCOORD4.x) * in_f.xlv_TEXCOORD4.y) + 0.5);
          float tmpvar_14;
          tmpvar_14 = ((_OutlineWidth * _ScaleRatioA) * in_f.xlv_TEXCOORD4.y);
          float tmpvar_15;
          tmpvar_15 = ((_OutlineSoftness * _ScaleRatioA) * in_f.xlv_TEXCOORD4.y);
          faceColor_10 = _FaceColor;
          outlineColor_9 = _OutlineColor;
          faceColor_10 = (faceColor_10 * in_f.xlv_COLOR0);
          outlineColor_9.w = (outlineColor_9.w * in_f.xlv_COLOR0.w);
          float2 tmpvar_16;
          tmpvar_16.x = (in_f.xlv_TEXCOORD0.z + (_FaceUVSpeedX * _Time.y));
          tmpvar_16.y = (in_f.xlv_TEXCOORD0.w + (_FaceUVSpeedY * _Time.y));
          float4 tmpvar_17;
          tmpvar_17 = tex2D(_FaceTex, tmpvar_16);
          faceColor_10 = (faceColor_10 * tmpvar_17);
          float2 tmpvar_18;
          tmpvar_18.x = (in_f.xlv_TEXCOORD0.z + (_OutlineUVSpeedX * _Time.y));
          tmpvar_18.y = (in_f.xlv_TEXCOORD0.w + (_OutlineUVSpeedY * _Time.y));
          float4 tmpvar_19;
          tmpvar_19 = tex2D(_OutlineTex, tmpvar_18);
          outlineColor_9 = (outlineColor_9 * tmpvar_19);
          float d_20;
          d_20 = tmpvar_13;
          float4 faceColor_21;
          faceColor_21 = faceColor_10;
          float4 outlineColor_22;
          outlineColor_22 = outlineColor_9;
          float outline_23;
          outline_23 = tmpvar_14;
          float softness_24;
          softness_24 = tmpvar_15;
          float tmpvar_25;
          tmpvar_25 = (1 - clamp((((d_20 - (outline_23 * 0.5)) + (softness_24 * 0.5)) / (1 + softness_24)), 0, 1));
          faceColor_21.xyz = (faceColor_21.xyz * faceColor_21.w);
          outlineColor_22.xyz = (outlineColor_22.xyz * outlineColor_22.w);
          float4 tmpvar_26;
          float _tmp_dvx_26 = (clamp((d_20 + (outline_23 * 0.5)), 0, 1) * sqrt(min(1, outline_23)));
          tmpvar_26 = lerp(faceColor_21, outlineColor_22, float4(_tmp_dvx_26, _tmp_dvx_26, _tmp_dvx_26, _tmp_dvx_26));
          faceColor_21 = tmpvar_26;
          faceColor_21 = (faceColor_21 * tmpvar_25);
          faceColor_10 = faceColor_21;
          faceColor_10.xyz = (faceColor_10.xyz / faceColor_10.w);
          tmpvar_7 = faceColor_10.xyz;
          tmpvar_8 = faceColor_10.w;
          float tmpvar_27;
          tmpvar_27 = in_f.xlv_TEXCOORD1.z;
          worldN_3.x = tmpvar_27;
          float tmpvar_28;
          tmpvar_28 = in_f.xlv_TEXCOORD2.z;
          worldN_3.y = tmpvar_28;
          float tmpvar_29;
          tmpvar_29 = in_f.xlv_TEXCOORD3.z;
          worldN_3.z = tmpvar_29;
          tmpvar_1 = _LightColor0.xyz;
          tmpvar_2 = lightDir_5;
          float3 normalWorld_30;
          normalWorld_30 = worldN_3;
          float4 tmpvar_31;
          tmpvar_31.w = 1;
          tmpvar_31.xyz = float3(normalWorld_30);
          float3 x_32;
          x_32.x = dot(unity_SHAr, tmpvar_31);
          x_32.y = dot(unity_SHAg, tmpvar_31);
          x_32.z = dot(unity_SHAb, tmpvar_31);
          float3 tmpvar_33;
          tmpvar_33 = max(((1.055 * pow(max(float3(0, 0, 0), (in_f.xlv_TEXCOORD6 + x_32)), float3(0.4166667, 0.4166667, 0.4166667))) - 0.055), float3(0, 0, 0));
          float4 c_34;
          float4 c_35;
          float diff_36;
          float tmpvar_37;
          tmpvar_37 = max(0, dot(worldN_3, tmpvar_2));
          diff_36 = tmpvar_37;
          c_35.xyz = float3(((tmpvar_7 * tmpvar_1) * diff_36));
          c_35.w = tmpvar_8;
          c_34.w = c_35.w;
          c_34.xyz = (c_35.xyz + (tmpvar_7 * tmpvar_33));
          c_4.w = c_34.w;
          c_4.xyz = c_34.xyz;
          out_f.color = c_4;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: CASTER
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
