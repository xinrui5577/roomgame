Shader "Tang/GuideMask"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_SceneWidth("SceneWidth",Int) = 1136
		_SceneHeight("SceneHeight",Int) = 640
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
		//中心
		_Origin("圆心",Vector) = (0,0,0,0)
		//裁剪方式 0圆形 1圆形
		_MaskType("Type",Float) = 0
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
	{
		Name "Default"
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_ALPHACLIP

		struct appdata_t
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		//UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float4 worldPosition : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	fixed4 _Color;
	fixed4 _TextureSampleAdd;
	float4 _ClipRect;
	float4 _Origin;
	float _MaskType;
	int _SceneWidth;
	int _SceneHeight;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.worldPosition = IN.vertex;
		OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

		OUT.texcoord = IN.texcoord;

		OUT.color = IN.color * _Color;
		return OUT;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f IN) : SV_Target
	{
		float2 uv = IN.texcoord;
		half4 col = IN.color;
		//开始裁剪
		//外部直接给坐标 宽 高 GPU计算比率
		float w2 = _SceneWidth / 2;
		float h2 = _SceneHeight / 2;
		float posX = (_Origin.x + w2) / _SceneWidth;
		float posY = (_Origin.y + h2) / _SceneHeight;
		float2 pos = float2(posX, posY);
		if (_MaskType == 0) {
			posX = posX * _SceneWidth / _SceneHeight;
			pos = float2(posX, posY);
			float rid = _Origin.z;
			uv.x = uv.x * _SceneWidth / _SceneHeight;
			float2 nor = uv - pos;
			if (length(nor) < rid)
				col.a = 0;
		}
		else {
			float w = _Origin.z;
			float h = _Origin.w;
			if (uv.x > pos.x - w && uv.x<pos.x + w && uv.y>pos.y - h && uv.y < pos.y + h)
				col.a = 0;
		}

		half4 color = (tex2D(_MainTex,uv) + _TextureSampleAdd) * col;
		color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
		clip(col.a);
#ifdef UNITY_UI_ALPHACLIP
		clip(color.a - 0.001);
#endif

		return color;
	}
		ENDCG
	}
	}
}