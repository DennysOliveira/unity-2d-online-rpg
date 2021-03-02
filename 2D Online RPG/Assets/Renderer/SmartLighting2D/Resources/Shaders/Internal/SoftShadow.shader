Shader "Light2D/Internal/SoftShadow" {
    
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _CoreSize ("CoreSize", Float) = 1
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

        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;

                    // Data
                    float3 texcoord : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                    
                    // Data
                    fixed4 color : COLOR;
                    float3 texcoord : TEXCOORD1;
                };

                uniform float _CoreSize;

                fixed4 _Color;

                v2f vert (appdata_t v)
                {
                    v2f o;
        
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = v.texcoord;
                    o.color = v.color;
                    o.worldPos = mul (unity_ObjectToWorld, v.vertex);
            
                    return o;
                }

                float2 PointToLine(float2 pixelPos, float2 vA, float2 vB)   // How to make smth like this?
                {
                    // Algorithm
                    float2 v0 = vB - vA;
                    v0 /= sqrt(v0.x * v0.x + v0.y * v0.y);

                    float t = dot(v0, pixelPos - vA);

                    if (t <= 0) {
                        return(vA);
                    } else if (t >= distance(vA, vB)) {
                        return(vB);
                    } else {
                        return(vA + v0 * t);
                    }
                }

                float4 Exclusion (float a, float b) {
                    return (0.5-2*(a-0.5)*(b-0.5));
                }

                float2 ReflectAngle(float2 v, float wallAngle) {
                    float2 n;
                    n.x = cos(wallAngle + 3.14159f / 2);
                    n.y = sin(wallAngle + 3.14159f / 2);

                    float dotproduct = v.x * n.x + v.y * n.y;

                    float2 result;
                    result.x = v.x - 2.0f * (dotproduct * n.x);
                    result.y = v.y - 2.0f * (dotproduct * n.y);

                    return(result);
                }

                float EdgeDistance(float2 edgeLocal, float2 edgePosition, float coreSize, float lightSize, float2 pixelPos, float state, float a, float b) {
                    float reflectionDirection;
                    float2 reflection;
                    float2 outerPoint;
                    float2 innerPoint;
                    float outerDistance;
                    float innerDistance;
                    float range;
     
                    float2 edge;
                    
                    float coreOuterRot;
                    float2 coreOuter;

                    float outerRot;
                    float2 outer;
                    
                    float coreInnerRot;
                    float2 coreInner;

                    float innerRot;
                    float2 inner;

                    float2 innerOffset; // Double Offset
                    
                    ///// Edge /////
                    edge.x = edgeLocal.x + edgePosition.x; 
                    edge.y = edgeLocal.y + edgePosition.y;

                    // Outer
                    if (a > 0) {
                        coreOuterRot = atan2(edge.y, edge.x) + (state * 3.14159f) / 2;
                        coreOuter.x = cos(coreOuterRot) * coreSize;
                        coreOuter.y = sin(coreOuterRot) * coreSize;

                    } else {
                        coreOuter.x = 0;
                        coreOuter.y = 0;
                    }
                  
                    outerRot = atan2(edge.y - coreOuter.y, edge.x - coreOuter.x);
                    outer.x = coreOuter.x + cos(outerRot) * lightSize;
                    outer.y = coreOuter.y + sin(outerRot) * lightSize;

                    // Left Inner
                    if (b > 0) {
                        coreInnerRot = atan2(edge.y, edge.x) - (state * 3.14159f) / 2;
                        coreInner.x = cos(coreInnerRot) * coreSize;
                        coreInner.y = sin(coreInnerRot) * coreSize;
                    } else {
                        coreInner.x = 0;
                        coreInner.y = 0;
                    }
                    

                    innerRot = atan2(edge.y - coreInner.y, edge.x - coreInner.x);
                    inner.x = coreInner.x + cos(innerRot) * lightSize;
                    inner.y = coreInner.y + sin(innerRot) * lightSize;

                    // Double Left Inner Offset
                    reflection = ReflectAngle(outer - edge, innerRot);
                    reflectionDirection = atan2(reflection.y, reflection.x);
                    innerOffset.x = edge.x + cos(reflectionDirection) * lightSize;
                    innerOffset.y = edge.y + sin(reflectionDirection) * lightSize;

                    // Calculate Distance
                    outerPoint = PointToLine(pixelPos, outer, edge);
                    innerPoint = PointToLine(pixelPos, innerOffset, edge);

                    outerDistance = distance(outerPoint, pixelPos);
                    innerDistance = distance(innerPoint, pixelPos);
                    range = (outerDistance + innerDistance);

                    return( innerDistance / range - outerDistance / range );
                }

                fixed4 frag (v2f i) : COLOR {
                    float2 leftEdgeLocal;
                    float2 rightEdgeLocal;
                    float2 edgePosition;
                    float edgeRotation;
                    float2 drawOffset;
                    float edge_size;

                    float2 pixelPos = i.worldPos;
                    float coreSize = _CoreSize;
                    float lightSize = 1125;
                    
                    float var_1 = i.texcoord.x;
                    float var_2 = i.texcoord.y; 
                    float var_3 = i.texcoord.z;

                    float var_4 = i.color.r;
                    float var_5 = i.color.g;
                    float var_6 = i.color.b;
                    float var_7 = i.color.a;

                    edgePosition.x = var_1;
                    edgePosition.y = var_2;
                    edgeRotation = var_3;
                    edge_size = var_4;
                    drawOffset.x = var_5;
                    drawOffset.y = var_6;
                    
                    float dirX = cos(edgeRotation) * edge_size;
                    float dirY = sin(edgeRotation) * edge_size;

                    leftEdgeLocal.x = dirX;
                    leftEdgeLocal.y = dirY;

                    rightEdgeLocal.x = -dirX;
                    rightEdgeLocal.y = -dirY;

                    float leftDistance = 0;
                    float rightDistance = 0;

                    float leftDistance1 = EdgeDistance(leftEdgeLocal, edgePosition, coreSize, lightSize, pixelPos, 1, 0, 1);
                    float rightDistance1 = EdgeDistance(rightEdgeLocal, edgePosition, coreSize, lightSize, pixelPos, -1, 0, 1);

                    float exlusionResult = max(leftDistance1, rightDistance1);
                
                    fixed4 col;
                    col.r = 0;
                    col.g = 0;
                    col.b = 0;
                    col.a = 1 - exlusionResult;

                    if (col.a > 1) {
                        col.a = 1;
                    }

                    if (col.a < 0) {
                        col.a = 0;
                    }

                    return col;
                }
            ENDCG
        }
    }

}