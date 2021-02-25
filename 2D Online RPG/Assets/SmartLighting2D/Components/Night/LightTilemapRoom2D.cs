using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using LightTilemapCollider;
using LightingSettings;

 //ITilemap.GetSprite(Vector3)
 
#if UNITY_2017_4_OR_NEWER

    using UnityEngine.Tilemaps;

    [ExecuteInEditMode]
    public class LightTilemapRoom2D : MonoBehaviour {
        public int nightLayer = 0;
        public enum MaskType {Sprite}  // Separate For Each Map Type!
        public enum ShaderType {ColorMask, MultiplyTexture};
    
        public MapType mapType = MapType.UnityRectangle;
        public MaskType maskType = MaskType.Sprite;
        public ShaderType shaderType = ShaderType.ColorMask;
        public Color color = Color.black;

        public SuperTilemapEditorSupport.TilemapRoom2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapRoom2D();
        public Rectangle rectangle = new Rectangle();

        public LightingTilemapRoomTransform lightingTransform = new LightingTilemapRoomTransform();
	

        public static List<LightTilemapRoom2D> List = new List<LightTilemapRoom2D>();


        public void OnEnable() {
            List.Add(this);

            LightingManager2D.Get();

			rectangle.SetGameObject(gameObject);
            superTilemapEditor.SetGameObject(gameObject);

            Initialize();
        }

        public void OnDisable() {
            List.Remove(this);
        }

        public LightTilemapCollider.Base GetCurrentTilemap() {
			switch(mapType) {
				case MapType.SuperTilemapEditor:
					return(superTilemapEditor);
				case MapType.UnityRectangle:
					return(rectangle);

			}
			return(null);
		}

        public void Initialize() {
			TilemapEvents.Initialize();
			
            GetCurrentTilemap().Initialize();
        }

        public void Update() {
			lightingTransform.Update(this);

			if (lightingTransform.UpdateNeeded) {

                GetCurrentTilemap().ResetWorld();

				Light2D.ForceUpdateAll();
			}
		}

        public TilemapProperties GetTilemapProperties() {
			return(GetCurrentTilemap().Properties);
		}

        public List<LightingTile> GetTileList() {
			return(GetCurrentTilemap().mapTiles);
		}

        public float GetRadius() {
			return(GetCurrentTilemap().GetRadius());
		}

        void OnDrawGizmosSelected() {
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected) {
				return;
			}

			DrawGizmos();
		}

		private void OnDrawGizmos() {
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) {
				return;
			}

			DrawGizmos();
		}

		private void DrawGizmos() {
			if (isActiveAndEnabled == false) {
				return;
			}

			// Gizmos.color = new Color(1f, 0.5f, 0.25f);

			Gizmos.color = new Color(0, 1f, 1f);

            switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds) {
				case EditorGizmosBounds.Rectangle:
					GizmosHelper.DrawRect(transform.position, GetCurrentTilemap().GetRect());
				break;
			}
		}
    }

#endif