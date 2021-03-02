using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EdgePass {
    public Vector2 edgePosition;
    public float edgeRotation;
    public float edgeSize;

    public float coreSize;

    public float var_1 = 0;
    public float var_2 = 0;
    public float var_3 = 0;
    public float var_4 = 0;
    public float var_5 = 0;
    public float var_6 = 0;
    public float var_7 = 0;

    public void SetVars() {
        var_1 = edgePosition.x;
        var_2 = edgePosition.y;
        var_3 = edgeRotation;
        var_4 = edgeSize;
        var_5 = Rendering.Light.ShadowEngine.drawOffset.x;
        var_6 = Rendering.Light.ShadowEngine.drawOffset.y;
    }

    public Vector2 leftEdgeLocal;
    public Vector2 rightEdgeLocal;
    public Vector2 leftEdge;
    public Vector2 rightEdge;

    public float leftOuterCore;
    public Vector2 leftCoreOut;

    public float leftOuterToEdge;
    public Vector2 leftCoreOutToEdge;

    public float leftInnerCore;
    public Vector2 leftCoreIn;

    public float leftInnerToEdge;
    public Vector2 leftCoreInToEdge;

    public float rightOuterCore;
    public Vector2 rightCoreOut;

    public float rightOuterToEdge;
    public Vector2 rightCoreOutToEdge;

    public float rightInnerCore;
    public Vector2 rightCoreIn;

    public float rightInnerToEdge;
    public Vector2 rightCoreInToEdge;

    public Vector2 edgeMiddle;
    public Vector2 projectedMiddle;

    public void Generate() {
        float lightSize = 1000;

        float dirX = Mathf.Cos(edgeRotation) * edgeSize;
        float dirY = Mathf.Sin(edgeRotation) * edgeSize;

        leftEdgeLocal = new Vector2(dirX, dirY);
        rightEdgeLocal = new Vector2(-dirX, -dirY);

        leftEdge = leftEdgeLocal + edgePosition;
        rightEdge = rightEdgeLocal + edgePosition;

        // left outer
        leftOuterCore = Mathf.Atan2(leftEdge.y, leftEdge.x) + Mathf.PI / 2;;
        leftCoreOut.x = Mathf.Cos(leftOuterCore) * coreSize;
        leftCoreOut.y = Mathf.Sin(leftOuterCore) * coreSize;
        leftCoreOut.x = 0;
        leftCoreOut.y = 0;
    
        leftOuterToEdge = Mathf.Atan2(leftEdge.y - leftCoreOut.y, leftEdge.x - leftCoreOut.x);
        leftCoreOutToEdge = leftCoreOut; // middle
        leftCoreOutToEdge.x += Mathf.Cos(leftOuterToEdge) * lightSize;
        leftCoreOutToEdge.y += Mathf.Sin(leftOuterToEdge) * lightSize;
    
        leftInnerCore = Mathf.Atan2(leftEdge.y, leftEdge.x) - Mathf.PI / 2;;
        leftCoreIn.x = Mathf.Cos(leftInnerCore) * coreSize;
        leftCoreIn.y = Mathf.Sin(leftInnerCore) * coreSize;
        
        leftInnerToEdge = Mathf.Atan2(leftEdge.y - leftCoreIn.y, leftEdge.x - leftCoreIn.x);
        leftCoreInToEdge = leftCoreIn; // middle
        leftCoreInToEdge.x += Mathf.Cos(leftInnerToEdge) * lightSize;
        leftCoreInToEdge.y += Mathf.Sin(leftInnerToEdge) * lightSize;
        
        // right outer
        rightOuterCore = Mathf.Atan2(rightEdge.y, rightEdge.x) + Mathf.PI / 2;;
        rightCoreOut.x = Mathf.Cos(rightOuterCore) * coreSize;
        rightCoreOut.y = Mathf.Sin(rightOuterCore) * coreSize;
    
        rightOuterToEdge = Mathf.Atan2(rightEdge.y - rightCoreOut.y, rightEdge.x - rightCoreOut.x);
        rightCoreOutToEdge = rightCoreOut; // middle
        rightCoreOutToEdge.x += Mathf.Cos(rightOuterToEdge) * lightSize;
        rightCoreOutToEdge.y += Mathf.Sin(rightOuterToEdge) * lightSize;
        
        rightInnerCore = Mathf.Atan2(rightEdge.y, rightEdge.x) - Mathf.PI / 2;;
        rightCoreIn.x = Mathf.Cos(rightInnerCore) * coreSize;
        rightCoreIn.y = Mathf.Sin(rightInnerCore) * coreSize;
        rightCoreIn.x = 0;
        rightCoreIn.y = 0;
    
    
        rightInnerToEdge = Mathf.Atan2(rightEdge.y - rightCoreIn.y, rightEdge.x - rightCoreIn.x);
        rightCoreInToEdge = rightCoreIn; // middle
        rightCoreInToEdge.x += Mathf.Cos(rightInnerToEdge) * lightSize;
        rightCoreInToEdge.y += Mathf.Sin(rightInnerToEdge) * lightSize;



        edgeMiddle = (leftEdge + rightEdge) / 2;

        Vector2 closestPoint = Math2D.ClosestPointOnLine(Vector2.zero, leftCoreOutToEdge, rightCoreInToEdge);
        float rotM = (float)System.Math.Atan2 (closestPoint.y, closestPoint.x);
        projectedMiddle.x = (leftEdge.x + rightEdge.x) / 2 + Mathf.Cos(rotM) * lightSize;
        projectedMiddle.y = (leftEdge.y + rightEdge.y) / 2 + Mathf.Sin(rotM) * lightSize; 
    }

    public void Draw() {
        GL.Color(new Color(var_4, var_5, var_6, var_7));
        GL.TexCoord3(var_1, var_2, var_3);

        Vector2 edgeAWorld = leftEdge;
        Vector2 edgeBWorld = rightEdge;

        Vector2 edgeALocal = leftEdgeLocal;
        Vector2 edgeBLocal = rightEdgeLocal;

        float lightDirection = (float)Math.Atan2((edgeAWorld.y + edgeBWorld.y) / 2 , (edgeAWorld.x + edgeBWorld.x) / 2 ) * Mathf.Rad2Deg;
        float EdgeDirection = (float)Math.Atan2(edgeALocal.y - edgeBLocal.y, edgeALocal.x - edgeBLocal.x) * Mathf.Rad2Deg - 180;

        lightDirection -= EdgeDirection;
        lightDirection = (lightDirection + 720) % 360;
        
    
        if (lightDirection > 180) {
            GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
            GL.Vertex3(edgeMiddle.x, edgeMiddle.y, 0);
            GL.Vertex3(leftEdge.x, leftEdge.y, 0);

            GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
            GL.Vertex3(edgeMiddle.x, edgeMiddle.y, 0);
            GL.Vertex3(rightEdge.x, rightEdge.y, 0);
        }

        Vector2? fullResult = Math2D.GetPointLineIntersectLine3(leftEdge, leftCoreOutToEdge, rightEdge, rightCoreInToEdge);

        if (fullResult != null) {

            GL.Vertex3(fullResult.Value.x, fullResult.Value.y, 0);
            GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
            GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);

        } 
        else 
        {
            Vector2? leftResult = Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, rightEdge, rightCoreInToEdge);
            if (leftResult != null) {

                GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);

                GL.Vertex3(rightEdge.x, rightEdge.y, 0);
                GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);

                GL.Vertex3(rightEdge.x, rightEdge.y, 0);
                GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);
                GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);

                GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);
            
            } 
            else 
            {
                Vector2? rightResult = Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, leftEdge, leftCoreOutToEdge);

                if (rightResult != null) {
                    GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                    GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                    GL.Vertex3(rightEdge.x, rightEdge.y, 0);

                    GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                    GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                    GL.Vertex3(leftEdge.x, leftEdge.y, 0);

                    GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                    GL.Vertex3(rightResult.Value.x, rightResult.Value.y, 0);
                    GL.Vertex3(leftEdge.x, leftEdge.y, 0);

                    GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                    GL.Vertex3(rightResult.Value.x, rightResult.Value.y, 0);
                    GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                } else {
                    if (lightDirection > 180) {
                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                        GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                        GL.Vertex3(rightEdge.x, rightEdge.y, 0);

                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                        GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                        GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                    }
                }   
            }
        }
    }
}


