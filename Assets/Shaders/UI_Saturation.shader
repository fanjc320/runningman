// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Saturation"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _ColorMask ("Color Mask", float) = 15
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
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _Color;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 tmpvar_1;
          tmpvar_1 = in_v.texcoord.xy;
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.xy = float2(tmpvar_1);
          tmpvar_2 = (in_v.color * _Color);
          float4 tmpvar_4;
          tmpvar_4.w = 0;
          tmpvar_4.xy = tmpvar_3.xy;
          tmpvar_4.z = in_v.normal.x;
          tmpvar_3 = tmpvar_4;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_COLOR = tmpvar_2;
          out_v.xlv_TEXCOORD0 = tmpvar_3;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 color_2;
          float4 tmpvar_3;
          tmpvar_3 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy) * in_f.xlv_COLOR);
          color_2 = tmpvar_3;
          float x_4;
          x_4 = (color_2.w - 0.01);
          if((x_4<0))
          {
              discard;
          }
          float3 RGB_5;
          RGB_5 = color_2.xyz;
          float satu_6;
          satu_6 = in_f.xlv_TEXCOORD0.z;
          float3 RESULT_7;
          float tmpvar_8;
          tmpvar_8 = (0.9999999 * satu_6);
          float tmpvar_9;
          tmpvar_9 = (0.0003046174 * satu_6);
          RESULT_7.x = (((((0.299 + (0.701 * tmpvar_8)) + (0.168 * tmpvar_9)) * RGB_5.x) + (((0.587 - (0.587 * tmpvar_8)) + (0.33 * tmpvar_9)) * RGB_5.y)) + (((0.114 - (0.114 * tmpvar_8)) - (0.497 * tmpvar_9)) * RGB_5.z));
          RESULT_7.y = (((((0.299 - (0.299 * tmpvar_8)) - (0.328 * tmpvar_9)) * RGB_5.x) + (((0.587 + (0.413 * tmpvar_8)) + (0.035 * tmpvar_9)) * RGB_5.y)) + (((0.114 - (0.114 * tmpvar_8)) + (0.292 * tmpvar_9)) * RGB_5.z));
          RESULT_7.z = (((((0.299 - (0.3 * tmpvar_8)) + (1.25 * tmpvar_9)) * RGB_5.x) + (((0.587 - (0.588 * tmpvar_8)) - (1.05 * tmpvar_9)) * RGB_5.y)) + (((0.114 + (0.886 * tmpvar_8)) - (0.203 * tmpvar_9)) * RGB_5.z));
          color_2.xyz = float3(RESULT_7);
          tmpvar_1 = color_2;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
