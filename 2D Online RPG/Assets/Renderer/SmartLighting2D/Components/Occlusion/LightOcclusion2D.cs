using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightOcclusion2D : MonoBehaviour {
    public enum ShadowType {Collider, SpritePhysicsShape};
    public enum OcclusionType {Hard, Soft};

    public OcclusionType occlusionType = OcclusionType.Hard;
	public float occlusionSize = 1f;

    private LightingOcclussion occlusionShape = null;
    public LightingOcclusionShape shape = new LightingOcclusionShape();

    public GameObject occlusionGameObject;
    public GameObject GetOcclusionGameObject() {
        if (occlusionGameObject == null) {
            GameObject gameObject = new GameObject("Occlussion");
            gameObject.transform.parent = transform;

            occlusionGameObject = gameObject;
        }
        if (occlusionGameObject) {
            occlusionGameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        return(occlusionGameObject);
    }

    public MeshFilter meshFilter;
    public MeshFilter GetMeshFilter() {
        if (meshFilter == null) {
            GameObject gameObject = GetOcclusionGameObject();
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null) {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }
        return(meshFilter);
    }

    public MeshRenderer meshRenderer;
    public MeshRenderer GetMeshRenderer() {
        if (meshRenderer == null) {
            GameObject gameObject = GetOcclusionGameObject();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
        }
        return(meshRenderer);
    }

	public LightingOcclussion GetOcclusionShape() {
		if (occlusionShape == null) {
			occlusionShape = new LightingOcclussion(shape, occlusionSize);
		}
		return(occlusionShape);
	}

    public void OnEnable() {
        shape.SetTransform(transform); 
		Initialize();
	}

    public void Initialize() {
        occlusionShape = null;
        shape.ResetLocal();

        switch(occlusionType) {
            case OcclusionType.Hard:
                GenerateMesh_Hard();
                break;
            case OcclusionType.Soft:
                GenerateMesh_Soft();
                break;
        }
    }

    public void Update() {}

    void GenerateMesh_Hard() {
        List<Pair2D> iterate1, iterate2;
        Vector2D first = null;
		Pair2D pA, pB;
        bool isEdgeCollider = shape.IsEdgeCollider();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int count = 0;

        GameObject gameObject = GetOcclusionGameObject();
        MeshRenderer meshRenderer = GetMeshRenderer();
        MeshFilter meshFilter = GetMeshFilter();
        occlusionShape = GetOcclusionShape();

        for(int x = 0; x < occlusionShape.polygonPoints.Count; x++) {
            iterate1 = occlusionShape.polygonPoints[x];
            iterate2 = occlusionShape.outlinePoints[x];
    
            first = null;

            int i = 0;
            for(int y = 0; y < iterate1.Count; y++) {
                pA = iterate1[y];
                
                if (isEdgeCollider && first == null) {
                    first = pA.A;
                    continue;
                }

                if (i >= iterate2.Count) {
                    continue;
                }

                pB = iterate2[i];

                vertices.Add(pA.A.ToVector2());
                uvs.Add(new Vector2(0, 0));

                vertices.Add(pA.B.ToVector2());
                uvs.Add(new Vector2(1, 0));

                vertices.Add(pB.B.ToVector2());
                uvs.Add(new Vector2(1, 1));

                vertices.Add(pB.A.ToVector2());
                uvs.Add(new Vector2(0, 1));

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);

                count += 4;

                i ++;
            }
		}

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.sharedMaterial = Lighting2D.materials.GetOcclusionBlur();
    }

    void GenerateMesh_Soft() {
        double angleA, angleB, angleC;
        List<DoublePair2> iterate3;
		DoublePair2 p;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        Vector2D vA = Vector2D.Zero(), vB = Vector2D.Zero(), vC = Vector2D.Zero(), pA = Vector2D.Zero(), pB = Vector2D.Zero();
        int count = 0;

        GameObject gameObject = GetOcclusionGameObject();
        MeshRenderer meshRenderer = GetMeshRenderer();
        MeshFilter meshFilter = GetMeshFilter();
        occlusionShape = GetOcclusionShape();

        for(int x = 0; x < occlusionShape.polygonPairs.Count; x++) {
            iterate3 = occlusionShape.polygonPairs[x];

            for(int y = 0; y < iterate3.Count; y++) {
                p = iterate3[y];
            
                vA.x = p.A.x;
                vA.y = p.A.y;

                vB.x = p.B.x;
                vB.y = p.B.y;

                pA.x = p.A.x;
                pA.y = p.A.y;

                pB.x = p.B.x;
                pB.y = p.B.y;

                vC.x = p.B.x;
                vC.y = p.B.y;

                angleA = p.A.Atan2(p.B) - Mathf.PI / 2;
                angleB = p.A.Atan2(p.B) - Mathf.PI / 2;
                angleC = p.B.Atan2(p.C) - Mathf.PI / 2;

                vA.Push (angleA, occlusionSize);
                vB.Push (angleB, occlusionSize);
                vC.Push (angleC, occlusionSize);

                Vector2D ps = (vB + vC) / 2;

                float distance = Vector2.Distance(p.B, vB.ToVector2()) - 180f * Mathf.Deg2Rad;
                float rot = p.B.Atan2(ps.ToVector2());

                ps = new Vector2D( p.B );
                ps.Push(rot, distance);

                vertices.Add(pA.ToVector2());
                uvs.Add(new Vector2(0f, 0f));

                vertices.Add(pB.ToVector2());
                uvs.Add(new Vector2(0.5f, 0f));

                vertices.Add(vB.ToVector2());
                uvs.Add(new Vector2(0.5f, 1f));

                vertices.Add(vA.ToVector2());
                uvs.Add(new Vector2(0f, 1f));

                vertices.Add(ps.ToVector2());
                uvs.Add(new Vector2(1f, 1f));

                vertices.Add(vC.ToVector2());
                uvs.Add(new Vector2(0.5f, 1f));

               

                triangles.Add(count + 0);
                triangles.Add(count + 1);
                triangles.Add(count + 2);

                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 0);


                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 4);

                triangles.Add(count + 4);
                triangles.Add(count + 5);
                triangles.Add(count + 1);

                count += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.sharedMaterial = Lighting2D.materials.GetOcclusionEdge();
    }
}