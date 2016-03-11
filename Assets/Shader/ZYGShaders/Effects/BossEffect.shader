Shader "Custom/Effects/Boss Effect" {
	Properties {
		_TintColor( "Tint Color", Color ) = ( 0.72, 0.72, 0, 0.5 )
		_Coef( "Coefficient", Range (0.0, 1.0 ) ) = 0.4
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
		_Vec3("Vector3", Vector ) = ( 1, 0, 0, 1 )
		
		_FxColor("Fx Color", Color) = ( 0, 0, 0, 0 )
	}

	Category {
		SubShader {
			Tags {
				"Queue"="Transparent-8"
				"IgnoreProjector"="True"
				"RenderType"="Opaque"
			}
			
			UsePass "Custom/Characters/Main Texture/SHADOWCASTER"
				
			ZWrite On
			Cull Off
			Alphatest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha 
			
			Pass {  
	            Tags{
	            	"LightMode" = "Always"
	            }
	            
	            ZWrite On
	            Cull Back
	            Blend SrcAlpha OneMinusSrcAlpha 
	  
	            CGPROGRAM  
	           
	            #include "UnityCG.cginc"
	            
	            #pragma vertex vert  
	            #pragma fragment frag  
	            
		        struct appdata {  
		            float4 m_vertex : POSITION;  
		            float3 m_normal : NORMAL;  
		            float2 m_uv : TEXCOORD0;
		        };  
		        
		        struct v4f {  
		            float4 m_pos : POSITION;  
		            float4 m_color : COLOR;
		            float2 m_uv : TEXCOORD0;
		        };  
		        
		        uniform float4 _TintColor;
		        
		        uniform float4 _FxColor;
		        
		        uniform float4 _Vec3;
		        
		        sampler2D _MainTex;
		        
		        float _Coef;
		        
		        v4f vert( appdata p_v ){  
		            v4f t_out;
		            
		            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.m_vertex );
		            
		            float t_dot = dot( p_v.m_normal, _Vec3.xyz );
		            
		            if( t_dot < 0 ){
		            	t_dot = -t_dot;
		            }
		            
					t_out.m_color.xyz = _TintColor.xyz * t_dot * _Coef * 2;
					
					t_out.m_color.w = 1;
					
					t_out.m_uv = p_v.m_uv;
		            
		            return t_out;  
		        }
		        
				half4 frag( v4f p_v ) : COLOR{  
					half4 t_ret = tex2D( _MainTex, p_v.m_uv );
						
					t_ret = t_ret + p_v.m_color + _FxColor;
						
					t_ret.w = 1;
						
					return t_ret;
	            }  
	            
	            ENDCG  
	        }
		} 
	}
	
	Fallback "Mobile/Unlit (Supports Lightmap)"
}
