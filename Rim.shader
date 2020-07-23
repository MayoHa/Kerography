Shader "MayoHa/SpriteOutLine"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HDR]_OutlineColor("OutlineColor",Color) = (1,1,1,1)
		_CheckRange("CheckRange",Range(0,1)) = 0
		_LineWidth("LineWidth",Float) = 0.39
        _CheckAccuracy("CheckAccuracy",Range(0.1,0.99)) = 0.9
		[HDR]_BloomColor("BloomColor",Color) = (1,1,1,1)
		_BloomTex("Bloom Tex",2D) = "white"{}
		_BloomBias("Bloom Bias",Range(-1,1)) = 0.1
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
		Blend One OneMinusSrcAlpha

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
			fixed4 _OutlineColor;
			float _CheckAccuracy;
			float _CheckRange;
			float _LineWidth;
			

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
				t = saturate(t-_BloomBias);
				t = saturate( 1-t);
				//Outline
				float isOut = step(abs(1/_LineWidth),c.a);
				if(isOut != 0){
					fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y*_CheckRange));  
                    fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y*_CheckRange));  
                    fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x*_CheckRange, 0));  
                    fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x*_CheckRange, 0));  
                    float bOut = step((1-_CheckAccuracy),pixelUp.a*pixelDown.a*pixelRight.a*pixelLeft.a);
                    c = lerp(_OutlineColor ,c,bOut);
                    return c+_BloomColor*t;
				}
				return c;
			}
		ENDCG

		
		}
	}




<configuration>
    <property>
        <name>hadoop.tmp.dir</name>
        <value>file:/usr/local/hadoop/tmp</value>
        <description>Abase for other temporary directories.</description>
    </property>
    <property>
        <name>fs.defaultFS</name>
        <value>hdfs://hadoop-master:9000</value>
    </property>
</configuration>


<configuration>
    <property>
        <name>dfs.replication</name>
        <value>3</value>
    </property>
    <property>
        <name>dfs.name.dir</name>
        <value>/usr/local/hadoop/hdfs/name</value>
    </property>
    <property>
        <name>dfs.data.dir</name>
        <value>/usr/local/hadoop/hdfs/data</value>
    </property>
    <!--
    <property>
        <name>dfs.namenode.datanode.registration.ip-hostname-check</name>
        <value>false</value>
    </property>
    -->
</configuration>



<configuration>
  <property>
      <name>mapreduce.framework.name</name>
      <value>yarn</value>
  </property>
   <property>
      <name>mapred.job.tracker</name>
      <value>http://hadoop-master:9001</value>
  </property>
  <property>
      <name>mapreduce.reduce.maxattempts</name>
      <value>100</value>
  </property>
  <property>
      <name>mapreduce.map.maxattempts</name>
      <value>100</value>
  </property>
</configuration>



<configuration>
<!-- Site specific YARN configuration properties -->
    <property>
        <name>yarn.nodemanager.aux-services</name>
        <value>mapreduce_shuffle</value>
    </property>
    <property>
        <name>yarn.resourcemanager.hostname</name>
        <value>hadoop-master</value>
    </property>
</configuration>