Shader "Custom/Characters/Bumped Colored Specular" {

	Properties{
		_Color( "Main Color", Color ) = ( 1,1,1,1 )
		_SpecColor( "Specular Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		
		_Shininess( "Shininess", Range( 0.01, 1 ) ) = 0.078125
		_Atten( "Atten", Range( 1, 4 ) ) = 2.0
		
		_MainTex( "Diffuse (RGB), Color Mask (A)", 2D ) = "white" {}
		_BumpTex( "Normalmap", 2D ) = "bump" {}
		_SpeckTex( "Specular (R)", 2D ) = "black" {}
	}

	SubShader{
		Tags {
			"RenderType"="Opaque"
		}
		
		LOD 300

		CGPROGRAM
		#pragma surface surf PPL

		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _SpeckTex;
		
		fixed4 _Color;
		float _Shininess;
		float _Atten;

		struct Input{
			float2 uv_MainTex;
		};

		void surf( Input p_in, inout SurfaceOutput p_out ){
			half4 t_tex = tex2D( _MainTex, p_in.uv_MainTex );
			
			half4 t_spe = tex2D( _SpeckTex, p_in.uv_MainTex );
			
			p_out.Albedo = lerp( t_tex.rgb, t_tex.rgb * _Color.rgb, t_tex.a );
			
			p_out.Alpha = _Color.a;
			
			p_out.Normal = UnpackNormal( tex2D( _BumpTex, p_in.uv_MainTex ) );
			
			p_out.Specular = t_spe.r;
			
			p_out.Gloss = ( 1 - _Shininess );
		}
		
		half4 LightingPPL( SurfaceOutput p_s_out, half3 p_lightDir, half3 p_viewDir, half p_atten ){
			half3 t_normal = normalize( p_s_out.Normal );
			
			half shininess = p_s_out.Gloss * 10.0 + 4.0;

			half reflectiveFactor = max( 0.0, dot( t_normal, normalize( p_lightDir + p_viewDir ) ) );

			half diffuseFactor = max( 0.0, dot( t_normal, p_lightDir ) );
			
			half specularFactor = pow( reflectiveFactor, shininess ) * p_s_out.Specular;

			half4 t_cf;
			
			t_cf.rgb = ( p_s_out.Albedo * diffuseFactor + _SpecColor.rgb * specularFactor ) * _LightColor0.rgb;
			
			t_cf.rgb *= ( p_atten * _Atten );
			
			t_cf.a = p_s_out.Alpha;
			
			return t_cf;
		}
		
		ENDCG
	}
	
	Fallback "Custom/Characters/Main Texture High Light"
}