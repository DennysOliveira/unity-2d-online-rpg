Shader "Light2D/Internal/DayBump" {
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _Bump ("Bump", 2D) = "Bump" {}

        _LightSize ("LightSize", Float) = 1
        _LightZ ("LightZ", Float) = 1
        _LightIntensity ("LightIntensity", Float) = 1

        _LightRX("LightRX", float) = 1
        _LightRY("LightRY", float) = 1
        _LightRZ("LightRZ", float) = 1
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {    
            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _Bump;
  
            uniform float _LightSize;
            uniform float _LightZ;
            uniform float _LightIntensity;

            uniform float _LightRX;
            uniform float _LightRY;
            uniform float _LightRZ;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.posWorld = mul(unity_ObjectToWorld, input.vertex);

                output.uv = float2(input.uv.xy);
                output.color = input.color;

                return output;
            }

            float4 frag(VertexOutput input) : COLOR {
                float alpha = tex2D(_MainTex, input.uv).a;

                float3 uvPos = tex2D(_Bump, input.uv).xyz;
                float3 normalDirection = (uvPos - 0.5f) * 2.0f;

                normalDirection = float3(mul(float4(normalDirection.xyz, 1.0f), unity_WorldToObject).xyz);
                normalDirection.z *= -1;
                normalDirection = normalize(normalDirection);

                float3 posWorld = float3(input.posWorld.xyz);
                posWorld.z = 0;
           
                float3 vertexToLightSource = float3(0, 0, -_LightZ) - posWorld.xyz;

                vertexToLightSource = float3(_LightRX, _LightRY, _LightRZ - 0.5);
                float3 lightDirection = normalize(vertexToLightSource);

                float distance = 1;
                float lightUV = 1; 

                float attenuation = sqrt(distance * distance) * _LightIntensity; 
               

                float normalDotLight = dot(normalDirection, lightDirection);
                float diffuseLevel = attenuation * max(0.0f, normalDotLight);

                float specularLevel;
                if (normalDotLight < 0.0f) {
                    specularLevel = 0.0f;
                } else {
                    specularLevel = attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), float3(0.0f, 0.0f, -1.0f))), 10);
                }

                float3 diffuseReflection = diffuseLevel;
                float3 specularReflection = specularLevel;

                return float4((diffuseReflection + specularReflection) * alpha, alpha);
             }

             ENDCG
        }
    }
}
