using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (SUPER_TILEMAP_EDITOR)

    namespace SuperTilemapEditorSupport {

        [System.Serializable]
        public class TilemapCollider2D : TilemapCollider {
 
      
        }
    }

#else

 namespace SuperTilemapEditorSupport {

        [System.Serializable]
        public class TilemapCollider2D : TilemapCollider {
            
        }

        public class TilemapCollider : LightTilemapCollider.Base {
            public enum ShadowType {None, Grid, TileCollider, Collider};
            public enum MaskType {None, Grid, Sprite, BumpedSprite};

            public ShadowType shadowTypeSTE = ShadowType.Grid;
            public MaskType maskTypeSTE = MaskType.Sprite;

            public bool eventsInit;
        }
    }

#endif