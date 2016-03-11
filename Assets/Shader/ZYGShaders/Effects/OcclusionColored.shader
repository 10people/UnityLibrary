Shader "Custom/Effects/Occlusion Colored" {
	Properties 
	{
		_OccludeColor( "Block Color", Color ) = ( 0.5, 0.8, 1, 0.33 )
        _Coef( "Coefficient", Range (0.0, 1.0 ) ) = 0.45
    }
    
    SubShader{  
        Tags{ 
        	"Queue" = "Transparent-10"
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}  
        
        Pass {  
            Tags{
            	"LightMode" = "Always"
            }
            
            ZWrite Off
            Cull Back
            ZTest Greater
            Blend SrcAlpha OneMinusSrcAlpha 
  
            CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct appdata {  
	            float4 m_vertex : POSITION;  
	            float3 m_normal : NORMAL;  
	        };  
	        
	        struct v4f {  
	            float4 m_pos : POSITION;  
	            float4 m_color : COLOR;  
	        };  
	        
	        uniform float4 _OccludeColor;
	        
	        float _Coef;
	        
	        v4f vert( appdata p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.m_vertex );
	            
				t_out.m_color.xyz = p_v.m_normal * _Coef + _OccludeColor.xyz;
					
				t_out.m_color.w = _OccludeColor.w;
	            
	            return t_out;  
	        }
	        
			half4 frag( v4f p_v ) : COLOR{  
				return p_v.m_color;
            }  
            
            ENDCG  
        }
    }
    
    Fallback "Mobile/Unlit (Supports Lightmap)"
}