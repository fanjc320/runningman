// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NametagDirectorBG_1"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Alpha ("Alpha", Range(0, 1)) = 0.5
    _TunnelLength ("_TunnelLength", Range(0, 800)) = 80
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Transparent"
      }
      ZClip Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile LIGHTMAP_OFF
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      uniform float _TunnelLength;
      uniform sampler2D _MainTex;
      uniform float _Alpha;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
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
          float4 xlv_TEXCOORD0 :TEXCOORD0;
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
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_2.z = ((((tmpvar_1.z * 2) + _TunnelLength) * 0.5) / _TunnelLength);
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          tmpvar_2.x = (tmpvar_2.x + (_Time.y * 0.1));
          tmpvar_2.y = (tmpvar_2.y - (_Time.y * 0.2));
          out_v.vertex = tmpvar_1;
          out_v.xlv_COLOR = in_v.color;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 c_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          float4 tmpvar_3;
          tmpvar_3 = (tmpvar_2 * in_f.xlv_TEXCOORD0.z);
          c_1 = tmpvar_3;
          float shift_4;
          shift_4 = (36 * _Time.y);
          float3 RESULT_5;
          float tmpvar_6;
          tmpvar_6 = cos(((shift_4 * 3.141593) / 180));
          float tmpvar_7;
          tmpvar_7 = sin(((shift_4 * 3.141593) / 180));
          RESULT_5.x = ((0.701 * tmpvar_6) + (0.168 * tmpvar_7));
          RESULT_5.y = ((-0.299 * tmpvar_6) - (0.328 * tmpvar_7));
          RESULT_5.z = ((-0.3 * tmpvar_6) + (1.25 * tmpvar_7));
          c_1.xyz = (c_1.xyz * RESULT_5);
          c_1.xyz = clamp(c_1.xyz, 0, 1);
          c_1.w = _Alpha;
          out_f.color = c_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
