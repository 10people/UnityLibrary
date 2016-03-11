Shader "Custom/Characters/Stroke High Light" {
	Properties {
		_FxColor("Fx Color", Color) = ( 0, 0, 0, 0 )
	
		_Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
		
		_SKColor( "Stroke Color", Color ) = ( 1, 0, 0, 1 )
		
		_Coef( "Coefficient", Range (0.0, 2.0 ) ) = 1.2
		
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
	}

	Category {
		SubShader {
			Tags {
				"Queue"="Transparent-8"
				"IgnoreProjector"="True"
				"RenderType"="Opaque"
			}
			
			UsePass "Custom/Characters/Main Texture/SHADOWCASTER"
			
			Pass {
				ZWrite On
				Cull Off
				Alphatest Greater 0
				Blend SrcAlpha OneMinusSrcAlpha 
				
				CGPROGRAM  
	           
	            #include "UnityCG.cginc"
	            
	            #pragma vertex vert  
	            #pragma fragment frag  
	            
		        struct v4f {  
		            float4 m_pos : POSITION;  
		            float4 m_color : COLOR;
		            float2 m_uv : TEXCOORD0;
		        };
		        
				uniform float4 _FxColor;
			    
				uniform float4 _Color;
				
				uniform float4 _SKColor;
				
				float _Coef;
				
				sampler2D _MainTex;
				
				static const half m_offset = 0.5f;
		        
		        v4f vert( appdata_base p_v ){  
		            v4f t_out;
		            
		            t_out.m_uv = p_v.texcoord;
		            
		            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
		            
					float3 t_normal = mul( (float3x3)UNITY_MATRIX_IT_MV, p_v.normal );
					
					t_normal = t_normal / length( t_normal );
				
					float t_dot = length( TransformViewToProjection( t_normal ).xy );
		            
		            if( t_dot < 0 ){
		            	t_dot = -t_dot;
		            }
		            
		            t_dot = t_dot - _Coef;
		            
		            if( t_dot < 0 ){
		            	t_dot = 0;
		            }
		            
					t_out.m_color = t_dot * _SKColor;
					
		            return t_out;  
		        }
		        
		        half4 frag( v4f p_v ) : COLOR{
					return ( tex2D( _MainTex, p_v.m_uv ) * _Color ) * 2 + _FxColor + p_v.m_color;
	            }
	            
	            ENDCG
			}
		}
	}
	
	Fallback "Custom/Characters/Main Texture High Light"
}