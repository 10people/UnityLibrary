Shader "Custom/Effects/Indicator Alpha Blended" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	SubShader { 
 		Tags { 
	 		"QUEUE"="Transparent-20" 
	 		"IGNOREPROJECTOR"="true" 
	 		"RenderType"="Transparent" 
	 		}
 		
 		Pass {
  			Tags { 
  				"QUEUE"="Transparent"
  				"IGNOREPROJECTOR"="true"
  				"RenderType"="Transparent"
  			}
  			
  			ZTest Always
  			ZWrite Off
  			Cull Off
			AlphaTest Greater 0.01
  			Blend SrcAlpha OneMinusSrcAlpha
  			ColorMask RGB
  			
  			CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct v4f {  
	            float4 m_pos : POSITION;  
	            float4 m_color : COLOR;  
	            float2 m_uv : TEXCOORD0;
	        };  
	        
	        uniform float4 _TintColor;
	        
	        sampler2D _MainTex;
	        
	        v4f vert( appdata_base p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
	            
				t_out.m_color = _TintColor;
				
				t_out.m_uv = p_v.texcoord;
	            
	            return t_out;  
	        }
	        
			half4 frag( v4f p_v ) : COLOR{  
				return p_v.m_color * tex2D( _MainTex, p_v.m_uv ) * 2;
            }  
            
            ENDCG  
  		}
  	}

	Fallback "Particles/Alpha Blended"
}
