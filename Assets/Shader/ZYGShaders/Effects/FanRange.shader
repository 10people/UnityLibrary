Shader "Custom/Effects/Fan Range" {
	Properties {
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
		_Color( "Main Color", Color ) = ( 1.0, 0.125, 0.125, 0.5 )
		_Angle( "Angle", Range ( 0, 360 ) ) = 60
		_Factor( "Scale", Float ) = 1
	}

	Category {
		Tags {
			"Queue"="Transparent-20" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}
		
		SubShader {

			Pass {
				Tags { 
					"Queue" = "Transparent" 
				}
				
				ZTest Always
				ZWrite Off
				AlphaTest Greater 0.01
				Blend SrcAlpha OneMinusSrcAlpha 
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			 	#include "UnityCG.cginc"
			 	
			 	#define COEF	0.00872664611111111111111111111111
			 	
			 	#define AREA	0.001
			 	
				struct v4f {
					float4 pos : POSITION;
					
					float4 color : COLOR;
					
					float2 uv : TEXCOORD0;
				};
				
				sampler2D _MainTex;
				 
				uniform float4 _Color;
								
				uniform float _Angle;
				
				uniform float _Factor;
				
				uniform float _Temp;
				
				v4f vert( appdata_base p_v ){
					v4f t_v;
					
					t_v.color = _Color;
					
					{
						_Angle = clamp( _Angle, 0, 360 );
					
						float t_z = cos( _Angle * COEF ) * _Factor;

						if( p_v.vertex.z < t_z ){
							t_v.color.w = 0;
						}
						else{
							t_v.color.w = 1;
						}
					}
					
					_Temp = AREA * _Factor;
					
					if( abs( p_v.vertex.x ) <= _Temp && 
						abs( p_v.vertex.y ) <= _Temp &&
						abs( p_v.vertex.z ) <= _Temp ){
						t_v.color.w = 1;

					}
					
					t_v.uv =  p_v.texcoord.xy;
					
					t_v.pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
					
					return t_v;
				}
				
				half4 frag( v4f p_v ) :COLOR {
					if( p_v.color.w < 1.0f ){
						p_v.color.w = 0;
					}
					else{
						p_v.color.w = _Color.w;
					}
				
					return tex2D( _MainTex, p_v.uv ) * p_v.color * 2;
				}
				
				ENDCG
			}
		} 
	}
}