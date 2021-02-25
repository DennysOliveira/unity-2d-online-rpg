using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day {

    public class Shadow {

        static public void Draw(DayLightCollider2D id, Vector2 position) {
            if (id.mainShape.shadowType == DayLightCollider2D.ShadowType.None) {
                return;
            }

            if (id.mainShape.shadowType == DayLightCollider2D.ShadowType.Sprite) {
                return;
            }

            if (id.mainShape.height < 0) {
                return;
            }
        
            if (id.InAnyCamera() == false) {
                return;
            }

            Color color = new Color(id.shadowTranslucency, id.shadowTranslucency, id.shadowTranslucency, 1);
            GL.Color(color);

            foreach(DayLightColliderShape shape in id.shapes) {
                DayLighting.ShadowMesh shadow = shape.shadowMesh;
                
                if (shadow == null) {
                    continue;
                }
                
                Vector3 pos = new Vector3(shape.transform2D.position.x + position.x, shape.transform2D.position.y + position.y, 0);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, 0), Vector3.one);

                foreach(MeshObject mesh in shadow.softMeshes) {
                    GLExtended.DrawMeshPass(mesh, pos, Vector3.one, 0);
                }

                foreach(MeshObject mesh in shadow.meshes) {
                    GLExtended.DrawMeshPass(mesh, pos, Vector3.one, 0);
                } 
            }
        }


          static public void DrawTilemap(DayLightTilemapCollider2D id, Vector2 position) {
            //if (id.mainShape.shadowType == DayLightCollider2D.shadowType.None) {
            //    return;
            //}

           // if (id.mainShape.shadowType == DayLightCollider2D.shadowType.Sprite) {
            //    return;
            //}

            //if (id.mainShape.height < 0) {
            //    return;
            //}//

            //if (id.InAnyCamera() == false) {
            //     continue;
            //}

            // shadowTranslucency
            GL.Color(Color.black);

            foreach(DayLightingTile dayTile in id.dayTiles) {
                DayLighting.TilemapShadowMesh shadow = dayTile.shadowMesh;
                
                if (shadow == null) {
                    continue;
                }

                Vector3 pos = new Vector3(position.x, position.y, 0);
                Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, 0), Vector3.one);

                foreach(MeshObject mesh in shadow.softMeshes) {
                    GLExtended.DrawMeshPass(mesh, pos, Vector3.one, 0);
                }

                foreach(MeshObject mesh in shadow.meshes) {
                    GLExtended.DrawMeshPass(mesh, pos, Vector3.one, 0);
                } 
            }
        }
    }
}