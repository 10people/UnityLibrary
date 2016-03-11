Shader "Custom/Characters/Main Texture High Light" {
	Properties {
		_FxColor("Fx Color", Color) = ( 0, 0, 0, 0 )
	
		_Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
	}

	Category {
		SubShader {
			Tags {
				"Queue"="Transparent-8"
				"IgnoreProjector"="True"
				"RenderType"="Opaque"
			}
			
			UsePass "Custom/Characters/Main Texture/SHADOWCASTER"
				
			ZWrite On
			Cull Off
			Alphatest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha 
			
			UsePass "Custom/Characters/Occlusion Colored/RENDERCHARACTER"
		} 
	}
	
	Fallback "Mobile/Unlit (Supports Lightmap)"
}