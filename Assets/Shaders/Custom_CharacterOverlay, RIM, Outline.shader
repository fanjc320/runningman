// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CharacterOverlay, RIM, Outline"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _ToonTex ("ToonTex (RGB)", 2D) = "white" {}
    _RimColor ("RimColor", Color) = (1,1,1,1)
    _RimStrength ("RimStrength", float) = 1
    _RimPower ("RimPower", float) = 1
    _OutlineColor ("OutlineColor", Color) = (1,1,1,1)
    _Outline ("Outline", float) = 1
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
      #pragma multi_compile LIGHTMAP_OFF
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
      uniform float _Outline;
      uniform float4 _OutlineColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
      };
      
      struct OUT_Data_Vert
      {
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
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          float3x3 tmpvar_3;
          tmpvar_3[0] = conv_mxt4x4_0(UNITY_MATRIX_IT_MV).xyz;
          tmpvar_3[1] = conv_mxt4x4_1(UNITY_MATRIX_IT_MV).xyz;
          tmpvar_3[2] = conv_mxt4x4_2(UNITY_MATRIX_IT_MV).xyz;
          float2x2 tmpvar_4;
          tmpvar_4[0] = conv_mxt4x4_0(UNITY_MATRIX_P).xy;
          tmpvar_4[1] = conv_mxt4x4_1(UNITY_MATRIX_P).xy;
          tmpvar_1.xy = (tmpvar_1.xy + mul(mul(mul(mul(tmpvar_4, normalize(mul(tmpvar_3, in_v.normal)).xy), tmpvar_1.z), _Outline), 3));
          tmpvar_2 = _OutlineColor;
          out_v.vertex = tmpvar_1;
          out_v.xlv_COLOR = tmpvar_2;
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
        "LIGHTMODE" = "ForwardBase"
      }
      ZClip Off
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
      uniform sampler2D unity_NHxRoughness;
      uniform sampler2D _MainTex;
      uniform sampler2D _ToonTex;
      uniform float _RimPower;
      uniform float _RimStrength;
      uniform float4 _RimColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
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
          tmpvar_2.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          float3x3 tmpvar_3;
          tmpvar_3[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_3[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_3[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_4;
          tmpvar_4 = normalize(mul(in_v.normal, tmpvar_3));
          tmpvar_2.z = (1 - clamp(dot(tmpvar_4, _WorldSpaceCameraPos), 0, 1));
          tmpvar_2.w = clamp(dot(tmpvar_4, _WorldSpaceLightPos0.xyz), 0, 1);
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_COLOR = tmpvar_1;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 texcol_1;
          float4 tooncol_2;
          float2 tmpvar_3;
          tmpvar_3.y = 0;
          tmpvar_3.x = in_f.xlv_TEXCOORD0.w;
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_ToonTex, tmpvar_3);
          tooncol_2 = tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD0.xy);
          texcol_1 = tmpvar_5;
          float2 tmpvar_6;
          tmpvar_6.y = 0.6;
          tmpvar_6.x = pow(in_f.xlv_TEXCOORD0.z, (_RimPower * 0.25));
          float4 tmpvar_7;
          tmpvar_7 = tex2D(unity_NHxRoughness, tmpvar_6);
          float tmpvar_8;
          tmpvar_8 = (tmpvar_7.w * 16);
          float3 tmpvar_9;
          float3 x_10;
          x_10 = (texcol_1.xyz * tooncol_2.x);
          tmpvar_9 = lerp(x_10, (((texcol_1.xyz * tooncol_2.x) * 0.5) + ((_RimColor.xyz * tmpvar_8) * _RimStrength)), in_f.xlv_TEXCOORD0.zzz);
          texcol_1.xyz = float3(tmpvar_9);
          out_f.color = texcol_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
