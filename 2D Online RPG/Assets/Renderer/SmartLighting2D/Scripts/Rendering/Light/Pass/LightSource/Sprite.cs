using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.LightSprite {

    public class Sprite {

        static public void Draw(Vector2 pos, Vector2 size, float rot, float z, bool flipX, bool flipY) {
            Vector2 scale = new Vector2(size.x, size.y);

            if (flipY) {
                scale.y = -scale.y;
            }

            if (flipX) {
                scale.x = -scale.x;
            }

            Rendering.Universal.Texture.Draw(pos, scale, rot, z);
        }
    }
}