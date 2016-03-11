Shader "Custom/Objects/Main Texture High Light" {
	Properties {
		_Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
	}

	Category {
		Tags {
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
		
		ZWrite On
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		
		SubShader {
			usePass "Custom/Characters/Occlusion Colored/RENDERCHARACTER"
		} 
	}
}