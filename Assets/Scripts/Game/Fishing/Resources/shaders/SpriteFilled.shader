///基于unity5.4.4的SpriteDefault
Shader "Sprites/SpriteFilled"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		///1:横向；2:纵向；
		_FillMethod ("FillMethod", Int) = 1
		//1Method(1:从左到右显示；2:从右到左显示)
		//2Method(1:从下到上显示；2:从上到下显示)
		_FillOrigin ("FillOrigin", Int) = 1
		_FillAmount ("FillAmount", Range(0,1)) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		LOD 200

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			int _FillMethod;
			int _FillOrigin;
			fixed _FillAmount;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA
				if(1 == _FillMethod)
				{
					if(1 == _FillOrigin && uv.x>_FillAmount)
					{
						discard;
					}
					else if(2 == _FillOrigin && uv.x<(1-_FillAmount))
					{
						discard;
					}
				}
				else if(2 == _FillMethod)
				{
					if(1 == _FillOrigin && uv.y>_FillAmount)
					{
						discard;
					}
					else if(2 == _FillOrigin && uv.y<(1-_FillAmount))
					{
						discard;
					}
				}
				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}

}
