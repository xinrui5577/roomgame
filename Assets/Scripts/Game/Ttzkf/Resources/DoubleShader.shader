Shader "Custom/DoubleShader" {  
Properties {  
    _Color ("Main Color", Color) = (1,1,1,1)//Tint Color  
    _MainTex ("Base (RGB)", 2D) = "white" {}  
	_Color_2 ("Main Color", Color) = (1,1,1,1)//Tint Color  
    _MainTex_2 ("Base (RGB)", 2D) = "white" {}  
	_Color_3 ("Main Color", Color) = (1,1,1,1)//Tint Color 
    _MainTex_3 ("Base (RGB)", 2D) = "white" {}  

}  
  
SubShader {  
    Tags { "Queue" = "Transparent" }
    LOD 10000  
    Material
			{
				Diffuse [_Color]
				Ambient (1,1,1,1)
			} 
	Blend SrcAlpha OneMinusSrcAlpha		
	
    Pass {  
        Cull Front  
        Lighting On
        SetTexture [_MainTex] { combine texture }   
        SetTexture [_MainTex]  
        {  
            ConstantColor [_Color]  
        }  
    }  
  
    Pass  
    {  
        Cull Back  
        Lighting Off  
        SetTexture [_MainTex_2] { combine texture }   
        SetTexture [_MainTex_2]  
        {  
            ConstantColor [_Color_2]  
            Combine Previous * Constant  
        }  
    } 
	
	Pass  
    {  
        Cull Back  
        Lighting Off  
        SetTexture [_MainTex_3] { combine texture }   
        SetTexture [_MainTex_3]  
        {  
            ConstantColor [_Color_3]  
            Combine Previous * Constant  
        }  
    } 
}  
}   