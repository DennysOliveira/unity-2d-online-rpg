Shader "Light2D/Internal/Mask" {

	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_SecTex ("CollisionTexture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		translucency ("Radius", Range(0,300)) = 30
		intensity ("Intensity", Range(0,300)) = 1
		textureSize("TextureSize", Range(32,4000)) = 2048
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
		
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _SecTex;

			float4 _SecTex_ST;
			float translucency;
			float intensity;
			float textureSize;

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

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
		
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float radius = translucency * IN.color.a;
				IN.color.a = 1;

				fixed4 tex = float4(1.0, 1.0, 1.0, 1.0);
				
				if (radius > 0) {
					float4 sum = float4(0.0, 0.0, 0.0, 0.0);

					float2 pos = float2(IN.vertex.x / textureSize, IN.vertex.y / textureSize);
					float2 tc = pos;

					float resolution = 1000;

					float blur = radius / resolution / 4;     
					float blurX = blur;
					float blurY = blur;
			
					int size = 30;

			
					//c = 1.0 / (size * 24);

					
   float c = 1.0 / (size * 84);
					//c = 1.0 / (size * 24);

					

					[unroll]
					for (int x = -19; x < 20; x++) {

						[unroll]
						for (int y = -19; y < 20; y++) {
					
							float val = c * sqrt(19-abs(x) + 19 - abs(y));

							sum += tex2D(_SecTex, float2(tc.x + x * blurX, tc.y + y * blurY)) * val;
					

						}
					}
					tex = float4(sum.rgb, 1);
				}

				
				fixed4 color = tex2D (_MainTex, IN.texcoord);
				color.r  = 1;
				color.g  = 1;
				color.b  = 1;

				if (color.a < 0.75) {
					color.a = 0;
				}

				color *= IN.color;
				color.rgb *= color.a;

				if (tex.r < color.r) {
					color.r = tex.r;
				}

					
				if (radius > 0) {
					color.r *= intensity;
				}
		
				return color;
			}

			ENDCG
		}
	}
}