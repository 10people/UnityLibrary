Shader "Custom/Effects/Flow SkyBox" {
	Properties {
		_TintColor( "Tint Color", Color ) = ( 0.72, 0.72, 0, 0.5 )
		_Coef( "Coefficient", Range (0.0, 360.0 ) ) = 0.0
		
		 _FrontTex ("Front (+Z)", 2D) = "white" {}
		 _BackTex ("Back (-Z)", 2D) = "white" {}
		 _LeftTex ("Left (+X)", 2D) = "white" {}
		 _RightTex ("Right (-X)", 2D) = "white" {}
		 _UpTex ("Up (+Y)", 2D) = "white" {}
		 _DownTex ("Down (-Y)", 2D) = "white" {}
	}
	
	CGINCLUDE  
            	
	#include "UnityCG.cginc"
	            
	#define COEF	0.00872664611111111111111111111111
	            
	struct appdata {  
		float4 m_vertex : POSITION;  
		float2 m_uv : TEXCOORD0;
	};  
		        
	struct v4f {  
		float4 m_pos : POSITION;  
		float2 m_uv : TEXCOORD0;
	};  
		        
	uniform float4 _TintColor;
			    
	float _Coef;
    
	v4f vert( appdata p_v ){  
		v4f t_out;
		            
		float t_s = sin( _Coef * COEF );
		        
		float t_c = cos( _Coef * COEF );
		        
		float2x2 t_m = float2x2( t_c, -t_s, t_s, t_c );
		        
		float2 t_xy = mul( float2( p_v.m_vertex.x, p_v.m_vertex.z ), t_m );
		        
		p_v.m_vertex.x = t_xy.x;
				
		p_v.m_vertex.z = t_xy.y;
		            
		t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.m_vertex );
		            
		t_out.m_uv = p_v.m_uv;
		            
		return t_out;  
	}
	            
	ENDCG  
	
	SubShader {
		Tags { "QUEUE"="Background" "RenderType"="Background" }
		
		Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
            
			CGPROGRAM  
            	
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag
			
			sampler2D _FrontTex;

			half4 frag( v4f p_v ) : COLOR{
				half4 t_ret = tex2D( _FrontTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}

			ENDCG
        }
        
        Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
  			
  			CGPROGRAM  
            
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag
			
			sampler2D _BackTex;  
			
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret = tex2D( _BackTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}  
	            
			ENDCG  
        }
        
        Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
  			
  			CGPROGRAM
            
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag  
			
			sampler2D _LeftTex;
			
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret = tex2D( _LeftTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}  
	            
			ENDCG  
        }

        Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
  			
  			CGPROGRAM
            
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag  
			
			sampler2D _RightTex;
			
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret = tex2D( _RightTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}  
	            
			ENDCG  
        }
        
        Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
  			
  			CGPROGRAM
            
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag  
			
			sampler2D _UpTex;
			
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret = tex2D( _UpTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}  
	            
			ENDCG  
        }
        
		Pass {
            Tags { "QUEUE"="Background" "RenderType"="Background" }
            
            ZWrite Off
 			Cull Off
  			Fog { Mode Off }
  			
  			CGPROGRAM
            
            #include "UnityCG.cginc"
	            
			#pragma vertex vert  
			#pragma fragment frag  
			
			sampler2D _DownTex;
			
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret = tex2D( _DownTex, p_v.m_uv ) * _TintColor;
						
				return t_ret;
			}  
	            
			ENDCG  
        }
	}
	
	FallBack "Mobile/Skybox"
}