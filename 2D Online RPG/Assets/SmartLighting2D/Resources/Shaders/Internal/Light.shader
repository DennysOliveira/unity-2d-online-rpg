Shader "Light2D/Internal/Light" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _LinearColor ("LinearColor", Float) = 0
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend One OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader {

            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
        
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;
                float _LinearColor;

                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v) {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    return o;
                }

                fixed4 frag (v2f i) : SV_Target {
                    fixed4 tex = tex2D(_MainTex, i.texcoord);

                    fixed4 col =  tex * 2.0f;
                    col.a = (1 - tex.a);

                    col.r *= _TintColor.a * 2;
                    col.g = col.r;
                    col.b = col.r;

                    col.rgb *= _TintColor;

                    if (_LinearColor) {
                        col.rgb =  pow(col.rgb, 1/2.2);
                    }
                
                    return col;
                }
                ENDCG
            }
        }

    }
}