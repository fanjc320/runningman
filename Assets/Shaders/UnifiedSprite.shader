// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnifiedSprite"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _EffectTex ("Effect Texture", 2D) = "white" {}
    _EffectStrength ("Effect Strength", Range(1, 10)) = 10
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _ColorMask ("Color Mask", float) = 15
    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "IGNOREPROJECTOR" = "true"
        "PreviewType" = "Plane"
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
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask 0
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _Color;
      uniform float _EffectStrength;
      uniform sampler2D _EffectTex;
      uniform float4 _TextureSampleAdd;
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
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 tmpvar_1;
          tmpvar_1 = in_v.normal;
          float2 tmpvar_2;
          tmpvar_2 = in_v.texcoord.xy;
          float2 tmpvar_3;
          tmpvar_3 = in_v.texcoord1.xy;
          float fracTime_4;
          float4 tmpvar_5;
          float2 tmpvar_6;
          float4 tmpvar_7;
          float4 tmpvar_8;
          tmpvar_6 = tmpvar_2;
          tmpvar_5 = (in_v.color * _Color);
          tmpvar_7.x = tmpvar_1.z;
          tmpvar_7.y = tmpvar_3.y;
          tmpvar_7.z = tmpvar_3.x;
          float tmpvar_9;
          tmpvar_9 = frac(_Time.y);
          fracTime_4 = tmpvar_9;
          float2 tmpvar_10;
          tmpvar_10.x = (in_v.normal.x + fracTime_4);
          tmpvar_10.y = tmpvar_1.y;
          tmpvar_8.xy = float2(tmpvar_10);
          tmpvar_8.z = tmpvar_1.x;
          float tmpvar_11;
          tmpvar_11 = max(max(tmpvar_5.x, tmpvar_5.y), tmpvar_5.z);
          tmpvar_8.w = tmpvar_11;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_COLOR = tmpvar_5;
          out_v.xlv_TEXCOORD0 = tmpvar_6;
          out_v.xlv_TEXCOORD1 = tmpvar_7;
          out_v.xlv_TEXCOORD2 = tmpvar_8;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 highlightFinal_2;
          float shinyMask_3;
          float4 color_4;
          float4 texCol_5;
          float4 tmpvar_6;
          tmpvar_6 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) + _TextureSampleAdd);
          texCol_5 = tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = tex2D(_EffectTex, in_f.xlv_TEXCOORD2.xy).w;
          shinyMask_3 = tmpvar_7;
          float3 tmpvar_8;
          tmpvar_8 = in_f.xlv_COLOR.xyz;
          highlightFinal_2 = tmpvar_8;
          float tmpvar_9;
          tmpvar_9 = (1 - dot(in_f.xlv_TEXCOORD1.xyz, in_f.xlv_TEXCOORD1.xyz));
          float _tmp_dvx_47 = dot(texCol_5.xyz, float3(0.33, 0.33, 0.33));
          color_4.xyz = ((((lerp(texCol_5.xyz, lerp((texCol_5.xyz + ((texCol_5.xyz * in_f.xlv_TEXCOORD2.w) * _EffectStrength)), in_f.xlv_COLOR.xyz, in_f.xlv_COLOR.www), float3(shinyMask_3, shinyMask_3, shinyMask_3)) * in_f.xlv_TEXCOORD1.x) + (lerp(float3(_tmp_dvx_47, _tmp_dvx_47, _tmp_dvx_47), texCol_5.xyz, in_f.xlv_TEXCOORD2.zzz) * in_f.xlv_TEXCOORD1.y)) + (highlightFinal_2 * in_f.xlv_TEXCOORD1.z)) + ((texCol_5.xyz * in_f.xlv_COLOR.xyz) * tmpvar_9));
          color_4.w = lerp(lerp(texCol_5.w, (texCol_5.w * in_f.xlv_COLOR.w), in_f.xlv_TEXCOORD1.z), (texCol_5.w * in_f.xlv_COLOR.w), tmpvar_9);
          tmpvar_1 = color_4;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
