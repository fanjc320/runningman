// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distorted/Unlit, Rim, InjectOutline + Transparent"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Alpha ("Alpha", float) = 0.5
    _ToonTex ("ToonTex (RGB)", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "LIGHTMODE" = "ForwardBase"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "LIGHTMODE" = "ForwardBase"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZClip Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile LIGHTMAP_OFF
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_WorldToObject;
      uniform float4 _MainTex_ST;
      uniform float4 _Distort;
      uniform float4 _SkyGradientTopColor;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _FogSilhouetteColor;
      uniform float _SkyGradientOffset;
      uniform sampler2D _MainTex;
      uniform sampler2D _ToonTex;
      uniform float4 _OutlineColor;
      uniform float _RimPower;
      uniform float _RimStrength;
      uniform float4 _RimColor;
      uniform float _Alpha;
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
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float4 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 tmpvar_1;
          float4 tmpvar_2;
          float4 tmpvar_3;
          float4 tmpvar_4;
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          float _tmp_dvx_50 = (tmpvar_1.z * tmpvar_1.z);
          tmpvar_1.xy = (tmpvar_1.xy + (float2(_tmp_dvx_50, _tmp_dvx_50) * _Distort.xy));
          float tmpvar_5;
          float tmpvar_6;
          tmpvar_6 = clamp((((tmpvar_1.y / tmpvar_1.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_5 = tmpvar_6;
          tmpvar_3.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(tmpvar_5, tmpvar_5, tmpvar_5));
          float tmpvar_7;
          float tmpvar_8;
          tmpvar_8 = clamp(((1000 - tmpvar_1.w) / 300), 0, 1);
          tmpvar_7 = tmpvar_8;
          tmpvar_3.xyz = lerp(tmpvar_3.xyz, _FogSilhouetteColor.xyz, float3(tmpvar_7, tmpvar_7, tmpvar_7));
          float tmpvar_9;
          tmpvar_9 = clamp(((700 - tmpvar_1.w) / 500), 0, 1);
          tmpvar_3.w = tmpvar_9;
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = float3(_WorldSpaceCameraPos);
          tmpvar_2.z = ((dot(normalize(in_v.normal), normalize((mul(unity_WorldToObject, tmpvar_10).xyz - in_v.vertex.xyz))) * (-0.5)) + 0.5);
          float3x3 tmpvar_11;
          tmpvar_11[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_11[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_11[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          tmpvar_2.w = clamp(dot(normalize(mul(in_v.normal, tmpvar_11)), _WorldSpaceLightPos0.xyz), 0, 1);
          tmpvar_4.w = clamp(in_v.texcoord1.x, 0, 1);
          out_v.vertex = tmpvar_1;
          out_v.xlv_COLOR = in_v.color;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          out_v.xlv_TEXCOORD2 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 c_1;
          float4 tooncol_2;
          float2 tmpvar_3;
          tmpvar_3.y = 0;
          tmpvar_3.x = in_f.xlv_TEXCOORD0.w;
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_ToonTex, tmpvar_3);
          tooncol_2 = tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          c_1.w = tmpvar_5.w;
          float3 tmpvar_6;
          float3 x_7;
          x_7 = (tmpvar_5.xyz * tooncol_2.x);
          tmpvar_6 = lerp(x_7, (((tmpvar_5.xyz * tooncol_2.x) * 0.5) + ((_RimColor.xyz * pow(in_f.xlv_TEXCOORD0.z, _RimPower)) * _RimStrength)), in_f.xlv_TEXCOORD0.zzz);
          c_1.xyz = float3(tmpvar_6);
          float3 tmpvar_8;
          tmpvar_8 = lerp(c_1.xyz, lerp(in_f.xlv_COLOR.xyz, _OutlineColor.xyz, in_f.xlv_COLOR.www), in_f.xlv_TEXCOORD2.www);
          c_1.xyz = float3(tmpvar_8);
          c_1.xyz = lerp(in_f.xlv_TEXCOORD1.xyz, c_1.xyz, in_f.xlv_TEXCOORD1.www);
          c_1.w = _Alpha;
          out_f.color = c_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
