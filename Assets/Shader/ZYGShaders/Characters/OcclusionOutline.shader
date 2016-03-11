Shader "Custom/Characters/Occlusion Outline" 
{
	Properties {
		_MainTex( "Base (RGB)", 2D ) = "white" { }
        _Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
        _OutlineColor( "Outline Color", Color ) = ( 0.5, 0.8, 1, 0.8 )
		_Outline ("Outline width", Range (0.0, 0.06)) = .005
	}
 
 	SubShader {
 		Tags { 
			"Queue" = "Transparent" 
		}
	
		Pass {
			Blend zero one
			ZTest Gequal
        }
 
		Pass {
			Cull Back
			ZTest Gequal
 			Blend One OneMinusDstColor
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		 	#include "UnityCG.cginc"
		 
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			 
			struct v4f {
				float4 pos : POSITION;
				float4 color : COLOR;
			};
			 
			uniform float _Outline;
			uniform float4 _OutlineColor;
			 
			v4f vert( appdata p_v ){
				v4f t_v;
				
				t_v.pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
			 
				float3 t_normal = mul( (float3x3)UNITY_MATRIX_IT_MV, p_v.normal );
				
				float2 offset = TransformViewToProjection( t_normal.xy );
			 
				t_v.pos.xy += offset * _Outline;
				
				t_v.color = _OutlineColor;
				
				return t_v;
			}
			
			half4 frag( v4f p_v ) :COLOR {
				return p_v.color;
			}
			
			ENDCG
		}
		
		usePass "Custom/Characters/Occlusion Colored/RENDERCHARACTER"
	}

	Fallback "Mobile/Unlit (Supports Lightmap)"
}