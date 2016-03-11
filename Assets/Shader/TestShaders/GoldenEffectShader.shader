Shader "Custom/GoldenEffectShader" {
	Properties{
		_GoldenColor("Target Color",Color) = (0,0,0,0)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Speed("Golden swift speed",Range(0,1)) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Unlit vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			half4 _GoldenColor;
			fixed _Speed;

			inline fixed4 LightingUnlit(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				fixed4 c = fixed4(1, 1, 1, 1);
				c.rgb = s.Albedo;
				c.a = s.Alpha;

				return c;
			}

			struct Input {
				half2 uv_MainTex;
				half3 vert_Normal;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;


			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vert_Normal = v.normal;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				half halfDiff = (dot(IN.vert_Normal, fmod(_Time.y, _Speed)*(1, 1, 1)) + 1)*0.5;

				//if (halfDiff > 0.5)
				//{
				//	o.Albedo = _GoldenColor.rgb;
				//}
				//else
				//{
				//	o.Albedo = c.rgb;
				//}
				o.Albedo = lerp(c.rgb, _GoldenColor.rgb, halfDiff);
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
