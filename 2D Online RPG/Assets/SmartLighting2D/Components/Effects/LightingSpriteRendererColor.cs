using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingSpriteRendererColor : MonoBehaviour {
    public int nightLayer = 0;
    public Color color;

    void Update() {
        foreach(LightSprite2D sprite in LightSprite2D.List) {
            if (sprite.nightLayer == nightLayer) {
                sprite.color = color;
            }
        }
    }
}
