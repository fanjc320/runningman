// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Bitmap"
{
  Properties
  {
    _MainTex ("Font Atlas", 2D) = "white" {}
    _FaceTex ("Font Texture", 2D) = "white" {}
    _FaceColor ("Text Color", Color) = (1,1,1,1)
    _VertexOffsetX ("Vertex OffsetX", float) = 0
    _VertexOffsetY ("Vertex OffsetY", float) = 0
    _ClipRect ("Mask Coords", Vector) = (0,0,100000,100000)
    _MaskSoftnessX ("Mask SoftnessX", float) = 0
    _MaskSoftnessY ("Mask SoftnessY", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZClip Off
      ZWrite Off
      Cull Off
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _ScreenParams;
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 UNITY_MATRIX_P;
      uniform float4 _FaceColor;
      uniform float _VertexOffsetX;
      uniform float _VertexOffsetY;
      uniform int _UseClipRect;
      uniform sampler2D _MainTex;
      uniform sampler2D _FaceTex;
      uniform float4 _ClipRect;
      uniform float _MaskSoftnessX;
      uniform float _MaskSoftnessY;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 vert_1;
          vert_1.zw = in_v.vertex.zw;
          vert_1.x = (in_v.vertex.x + _VertexOffsetX);
          vert_1.y = (in_v.vertex.y + _VertexOffsetY);
          float4 pos_2;
          pos_2 = UnityObjectToClipPos(vert_1);
          float2 tmpvar_3;
          tmpvar_3 = (_ScreenParams.xy * 0.5);
          pos_2.xy = ((floor((((pos_2.xy / pos_2.w) * tmpvar_3) + float2(0.5, 0.5))) / tmpvar_3) * pos_2.w);
          float2 tmpvar_4;
          tmpvar_4.x = ((floor(in_v.texcoord1.x) * 4) / 4096);
          tmpvar_4.y = (frac(in_v.texcoord1.x) * 4);
          float2 tmpvar_5;
          tmpvar_5.x = (_ScreenParams.x * conv_mxt4x4_0(UNITY_MATRIX_P).x);
          tmpvar_5.y = (_ScreenParams.y * conv_mxt4x4_1(UNITY_MATRIX_P).y);
          float4 tmpvar_6;
          tmpvar_6.xy = vert_1.xy;
          tmpvar_6.zw = (0.5 / (pos_2.ww / abs(tmpvar_5)));
          out_v.vertex = pos_2;
          out_v.xlv_COLOR = (in_v.color * _FaceColor);
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.xlv_TEXCOORD1 = tmpvar_4;
          out_v.xlv_TEXCOORD2 = tmpvar_6;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 c_1;
          float4 tmpvar_2;
          tmpvar_2 = (tex2D(_FaceTex, in_f.xlv_TEXCOORD1) * in_f.xlv_COLOR);
          c_1.xyz = tmpvar_2.xyz;
          c_1.w = (tmpvar_2.w * tex2D(_MainTex, in_f.xlv_TEXCOORD0).w);
          float2 tmpvar_3;
          tmpvar_3 = ((_ClipRect.zw - _ClipRect.xy) * 0.5);
          float2 tmpvar_4;
          tmpvar_4 = (_ClipRect.xy + tmpvar_3);
          if(_UseClipRect)
          {
              float2 m_5;
              float2 tmpvar_6;
              tmpvar_6.x = _MaskSoftnessX;
              tmpvar_6.y = _MaskSoftnessY;
              float2 tmpvar_7;
              tmpvar_7 = (tmpvar_6 * in_f.xlv_TEXCOORD2.zw);
              float2 tmpvar_8;
              tmpvar_8 = (1 - clamp(((((abs((in_f.xlv_TEXCOORD2.xy - tmpvar_4)) - tmpvar_3) * in_f.xlv_TEXCOORD2.zw) + tmpvar_7) / (1 + tmpvar_7)), 0, 1));
              m_5 = (tmpvar_8 * tmpvar_8);
              c_1 = (c_1 * (m_5.x * m_5.y));
          }
          out_f.color = c_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