/*




 Vector2 draw = Rendering.Light.ShadowEngine.drawOffset;
        
    
        if (lightDirection > 180) {
            GL.Vertex3(draw.x + projectedMiddle.x, draw.y +projectedMiddle.y, 0);
            GL.Vertex3(draw.x + edgeMiddle.x, draw.y +edgeMiddle.y, 0);
            GL.Vertex3(draw.x + leftEdge.x, draw.y +leftEdge.y, 0);

            GL.Vertex3(draw.x + projectedMiddle.x, draw.y +projectedMiddle.y, 0);
            GL.Vertex3(draw.x + edgeMiddle.x, draw.y +edgeMiddle.y, 0);
            GL.Vertex3(draw.x + rightEdge.x, draw.y +rightEdge.y, 0);
        }

        Vector2? fullResult = Math2D.GetPointLineIntersectLine3(leftEdge, leftCoreOutToEdge, rightEdge, rightCoreInToEdge);

        if (fullResult != null) {

            GL.Vertex3(draw.x + fullResult.Value.x, draw.y + fullResult.Value.y, 0);
            GL.Vertex3(draw.x + rightCoreInToEdge.x, draw.y + rightCoreInToEdge.y, 0);
            GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);

        } 
        else 
        {
            Vector2? leftResult = Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, rightEdge, rightCoreInToEdge);
            if (leftResult != null) {

                GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                GL.Vertex3(draw.x + rightCoreInToEdge.x, draw.y + rightCoreInToEdge.y, 0);
                GL.Vertex3(draw.x + leftResult.Value.x, draw.y + leftResult.Value.y, 0);

                GL.Vertex3(draw.x + rightEdge.x, draw.y + rightEdge.y, 0);
                GL.Vertex3(draw.x + leftEdge.x, draw.y + leftEdge.y, 0);
                GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);

                GL.Vertex3(draw.x + rightEdge.x, draw.y + rightEdge.y, 0);
                GL.Vertex3(draw.x + leftResult.Value.x, draw.y + leftResult.Value.y, 0);
                GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);

                GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);
                GL.Vertex3(draw.x + leftResult.Value.x, draw.y + leftResult.Value.y, 0);
            
            } 
            else 
            {
                Vector2? rightResult = Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, leftEdge, leftCoreOutToEdge);

                if (rightResult != null) {
                    GL.Vertex3(draw.x + rightCoreInToEdge.x, draw.y + rightCoreInToEdge.y, 0);
                    GL.Vertex3(draw.x + leftEdge.x, draw.y + leftEdge.y, 0);
                    GL.Vertex3(draw.x + rightEdge.x, draw.y + rightEdge.y, 0);

                    GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                    GL.Vertex3(draw.x + rightCoreInToEdge.x, draw.y + rightCoreInToEdge.y, 0);
                    GL.Vertex3(draw.x + leftEdge.x, draw.y + leftEdge.y, 0);

                    GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                    GL.Vertex3(draw.x + rightResult.Value.x, draw.y + rightResult.Value.y, 0);
                    GL.Vertex3(draw.x + leftEdge.x, draw.y + leftEdge.y, 0);

                    GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                    GL.Vertex3(draw.x + rightResult.Value.x, draw.y + rightResult.Value.y, 0);
                    GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);
                } else {
                    if (lightDirection > 180) {
                        GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                        GL.Vertex3(draw.x + rightCoreInToEdge.x, draw.y + rightCoreInToEdge.y, 0);
                        GL.Vertex3(draw.x + rightEdge.x, draw.y + rightEdge.y, 0);

                        GL.Vertex3(draw.x + projectedMiddle.x, draw.y + projectedMiddle.y, 0);
                        GL.Vertex3(draw.x + leftCoreOutToEdge.x, draw.y + leftCoreOutToEdge.y, 0);
                        GL.Vertex3(draw.x + leftEdge.x, draw.y + leftEdge.y, 0);
                    }
                }   
            }
        }
    }
    */