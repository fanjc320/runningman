// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distorted/Unlit"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
      }
      ZClip Off
      Cull Front
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 UNITY_MATRIX_IT_MV;
      //uniform float4x4 UNITY_MATRIX_P;
      uniform float4 _Distort;
      uniform float _Outline;
      uniform float4 _OutlineColor;
      uniform float4 _SkyGradientTopColor;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _FogSilhouetteColor;
      uniform float _SkyGradientOffset;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
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
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          float _tmp_dvx_51 = (tmpvar_1.z * tmpvar_1.z);
          tmpvar_1.xy = (tmpvar_1.xy + (float2(_tmp_dvx_51, _tmp_dvx_51) * _Distort.xy));
          float3x3 tmpvar_4;
          tmpvar_4[0] = conv_mxt4x4_0(UNITY_MATRIX_IT_MV).xyz;
          tmpvar_4[1] = conv_mxt4x4_1(UNITY_MATRIX_IT_MV).xyz;
          tmpvar_4[2] = conv_mxt4x4_2(UNITY_MATRIX_IT_MV).xyz;
          float2x2 tmpvar_5;
          tmpvar_5[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_5[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          tmpvar_1.xy = (tmpvar_1.xy + (mul(mul(mul(tmpvar_5, normalize(mul(tmpvar_4, in_v.normal)).xy), tmpvar_1.z), _Outline) / sqrt(((tmpvar_1.z * tmpvar_1.z) * 0.0001))));
          float tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = clamp((((tmpvar_1.y / tmpvar_1.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_6 = tmpvar_7;
          tmpvar_2.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(tmpvar_6, tmpvar_6, tmpvar_6));
          float tmpvar_8;
          float tmpvar_9;
          tmpvar_9 = clamp(((1000 - tmpvar_1.w) / 300), 0, 1);
          tmpvar_8 = tmpvar_9;
          tmpvar_2.xyz = lerp(tmpvar_2.xyz, _FogSilhouetteColor.xyz, float3(tmpvar_8, tmpvar_8, tmpvar_8));
          float tmpvar_10;
          tmpvar_10 = clamp(((700 - tmpvar_1.w) / 500), 0, 1);
          tmpvar_2.w = tmpvar_10;
          tmpvar_3 = _OutlineColor;
          out_v.vertex = tmpvar_1;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_COLOR = tmpvar_3;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = in_f.xlv_COLOR;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: 
    {
      Tags
      { 
      }
      ZClip Off
      Fog
      { 
        Mode  Off
      } 
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_WorldToObject;
      uniform float4 _Distort;
      uniform float4 _MainTex_ST;
      uniform float4 _SkyGradientTopColor;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _FogSilhouetteColor;
      uniform float _SkyGradientOffset;
      uniform sampler2D _MainTex;
      uniform sampler2D _RimTex;
      uniform float4 _RimColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
          tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
          float tmpvar_4;
          float tmpvar_5;
          tmpvar_5 = clamp((((tmpvar_1.y / tmpvar_1.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_4 = tmpvar_5;
          tmpvar_3.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(tmpvar_4, tmpvar_4, tmpvar_4));
          float tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = clamp(((1000 - tmpvar_1.w) / 300), 0, 1);
          tmpvar_6 = tmpvar_7;
          tmpvar_3.xyz = lerp(tmpvar_3.xyz, _FogSilhouetteColor.xyz, float3(tmpvar_6, tmpvar_6, tmpvar_6));
          float tmpvar_8;
          tmpvar_8 = clamp(((700 - tmpvar_1.w) / 500), 0, 1);
          tmpvar_3.w = tmpvar_8;
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = float3(_WorldSpaceCameraPos);
          tmpvar_2.z = (((dot(in_v.normal, normalize((mul(unity_WorldToObject, tmpvar_9).xyz - in_v.vertex.xyz))) + 1) * 0.5) - 0.5);
          out_v.vertex = tmpvar_1;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 rimcol_1;
          float4 c_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          c_2.w = tmpvar_3.w;
          float2 tmpvar_4;
          tmpvar_4.y = 0;
          tmpvar_4.x = in_f.xlv_TEXCOORD0.z;
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_RimTex, tmpvar_4);
          rimcol_1 = tmpvar_5;
          float3 tmpvar_6;
          float3 y_7;
          y_7 = (tmpvar_3.xyz + (_RimColor.xyz * 0.5));
          tmpvar_6 = lerp(tmpvar_3.xyz, y_7, rimcol_1.xxx);
          c_2.xyz = float3(tmpvar_6);
          c_2.xyz = lerp(in_f.xlv_TEXCOORD1.xyz, c_2.xyz, in_f.xlv_TEXCOORD1.www);
          out_f.color = c_2;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
