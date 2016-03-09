Shader "Hidden/VagueShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			half2 _MainTex_TexelSize;
			half4 _offset_xy;

			static const half config[5] = { 0.1,0.2,0.4,0.2,0.1 };

			fixed4 frag(v2f v) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,0);


			//col = tex2D(_MainTex, v.uv);
				for (int i = 0; i < 5; i++)
				{
					col += tex2D(_MainTex, v.uv + (i - 2)*_offset_xy*_MainTex_TexelSize)*config[i];
				}

				return col;
			}
		ENDCG
	}
	}
}
