// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Mobile/Distance Field Overlay"
{
  Properties
  {
    _FaceColor ("Face Color", Color) = (1,1,1,1)
    _FaceDilate ("Face Dilate", Range(-1, 1)) = 0
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
    _OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
    _UnderlayColor ("Border Color", Color) = (0,0,0,0.5)
    _UnderlayOffsetX ("Border OffsetX", Range(-1, 1)) = 0
    _UnderlayOffsetY ("Border OffsetY", Range(-1, 1)) = 0
    _UnderlayDilate ("Border Dilate", Range(-1, 1)) = 0
    _UnderlaySoftness ("Border Softness", Range(0, 1)) = 0
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
    _MaskID ("Mask ID", float) = 0
    _ClipRect ("Mask Coords", Vector) = (0,0,100000,100000)
    _MaskSoftnessX ("Mask SoftnessX", float) = 0
    _MaskSoftnessY ("Mask SoftnessY", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Overlay"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Overlay"
        "RenderType" = "Transparent"
      }
      ZClip Off
      ZTest Always
      ZWrite Off
      Cull Off
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
      uniform float _WeightNormal;
      uniform float _WeightBold;
      uniform float _ScaleRatioA;
      uniform float _VertexOffsetX;
      uniform float _VertexOffsetY;
      uniform float4 _ClipRect;
      uniform float _GradientScale;
      uniform float _ScaleX;
      uniform float _ScaleY;
      uniform float _PerspectiveFilter;
      uniform sampler2D _MainTex;
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
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_COLOR1 :COLOR1;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
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
          float scale_6;
          float2 pixelSize_7;
          float4 vert_8;
          float tmpvar_9;
          tmpvar_9 = float((0>=in_v.texcoord1.y));
          vert_8.zw = in_v.vertex.zw;
          vert_8.x = (in_v.vertex.x + _VertexOffsetX);
          vert_8.y = (in_v.vertex.y + _VertexOffsetY);
          float4 tmpvar_10;
          tmpvar_10 = UnityObjectToClipPos(vert_8);
          float2 tmpvar_11;
          tmpvar_11.x = _ScaleX;
          tmpvar_11.y = _ScaleY;
          float2x2 tmpvar_12;
          tmpvar_12[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_12[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          pixelSize_7 = (tmpvar_10.ww / (tmpvar_11 * abs(mul(tmpvar_12, _ScreenParams.xy))));
          scale_6 = (rsqrt(dot(pixelSize_7, pixelSize_7)) * ((abs(in_v.texcoord1.y) * _GradientScale) * 1.5));
          if((conv_mxt4x4_3(UNITY_MATRIX_P).w==0))
          {
              float3x3 tmpvar_13;
              tmpvar_13[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
              tmpvar_13[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
              tmpvar_13[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
              scale_6 = lerp((abs(scale_6) * (1 - _PerspectiveFilter)), scale_6, abs(dot(normalize(mul(in_v.normal, tmpvar_13)), normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, vert_8).xyz)))));
          }
          scale_6 = (scale_6 / (1 + ((_OutlineSoftness * _ScaleRatioA) * scale_6)));
          float tmpvar_14;
          tmpvar_14 = (((0.5 - ((lerp(_WeightNormal, _WeightBold, tmpvar_9) / _GradientScale) + ((_FaceDilate * _ScaleRatioA) * 0.5))) * scale_6) - 0.5);
          float tmpvar_15;
          tmpvar_15 = ((_OutlineWidth * _ScaleRatioA) * (0.5 * scale_6));
          float tmpvar_16;
          tmpvar_16 = tmpvar_1.w;
          opacity_5 = tmpvar_16;
          float4 tmpvar_17;
          tmpvar_17.xyz = tmpvar_1.xyz;
          tmpvar_17.w = opacity_5;
          float4 tmpvar_18;
          tmpvar_18 = (tmpvar_17 * _FaceColor);
          faceColor_4 = tmpvar_18;
          outlineColor_3.xyz = _OutlineColor.xyz;
          faceColor_4.xyz = (faceColor_4.xyz * faceColor_4.w);
          outlineColor_3.w = (_OutlineColor.w * opacity_5);
          outlineColor_3.xyz = (_OutlineColor.xyz * outlineColor_3.w);
          float4 tmpvar_19;
          float _tmp_dvx_38 = sqrt(min(1, (tmpvar_15 * 2)));
          tmpvar_19 = lerp(faceColor_4, outlineColor_3, float4(_tmp_dvx_38, _tmp_dvx_38, _tmp_dvx_38, _tmp_dvx_38));
          outlineColor_3 = tmpvar_19;
          float4 tmpvar_20;
          tmpvar_20.x = scale_6;
          tmpvar_20.y = (tmpvar_14 - tmpvar_15);
          tmpvar_20.z = (tmpvar_14 + tmpvar_15);
          tmpvar_20.w = tmpvar_14;
          float4 tmpvar_21;
          tmpvar_21.xy = (vert_8.xy - _ClipRect.xy);
          float _tmp_dvx_39 = (0.5 / pixelSize_7);
          tmpvar_21.zw = float2(_tmp_dvx_39, _tmp_dvx_39);
          float4 tmpvar_22;
          float4 tmpvar_23;
          tmpvar_22 = tmpvar_20;
          tmpvar_23 = tmpvar_21;
          out_v.vertex = tmpvar_10;
          out_v.xlv_COLOR = faceColor_4;
          out_v.xlv_COLOR1 = outlineColor_3;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_22;
          out_v.xlv_TEXCOORD2 = tmpvar_23;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float tmpvar_2;
          tmpvar_2 = (tmpvar_1.w * in_f.xlv_TEXCOORD1.x);
          float tmpvar_3;
          tmpvar_3 = clamp((tmpvar_2 - in_f.xlv_TEXCOORD1.z), 0, 1);
          float tmpvar_4;
          tmpvar_4 = clamp((tmpvar_2 - in_f.xlv_TEXCOORD1.y), 0, 1);
          float4 tmpvar_5;
          tmpvar_5 = (lerp(in_f.xlv_COLOR1, in_f.xlv_COLOR, float4(tmpvar_3, tmpvar_3, tmpvar_3, tmpvar_3)) * tmpvar_4);
          out_f.color = tmpvar_5;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
