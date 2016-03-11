Shader "Custom/Effects/Battle Map" {
	Properties {
		_MainTex( "Base (RGB), Alpha (A)", 2D ) = "black" {}
		
		_x( "x", Range ( 0.0, 1 ) ) = 0
		
		_y( "y", Range ( 0.0, 1 ) ) = 0
		
		_w( "w", Range ( 0.0, 1 ) ) = 0.5
	}
	
	SubShader{  
        Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
        
        Pass {  
            Tags{
            	"LightMode" = "Always"
            }
            
            Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha
		
           	CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct appdata {  
	            float4 m_vertex : POSITION;  
	            float2 m_uv : TEXCOORD0;
	        };  
	        
	        struct v4f {  
	            float4 m_pos : POSITION;
	            half2 m_tex : TEXCOORD0;
	            half2 m_tex_c : TEXCOORD1;
	        };  
	        
	        sampler2D _MainTex;
	        float4 _MainTex_ST;
	        
	        uniform float _x;
	        
	        uniform float _y;
	        
	        uniform float _w;
	        
	        v4f vert( appdata p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul(UNITY_MATRIX_MVP, p_v.m_vertex);
	            
	            t_out.m_tex_c = t_out.m_pos.xy;
				
				t_out.m_tex = TRANSFORM_TEX(p_v.m_uv, _MainTex);
				
	            return t_out;
	        }
	        
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret;
				
				t_ret = tex2D( _MainTex, p_v.m_tex ); 
				
				if( abs( distance( (_x, _y ), p_v.m_tex_c ) ) > _w ){
					t_ret.w = 0;
				}
				
				return t_ret;
            }  
            
            ENDCG  
        }
    }
    
	FallBack "Diffuse"
}
