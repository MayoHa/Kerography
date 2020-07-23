Shader "MayoHa/SpriteBloom"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		[HDR]_BloomColor("Bloom Color",Color) = (1,1,1,1)
		_BloomTex("Bloom Tex",2D) = "white"{}
		_BloomBias("Bloom Bias",Range(-1,1)) = 0.1
		_BloomTime("Bloom Time",Float) = 1
		_BloomThread("Bloom Thread",Range(0,1)) = 0
		_BloomOfSrc("BloomOfSrc",Range(0,1)) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		//Screen Mode
		Blend one OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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
				float2 bloomTex : TEXCOORD1;
			};
			
			fixed4 _Color;
			fixed4 _BloomColor;
			sampler2D _BloomTex;
			float4 _BloomTex_ST;
			half _BloomBias;
			float _BloomTime;
			float _BloomThread;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.bloomTex = IN.texcoord;
				TRANSFORM_TEX(OUT.texcoord,_BloomTex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			float _BloomOfSrc;
	
			

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				//bloomTex
				float t = tex2D(_BloomTex,IN.bloomTex);
				t =1 - saturate(t-_BloomBias);
				fixed4 bloomColor = fixed4(c.rgb * _BloomOfSrc + _BloomColor.rgb * (1 - _BloomOfSrc),1);
				//float difference = c.a - _BloomThread;
				//float smooth = t* abs(c.a - _BloomThread);
				//smooth = smoothstep(difference-0.1,difference+0.1,smooth);
				c = c.a > _BloomThread ?c+bloomColor*t*_BloomTime : c ;
				
				//t = saturate( 1-t);
				//Outline
				//float isOut = step(abs(1/_LineWidth),c.a);

                    return c;

			}
		ENDCG

		
		}
	}
}