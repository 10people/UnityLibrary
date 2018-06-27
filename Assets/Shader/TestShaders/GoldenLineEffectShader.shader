// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GoldenLineEffectShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Speed("Swift Speed",Float) = 1
	_GoldenWidth("GoldenWidth",Range(0,1)) = 0.1
		_GoldenColor("GoldenColor",Color) = (0,0,0,0)
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			fixed _Speed;
			half _GoldenWidth;
			half4 _GoldenColor;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

			if ((i.uv.x * 2 + i.uv.y * 3) < fmod(_Time.y*_Speed, 5) + _GoldenWidth && (i.uv.x * 2 + i.uv.y * 3) > fmod(_Time.y*_Speed, 5) - _GoldenWidth)
			{
				return _GoldenColor;
			}
			else
			{
				return col;
			}
		}
		ENDCG
	}
	}
}
