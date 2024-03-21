// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/LightLoop"
{
  Properties
  {
    _Center ("Center", Vector) = (0,0,0,0)
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
      Blend SrcAlpha One
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4 _Time;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 tmpvar_3;
          float2 tmpvar_4;
          tmpvar_4 = tmpvar_2;
          tmpvar_3 = tmpvar_1;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_COLOR = tmpvar_3;
          out_v.xlv_TEXCOORD0 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 HSV_2;
          float4 color_3;
          float4 tmpvar_4;
          tmpvar_4 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR);
          color_3.w = tmpvar_4.w;
          float3 tmpvar_5;
          tmpvar_5.yz = float2(1, 1);
          tmpvar_5.x = _Time.x;
          HSV_2 = tmpvar_5;
          float3 tmpvar_6;
          tmpvar_6 = lerp(float3(1, 1, 1), clamp((abs(((frac((HSV_2.xxx + float3(1, 0.6666667, 0.3333333))) * 6) - float3(3, 3, 3))) - float3(1, 1, 1)), float3(0, 0, 0), float3(1, 1, 1)), HSV_2.yyy);
          color_3.xyz = (HSV_2.z * tmpvar_6);
          color_3.w = (color_3.w * in_f.xlv_COLOR.w);
          tmpvar_1 = color_3;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
