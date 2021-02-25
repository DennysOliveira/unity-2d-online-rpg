using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSource {

    public static class Bounds {
        private static MeshObject meshObject = null;

        private static MeshObject pixelPerfectMeshObject = null;

        public static void Draw(Light2D light, Vector2 position, Material material, float z) {
            float rotation = light.transform.rotation.eulerAngles.z + (Mathf.PI / 4) * Mathf.Rad2Deg;
            float size = light.size;
            size = Mathf.Sqrt(((size * size) + (size * size)));

            Vector3 matrixPosition = new Vector3(position.x, position.y, z);
            Quaternion matrixRotation = Quaternion.Euler(0, 0, rotation);
            Vector3 matrixScale = new Vector3(size, size, 1);

            if (light.IsPixelPerfect()) {
                GLExtended.DrawMesh(GetMeshPixelPerfect(light), matrixPosition, matrixScale, rotation);
            } else {
                GLExtended.DrawMesh(GetMesh(), matrixPosition, matrixScale, rotation);
            }
        }

        private static MeshObject GetMeshPixelPerfect(Light2D light) {
            if (pixelPerfectMeshObject == null) {
                Camera camera = Camera.main;

                Rect rect = CameraTransform.GetWorldRect(camera);

                CalculatePoints();
          
                CalculateOffsets(rect.height * 1.1f / light.size);

                pixelPerfectMeshObject = GenerateMesh();
            }

            return(pixelPerfectMeshObject);
        }

        private static MeshObject GetMesh() {
            if (meshObject == null) {
                CalculatePoints();
                CalculateOffsets(1);

                meshObject = GenerateMesh();
            }

            return(meshObject);
        }

        private static MeshObject GenerateMesh() {
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int count = 0;

            vertices.Add(right0);
            vertices.Add(right1);
            vertices.Add(right3);
            vertices.Add(right2);

            triangles.Add(count + 0);
            triangles.Add(count + 1);
            triangles.Add(count + 2);

            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count + 0);
            
            count += 4;

            vertices.Add(left0);
            vertices.Add(left1);
            vertices.Add(left3);
            vertices.Add(left2);

            triangles.Add(count + 0);
            triangles.Add(count + 1);
            triangles.Add(count + 2);

            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count + 0);

            count += 4;


            vertices.Add(down0);
            vertices.Add(down1);
            vertices.Add(down3);
            vertices.Add(down2);

            triangles.Add(count + 0);
            triangles.Add(count + 1);
            triangles.Add(count + 2);

            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count + 0);


            count += 4;

            vertices.Add(up0);
            vertices.Add(up1);
            vertices.Add(up3);
            vertices.Add(up2);

            triangles.Add(count + 0);
            triangles.Add(count + 1);
            triangles.Add(count + 2);

            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count + 0);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            return(MeshObject.Get(mesh));
        }

        private static void CalculatePoints() {
            angle0 = angle0.RotToVec(0, 1);
            angle90 = angle90.RotToVec(0 + Mathf.PI / 2);
            angle180 = angle180.RotToVec(0 + Mathf.PI);
            angle270 = angle270.RotToVec(0 - Mathf.PI / 2);

            angle45 = angle45.RotToVec(0 + Mathf.PI / 4);
            angle135 = angle135.RotToVec(0 + Mathf.PI / 2 + Mathf.PI / 4);
            angle225 = angle225.RotToVec(0 + Mathf.PI + Mathf.PI / 4);
            angle315 = angle315.RotToVec(0 - Mathf.PI / 2 + Mathf.PI / 4);
        }

        private static void CalculateOffsets(float size) {

            // Up
            up0.x = angle90.x + angle135.x * size;
            up0.y = angle90.y + angle135.y * size;

            up1.x = up0.x + angle45.x * size;
            up1.y = up0.y + angle45.y * size;

            up2.x = angle0.x + angle315.x * size;
            up2.y = angle0.y + angle315.y * size;

            up3.x = up2.x + angle45.x * size;
            up3.y = up2.y + angle45.y * size;

            // Down
            down0.x = angle270.x + angle315.x * size;
            down0.y = angle270.y + angle315.y * size;

            down1.x = down0.x + angle225.x * size;
            down1.y = down0.y + angle225.y * size;

            down2.x = angle180.x + angle135.x * size;
            down2.y = angle180.y + angle135.y * size;

            down3.x = down2.x + angle225.x * size;
            down3.y = down2.y + angle225.y * size;
        
            // Left
            left0.x = angle0.x + angle45.x * size;
            left0.y = angle0.y + angle45.y * size;

            left1.x = left0.x + angle315.x * size;
            left1.y = left0.y + angle315.y * size;

            left2.x = angle270.x + angle225.x * size;
            left2.y = angle270.y + angle225.y * size;

            left3.x = left2.x + angle315.x * size;
            left3.y = left2.y + angle315.y * size;
                
            // Right
            right0.x = angle180.x + angle225.x * size;
            right0.y = angle180.y + angle225.y * size;

            right1.x = right0.x + angle135.x * size;
            right1.y = right0.y + angle135.y * size;

            right2.x = angle90.x + angle45.x * size;
            right2.y = angle90.y + angle45.y * size;

            right3.x = right2.x + angle135.x * size;
            right3.y = right2.y + angle135.y * size;
        }



        static Vector2 angle0 = Vector2.zero; 
        static Vector2 angle90 = Vector2.zero; 
        static Vector2 angle180 = Vector2.zero; 
        static Vector2 angle270 = Vector2.zero; 

        static Vector2 angle45 = Vector2.zero; 
        static Vector2 angle135 = Vector2.zero; 
        static Vector2 angle225 = Vector2.zero; 
        static Vector2 angle315 = Vector2.zero; 

        static Vector2 up0 = Vector2.zero; 
        static Vector2 up1 = Vector2.zero; 
        static Vector2 up2 = Vector2.zero; 
        static Vector2 up3 = Vector2.zero; 

        static Vector2 down0 = Vector2.zero; 
        static Vector2 down1 = Vector2.zero; 
        static Vector2 down2 = Vector2.zero; 
        static Vector2 down3 = Vector2.zero; 

        static Vector2 left0 = Vector2.zero; 
        static Vector2 left1 = Vector2.zero; 
        static Vector2 left2 = Vector2.zero; 
        static Vector2 left3 = Vector2.zero; 

        static Vector2 right0 = Vector2.zero; 
        static Vector2 right1 = Vector2.zero; 
        static Vector2 right2 = Vector2.zero; 
        static Vector2 right3 = Vector2.zero; 
    }

}
