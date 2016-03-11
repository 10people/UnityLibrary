Shader "Custom/DepthSurfaceShader" {
	Properties{
	}
	SubShader{
		Tags {
		"RenderType" = "Opaque"
		"Queue" = "Overlay"
	}
		LOD 200

		ZWrite Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert noforwardadded

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = (1,1,1,1);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
