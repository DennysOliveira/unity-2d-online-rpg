// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Light2D/Internal/AlphaMask" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Mask ("Mask (A)", 2D) = "white" {}
        _LinearColor ("LinearColor", Float) = 0
    }
    SubShader {
        Tags {
            "Queue" = "Transparent+10"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
	
        Pass {   
            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            uniform sampler2D _MainTex;
            uniform sampler2D _Mask;
            uniform float _LinearColor;

            struct VertexInput
            {
                float4 pos : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.pos);
                output.uv = float2(input.uv.xy);

                return output;
            }

            float4 frag(VertexOutput input) : COLOR {
                float4 color = tex2D(_MainTex, input.uv);
                float4 mask = tex2D (_Mask, input.uv);

                if (color.a > 0) {
                    
                    float multiplier = (mask.r + mask.g + mask.b) / 3;

                    if (multiplier > 1) {
                        multiplier = 1;
                    }

                    color.a *= multiplier;

                    if (color.a > 1) {
                        color.a = 1;
                    }
                }

                if (_LinearColor) {
                    color.a = pow(color.a, 1/2.2) * 2;
                }

                return(color);
            }

            ENDCG
        }
    }
  
}