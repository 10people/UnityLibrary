Shader "Custom/DisappearSurfaceShader" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Speed("Speed",Float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		float _Speed;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;

			o.Alpha = c.r;
			//if (c.r + c.g + c.a < _Time.y*_Speed)
			//{
			//	o.Alpha = 0;
			//}
			//else
			//{
			//	o.Alpha = c.a;
			//}
		}
		ENDCG
	}
		FallBack "Diffuse"
}
