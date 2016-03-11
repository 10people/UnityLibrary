Shader "Custom/Characters/Main Texture"{
	Properties {
		_Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
	}

	Category {
		Tags {
			"Queue"="Transparent+2"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
			
		ZWrite On
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		
		SubShader {
			Pass{
				Tags { 
					"LightMode"="ShadowCaster" 
					"SHADOWSUPPORT"="true" 
					"RenderType"="Opaque" 
				}
				
				Name "SHADOWCASTER"
				Cull Back
			}
		
			Pass {
				SetTexture [_MainTex] {
					combine texture
				}
			}
		}
	}
}