using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GLExtended
{

    public static Color color = Color.white;
    public static void SetColor(Color _color) {
        color = _color;
    }

    public static void ResetColor() {
        color = Color.white;
    }

    public static void DrawMeshPass(List<MeshObject> meshes, Vector3 position, Vector2 scale, float rotation) {
        foreach(MeshObject mesh in meshes) {
            DrawMeshPass(mesh, position, scale, rotation);
        }
    }

    public static void DrawMeshPass(MeshObject mesh, Vector3 position, Vector2 scale, float rotation) {
        bool useUV = mesh.uv.Length > 0;

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            int t0 = mesh.triangles[i + 0];
            int t1 = mesh.triangles[i + 1];
            int t2 = mesh.triangles[i + 2];

            Vector3 p0 = mesh.vertices[t0]; 
            Vector3 p1 = mesh.vertices[t1];
            Vector3 p2 = mesh.vertices[t2];

            p0.x *= scale.x;
            p0.y *= scale.y;

            p1.x *= scale.x;
            p1.y *= scale.y;

            p2.x *= scale.x;
            p2.y *= scale.y;

            float angle0 = (float)Math.Atan2(p0.y, p0.x) + rotation * Mathf.Deg2Rad;
            float dist0 = Mathf.Sqrt(p0.x * p0.x + p0.y * p0.y);

            p0.x = Mathf.Cos(angle0) * dist0;
            p0.y = Mathf.Sin(angle0) * dist0;

            float angle1 = (float)Math.Atan2(p1.y, p1.x) + rotation * Mathf.Deg2Rad;
            float dist1 = Mathf.Sqrt(p1.x * p1.x + p1.y * p1.y);

            p1.x = Mathf.Cos(angle1) * dist1;
            p1.y = Mathf.Sin(angle1) * dist1;

            float angle2 = (float)Math.Atan2(p2.y, p2.x) + rotation * Mathf.Deg2Rad;
            float dist2 = Mathf.Sqrt(p2.x * p2.x + p2.y * p2.y);

            p2.x = Mathf.Cos(angle2) * dist2;
            p2.y = Mathf.Sin(angle2) * dist2;

            p0.x += position.x;
            p0.y += position.y;

            p1.x += position.x;
            p1.y += position.y;

            p2.x += position.x;
            p2.y += position.y;

            if (useUV) {
                Vector2 uv0 = mesh.uv[t0];
                Vector2 uv1 = mesh.uv[t1];
                Vector2 uv2 = mesh.uv[t2];

                GL.TexCoord3(uv0.x, uv0.y, 0);
                GL.Vertex3(p0.x, p0.y, 0);
                GL.TexCoord3(uv1.x, uv1.y, 0);
                GL.Vertex3(p1.x, p1.y, 0);
                GL.TexCoord3(uv2.x, uv2.y, 0);
                GL.Vertex3(p2.x, p2.y, 0);
            } else {
                GL.Vertex3(p0.x, p0.y, 0);
                GL.Vertex3(p1.x, p1.y, 0);
                GL.Vertex3(p2.x, p2.y, 0);
            }
        }
    }

    public static void DrawMesh(MeshObject mesh, Vector3 position, Vector2 scale, float rotation) {
        GL.Begin(GL.TRIANGLES);

        DrawMeshPass(mesh, position, scale, rotation);

        GL.End();
    }

    public static void DrawMesh(List<MeshObject> meshes, Vector3 position, Vector2 scale, float rotation) {
        GL.Begin(GL.TRIANGLES);

        foreach(MeshObject mesh in meshes) {
            DrawMeshPass(mesh, position, scale, rotation);
        }

        GL.End();
    }
}
