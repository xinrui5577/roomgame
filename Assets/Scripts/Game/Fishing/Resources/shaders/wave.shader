Shader "Yx/Refractor1" {

	Properties{
		_Background("Background", 2D) = "" {}            //背景纹理
	_BackgroundScrollX("X Offset", float) = 0        //背景偏移
		_BackgroundScrollY("Y Offset", float) = 0
		_BackgroundScaleX("X Scale", float) = 1.0        //背景缩放
		_BackgroundScaleY("Y Scale", float) = 1.0
		_Refraction("Refraction", float) = 1.0        //折射值
		_DistortionMap("Distortion Map", 2D) = "" {}    //扭曲
	_DistortionScrollX("X Offset", float) = 0
		_DistortionScrollY("Y Offset", float) = 0
		_DistortionScaleX("X Scale", float) = 1.0
		_DistortionScaleY("Y Scale", float) = 1.0
		_DistortionPower("Distortion Power", float) = 0.08
	}

		SubShader{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

		Pass{

		Cull Off
		ZTest LEqual
		ZWrite On
		AlphaTest Off
		Lighting Off
		ColorMask RGBA
		Blend Off

		CGPROGRAM
#pragma target 2.0
#pragma fragment frag
#pragma vertex vert
#include "UnityCG.cginc"

		uniform sampler2D _Background;
	uniform sampler2D _DistortionMap;
	uniform float _BackgroundScrollX;
	uniform float _BackgroundScrollY;
	uniform float _DistortionScrollX;
	uniform float _DistortionScrollY;
	uniform float _DistortionPower;
	uniform float _BackgroundScaleX;
	uniform float _BackgroundScaleY;
	uniform float _DistortionScaleX;
	uniform float _DistortionScaleY;
	uniform float _Refraction;

	struct AppData {
		float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
	};

	struct VertexToFragment {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	VertexToFragment vert(AppData v) {
		VertexToFragment o;
		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	fixed4 frag(VertexToFragment i) : COLOR{
		float2 bgOffset = float2(_BackgroundScrollX,_BackgroundScrollY);
		float2 disOffset = float2(_DistortionScrollX,_DistortionScrollY);
		float2 disScale = float2(_DistortionScaleX,_DistortionScaleY);
		float2 bgScale = float2(_BackgroundScaleX,_BackgroundScaleY);

		float4 disTex = tex2D(_DistortionMap, disScale * i.uv + disOffset);

		float2 offsetUV = (-_Refraction*(disTex * _DistortionPower - (_DistortionPower*0.5)));

		return tex2D(_Background, bgScale * i.uv + bgOffset + offsetUV);
	}

		ENDCG
	}
	}
}