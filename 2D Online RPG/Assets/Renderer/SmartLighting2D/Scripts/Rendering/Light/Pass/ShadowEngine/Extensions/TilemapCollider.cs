using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapCollider {

        public class Rectangle {
            static public void Draw(Light2D light, LightTilemapCollider2D id) {
                Vector2 position = -light.transform.position;

                switch(id.rectangle.shadowType) {
                    case LightTilemapCollider.ShadowType.CompositeCollider:
                        ShadowEngine.objectOffset = id.transform.position;

                        ShadowEngine.Draw(id.rectangle.compositeColliders, 0, 0);

                        ShadowEngine.objectOffset = Vector2.zero;
                    break;
                }
            }
        }
    }
}