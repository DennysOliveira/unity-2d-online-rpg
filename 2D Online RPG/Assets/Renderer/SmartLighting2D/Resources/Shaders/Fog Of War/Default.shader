﻿Shader "Light2D/Fog of War/Sprite/Default" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

    }
    SubShader {
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

 
        Stencil
        {
            Ref 1
            Comp equal
        }

        Pass {
                
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
    
            sampler2D _MainTex;

            fixed4 _Color;


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
                    fixed4 color = tex2D (_MainTex, IN.texcoord);
               

                    color *= IN.color;
                    color.rgb *= color.a;
                    
                    return color;
                }

                 ENDCG
        }
    

   
      

       
    }
   
}