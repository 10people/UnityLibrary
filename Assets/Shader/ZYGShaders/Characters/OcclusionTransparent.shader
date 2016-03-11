Shader "Custom/Characters/Occlusion Transparent" 
{
	Properties 
	{
		_MainTex( "Base (RGB)", 2D ) = "white" { }
        _Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
        _OccludeColor( "Block Color", Color ) = ( 1, 1, 1, 0.4 )
    }
    
    SubShader {  
        Tags { 
        	"Queue" = "Transparent"
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}  
        
        Pass {
            Cull Back
            ZWrite Off
            ZTest GEqual
            Blend SrcAlpha OneMinusSrcAlpha 
            
            SetTexture [_MainTex] 
            {
            	ConstantColor [_OccludeColor]
                Combine Texture * Constant
            }
        }
        
        usePass "Custom/Characters/Occlusion Colored/RENDERCHARACTER"
    }  

	Fallback "Mobile/Unlit (Supports Lightmap)"
}