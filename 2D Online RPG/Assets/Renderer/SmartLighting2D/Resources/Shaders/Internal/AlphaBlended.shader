Shader "Light2D/Internal/AlphaBlended" {
        
  Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (1,1,1,1)
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
	
			#include "UnityCG.cginc"
		
			fixed4 _TintColor;
			sampler2D _MainTex;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv0  : TEXCOORD0;
				float2 uv1 : TEXCOORD1;

			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv0 = IN.texcoord0;
				OUT.uv1 = IN.texcoord1;
				OUT.color = IN.color * _TintColor;
		
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{

				//fixed4 color = tex2D (_MainTex, IN.uv0  / IN.uv1.x );
				fixed4 color = tex2D (_MainTex, IN.uv0 );
				//color.r  = 1;
				//color.g  = 1;
				//color.b  = 1;

		
				color *= IN.color;
				color.rgb *= color.a;
				
				return color;
			}


			ENDCG



			
		}
	}
}