using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Light2DWindow : EditorWindow {
	private int tab = 0;
	private Vector2 scrollPosition;

	static public Light2DWindow GetWindow(){ 
		Light2DWindow editorWindow = GetWindow<Light2DWindow>(false, "2D Light", true);

		return(editorWindow);
	}

	[MenuItem("Tools/2D Light")]
    public static void ShowWindow() {
        UpdateWindow();
    }

	public static void UpdateWindow() {
		Light2DWindow editorWindow = GetWindow();

		editorWindow.minSize = new Vector2(350, 350);
		editorWindow.maxSize = new Vector2(4000, 800);
	}

	void OnGUI() {
		tab = GUILayout.Toolbar (tab, new string[] { "Profile", "Project"});

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true,  GUILayout.Width(Screen.width - 2),  GUILayout.Height(Screen.height - 45 - 2)); 
		
		EditorGUILayout.Space();

		switch (tab) {
			case 0:
				ProfileEditor.Draw();
				break;

			case 1:
				ProjectSettingsEditor.Draw();
				break;
		}

		 GUILayout.EndScrollView();
    }
}