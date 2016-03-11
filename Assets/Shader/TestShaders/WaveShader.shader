Shader "Custom/WaveShader" {
	Properties{
		_SummitColor("_SummitColor", Color) = (1,1,1,1)
		_DilemmaColor("_DilemmaColor", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Amount("_Amount",Range(0,10)) = 1
		_Frequency("Frequency",Range(0,10)) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Lambert vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
		float3 _SummitColor;
		float3 _DilemmaColor;
		float _Amount;
		float _Frequency;

			struct Input {
				float2 uv_MainTex;
				float vertex_Lerp;
			};

			void vert(inout appdata_full v, out Input IN)
			{
				UNITY_INITIALIZE_OUTPUT(Input, IN);

				float waveAmount = sin(_Frequency*v.vertex.x + _Time.y)*_Amount;
				v.vertex.y = waveAmount;

				IN.vertex_Lerp = waveAmount / _Amount / 2 + 0.5;
			}


			void surf(Input IN, inout SurfaceOutput o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				float3 environmentColor = lerp(_DilemmaColor, _SummitColor, IN.vertex_Lerp);
				o.Albedo = c.rgb*environmentColor;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
