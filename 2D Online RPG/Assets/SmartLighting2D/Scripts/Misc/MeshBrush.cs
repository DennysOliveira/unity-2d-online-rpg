using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MeshBrush {
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector2> uv = new List<Vector2>();
    public List<int> triangles = new List<int>();
    public List<Color> colors = new List<Color>();
    int tris = 0;

    Mesh mesh;

    public MeshBrush() {
        mesh = new Mesh();
    }

    public void Clear() {
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
        colors.Clear();

        tris = 0;
    }

    public void AddMesh(Mesh mesh, Vector3 offset) {
        for(int i = 0; i < mesh.vertices.Length; i++) {
            vertices.Add(mesh.vertices[i] + offset);
        }

        for(int i = 0; i < mesh.uv.Length; i++) {
            uv.Add(mesh.uv[i]);
        }

        for(int i = 0; i < mesh.triangles.Length; i++) {
            triangles.Add(mesh.triangles[i] + tris);
        }

        tris += mesh.vertices.Length;
    }

    public Mesh Export() {
        if (mesh == null) {
            return(null);
        }
        mesh.triangles = null;
        mesh.vertices = null;
        mesh.uv = null;
        mesh.colors = null;

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        if (colors.Count > 0) {
            mesh.colors = colors.ToArray();
        }

        //Debug.Log(triangles.Count / 3);

        return(mesh);
    }
}