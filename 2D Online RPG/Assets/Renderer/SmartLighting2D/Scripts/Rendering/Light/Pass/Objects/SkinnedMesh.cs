using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light {
        
    public class SkinnedMesh {

        public static void Mask(Light2D light, LightCollider2D id, Material material, LayerSetting layerSetting) {
			if (id.InLight(light) == false) {
				return;
			}

			foreach(LightColliderShape shape in id.shapes) {
				SkinnedMeshRenderer skinnedMeshRenderer = shape.skinnedMeshShape.GetSkinnedMeshRenderer();

				if (skinnedMeshRenderer == null) {
					return;
				}

				List<MeshObject> meshObject = shape.GetMeshes();

				if (meshObject == null) {
					return;
				}

				if (skinnedMeshRenderer.sharedMaterial != null) {
					material.mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
				} else {
					material.mainTexture = null;
				}

				Vector2 position = shape.transform2D.position - light.transform2D.position;

				Vector2 pivotPosition = shape.GetPivotPoint() - light.transform2D.position;
				material.color = LayerSettingColor.Get(pivotPosition, layerSetting, id.maskEffect, id.maskTranslucency);

				material.SetPass(0);

				GLExtended.DrawMesh(meshObject, position, id.mainShape.transform2D.scale, shape.transform2D.rotation);

				material.mainTexture = null;
			}
		}
    }
}