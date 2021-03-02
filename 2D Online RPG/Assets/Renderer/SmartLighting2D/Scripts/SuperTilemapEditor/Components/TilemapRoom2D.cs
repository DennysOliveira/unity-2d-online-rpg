using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (SUPER_TILEMAP_EDITOR)

    namespace SuperTilemapEditorSupport {

        public class TilemapRoom2D : TilemapCollider {
            // public enum MaskType {None, Grid, Sprite};
            // public MaskType maskType = MaskType.Sprite;
            // No Enums?
        }
    }

#else

    namespace SuperTilemapEditorSupport {
        public class TilemapRoom2D : LightTilemapCollider.Base {
            
            public override void Initialize() {}
        }
    }

#endif