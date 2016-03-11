Shader "Custom/Effects/Battle Map RT" {
	Properties {
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		
		_FxTex ("Albedo (RGB)", 2D) = "white" {}
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
	        };  
	        
	        sampler2D _MainTex;
	        float4 _MainTex_ST;
	        
	        sampler2D _FxTex;
	        float4 _FxTex_ST;
	        
	        v4f vert( appdata p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.m_vertex );
				
				t_out.m_tex = TRANSFORM_TEX( p_v.m_uv, _MainTex );
				
	            return t_out;
	        }
	        
			half4 frag( v4f p_v ) : COLOR{  
				half4 t_ret;
				
				t_ret = tex2D( _MainTex, p_v.m_tex ); 
				
				half4 t_fx = tex2D( _FxTex, p_v.m_tex );
				
				if( t_fx.w < 0.5 ){
					t_ret.w = 0;
				}
				
				return t_ret;
            }  
            
            ENDCG  
        }
    }
    
	FallBack "Diffuse"
}
