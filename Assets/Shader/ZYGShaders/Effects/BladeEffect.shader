Shader "Custom/Effects/Blade Effect" {
    Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	SubShader { 
 		Tags { 
	 		"QUEUE"="Transparent-20" 
	 		"IGNOREPROJECTOR"="true" 
	 		"RenderType"="Transparent" 
	 	}
 		
 		GrabPass {
 			"_BuffTex"
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
  			Blend SrcAlpha OneMinusSrcAlpha
  			
  			CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct v4f {  
	            float4 m_pos : POSITION;  
	            float2 m_coord : TEXCOORD1;
	            float2 m_screen_pos : TEXCOORD0;
	        };
	        
	        uniform float4 _TintColor;
	        
	        sampler2D _MainTex;
	        
	        sampler2D _BuffTex;
	        
	        v4f vert( appdata_base p_v ){  
	            v4f t_out;
	            
	            t_out.m_coord = p_v.texcoord;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
	            
	            float4 t_scr_pos = ComputeScreenPos( t_out.m_pos );
	            
	            t_out.m_screen_pos.xy = t_scr_pos.xy / t_scr_pos.w;
	            
				t_out.m_screen_pos.y = 1 - t_out.m_screen_pos.y;
				
	            return t_out;  
	        }
	        
	        static const half2 m_offset = { 0.025, 0.025 };
	        
			half4 frag( v4f p_v ) : COLOR{
				half4 t_tex_frag = tex2D( _MainTex, p_v.m_coord );
				
				half4 t_scr_frag = tex2D( _BuffTex, p_v.m_screen_pos.xy + float2( 0, -m_offset.y ) );
					
				t_scr_frag = t_scr_frag + tex2D( _BuffTex, p_v.m_screen_pos.xy + float2( 0, m_offset.y ) );
					
				t_scr_frag = t_scr_frag + tex2D( _BuffTex, p_v.m_screen_pos.xy + float2( -m_offset.x, 0 ) );
					
				t_scr_frag = t_scr_frag + tex2D( _BuffTex, p_v.m_screen_pos.xy + float2( m_offset.x, 0 ) );
				
				t_scr_frag = t_scr_frag * 0.25;
				
				t_scr_frag.w = t_tex_frag.w * 2;
				
				return t_scr_frag;
            }  
            
            ENDCG  
  		}
  	}

	Fallback "Particles/Alpha Blended"
}
