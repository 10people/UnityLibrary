Shader "Custom/Characters/Occlusion Colored" {
	Properties 
	{
		_MainTex( "Base (RGB)", 2D ) = "white" { }
        _Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
        _OccludeColor( "Block Color", Color ) = ( 0.5, 0.8, 1, 0.33 )
        _Coef( "Coefficient", Range (0.0, 1.0 ) ) = 0.45
        
        _FxColor("Fx Color", Color) = ( 0, 0, 0, 0 )
    }
    
    SubShader {  
    	Tags { 
           	"Queue" = "Transparent-9"
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}
            
		UsePass "Custom/Characters/Main Texture/SHADOWCASTER"
            
        Pass {  
			ZWrite Off
            ZTest GEqual
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
        
        Pass {
        	name "RENDERCHARACTER"
        	ZWrite On
        	Cull Off
        	Alphatest Greater 0.01
			Blend SrcAlpha OneMinusSrcAlpha 
            
            SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant DOUBLE, texture * constant				
			}
			
			SetTexture [_MainTex] {
				constantColor [_FxColor]
				Combine Previous + constant, Previous			
			}
        }
    }  

	Fallback "Mobile/Unlit (Supports Lightmap)"
}