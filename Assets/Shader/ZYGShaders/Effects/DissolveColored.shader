Shader "Custom/Effects/Dissolve Colored" {
	Properties {
		_Dissolve ("Dissolve", Range(0,1)) = 0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { 
           	"Queue" = "Transparent-9"
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}
            
//		UsePass "Custom/Characters/Main Texture/SHADOWCASTER"
            
        Pass {  
			ZWrite On
			Cull Off
			Alphatest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha 
			
            CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct appdata {  
	            float4 m_vertex : POSITION;  
	            float2 m_tex: TEXCOORD0;
	            float3 m_normal : NORMAL;  
	        };  
	        
	        struct v4f {  
	            float4 m_pos : POSITION;
	            float2 m_tex : TEXCOORD0;
	        };  
	        
	        sampler2D _MainTex;
	        
	        float4 _MainTex_ST;
	        
	        float _Dissolve;
	        
	        v4f vert( appdata p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.m_vertex );
	            
	            t_out.m_tex = p_v.m_tex;
	            
	            return t_out;  
	        }
	        
			fixed4 frag( v4f p_v ) : COLOR{  
				float4 t_c = saturate( tex2D( _MainTex, p_v.m_tex ) * 1.2f );

				if( ( t_c.x + t_c.y + t_c.z ) * 0.3f < _Dissolve ){
					t_c.w = 0;
				}
				
				return t_c;
            }  
            
            ENDCG  
        }
	} 
	
	FallBack "Diffuse"
}
