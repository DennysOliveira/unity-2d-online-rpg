using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
    
    public class Shape {

        public static void Mask(Light2D light, LightCollider2D id, LayerSetting layerSetting) {
            if (id.InLight(light) == false) {
                return;
            }

            int shapeCount = id.shapes.Count;

            for(int i = 0; i < shapeCount; i++) {
                LightColliderShape shape = id.shapes[i];

                List<MeshObject> meshObjects = shape.GetMeshes();

                if (meshObjects == null) {
                    return;
                }
                            
                Vector2 position = shape.transform2D.position - light.transform2D.position;

                Vector2 pivotPosition = shape.GetPivotPoint() - light.transform2D.position;
                GL.Color(LayerSettingColor.Get(pivotPosition, layerSetting, id.maskEffect, id.maskTranslucency));

                GLExtended.DrawMeshPass(meshObjects, position, shape.transform.lossyScale, shape.transform2D.rotation);
            }
        }
    }
}