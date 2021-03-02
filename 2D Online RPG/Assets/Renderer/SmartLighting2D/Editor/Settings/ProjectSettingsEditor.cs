using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;

public class ProjectSettingsEditor {

    static public void Draw() {
        EditorGUI.BeginChangeCheck ();

        LightingSettings.ProjectSettings mainProfile = Lighting2D.ProjectSettings;

        mainProfile.Profile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Default Profile", mainProfile.Profile, typeof(LightingSettings.Profile), true);

        EditorGUILayout.Space();

        mainProfile.renderingMode = (RenderingMode)EditorGUILayout.EnumPopup("Rendering Mode", mainProfile.renderingMode);   
            
        EditorGUILayout.Space();

        mainProfile.colorSpace = (LightingSettings.ColorSpace)EditorGUILayout.EnumPopup("Color Space", mainProfile.colorSpace);

        EditorGUILayout.Space();

        mainProfile.managerInstance = (LightingSettings.ManagerInstance)EditorGUILayout.EnumPopup("Manager Instance", mainProfile.managerInstance);

        mainProfile.managerInternal = (LightingSettings.ManagerInternal)EditorGUILayout.EnumPopup("Manager Internal", mainProfile.managerInternal);

        EditorGUILayout.Space();

        mainProfile.MaxLightSize = EditorGUILayout.IntSlider("Max Light Size", mainProfile.MaxLightSize, 10, 1000);

        EditorGUILayout.Space();

        EditorView.Draw(mainProfile);

        EditorGUILayout.Space();

        Chunks.Draw(mainProfile);

        EditorGUI.EndChangeCheck ();

        if (GUI.changed) {
            LightingManager2D.ForceUpdate();
            Lighting2D.UpdateByProfile(mainProfile.Profile);

            EditorUtility.SetDirty(mainProfile);
        }
    }


    public class Chunks {
        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Chunks", mainProfile.chunks);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;   

            EditorGUILayout.Space();


            
            mainProfile.chunks.enabled = EditorGUILayout.Toggle("Enable", mainProfile.chunks.enabled);

            mainProfile.chunks.chunkSize = EditorGUILayout.IntSlider("Chunk Size", mainProfile.chunks.chunkSize, 10, 100);


            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }
    }

    

    public class EditorView {
        public static void Draw(LightingSettings.ProjectSettings mainProfile) {
            bool foldout = GUIFoldoutHeader.Begin("Editor View", mainProfile.editorView);

            if (foldout == false) {
                GUIFoldoutHeader.End();
                return;
            }

            EditorGUI.indentLevel++;   

            EditorGUILayout.Space();
  
            mainProfile.editorView.drawGizmos = (EditorDrawGizmos)EditorGUILayout.EnumPopup("Draw Gizmos", mainProfile.editorView.drawGizmos);

            mainProfile.editorView.drawGizmosBounds = (EditorGizmosBounds)EditorGUILayout.EnumPopup("Gizmos Bounds", mainProfile.editorView.drawGizmosBounds);

            EditorGUILayout.Space();

            mainProfile.editorView.gameViewLayer = EditorGUILayout.LayerField("Game Layer (Default)", mainProfile.editorView.gameViewLayer);

            mainProfile.editorView.sceneViewLayer = EditorGUILayout.LayerField("Scene Layer (Default)", mainProfile.editorView.sceneViewLayer);

            EditorGUILayout.Space();

            mainProfile.editorView.fowGameViewLayer = EditorGUILayout.LayerField("FOW Game Layer (Default)", mainProfile.editorView.fowGameViewLayer);

            mainProfile.editorView.fowSceneViewLayer = EditorGUILayout.LayerField("FOW Scene Layer (Default)", mainProfile.editorView.fowSceneViewLayer);

            EditorGUI.indentLevel--;

            GUIFoldoutHeader.End();
        }
    }

   
}
