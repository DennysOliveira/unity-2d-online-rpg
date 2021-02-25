using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public static class Lighting2D {
	public const int VERSION = 20200101;
	public const string VERSION_STRING = "2020.1.1";

	static public Lighting2DMaterials materials = new Lighting2DMaterials();

	// Disable
	static public bool disable {
		set => Profile.disable = value;
		get => Profile.disable;
	}

	// Buffer Settings
	static public BufferPreset[] BufferPresets {
		get => Profile.bufferPresets.list;
	}

	// Common Settings
	static public LightingSettings.QualitySettings QualitySettings {
		get => Profile.qualitySettings;
	}
	// Day Settings
	static public DayLightingSettings DayLightingSettings {
		get => Profile.dayLightingSettings;
	}

	// Fog of War
	static public LightingSettings.FogOfWar FogOfWar {
		get => Profile.fogOfWar;
	}

	static public RenderingMode RenderingMode {
		get {
			if (projectSettings == null) {
				return(RenderingMode.OnPostRender);
			}

			return(ProjectSettings.renderingMode);
		}
	}

	static public CoreAxis CoreAxis {
		get {
			return(Profile.qualitySettings.coreAxis);
		}
	}

	// Set & Get API
	static public Color DarknessColor {
		get { return BufferPresets[0].darknessColor; }
		set { BufferPresets[0].darknessColor = value; }
	}

	static public float Resolution {
		get { return BufferPresets[0].lightingResolution; }
		set { BufferPresets[0].lightingResolution = value; }
	}

	// Methods
	static public void UpdateByProfile(Profile setProfile) {
		if (setProfile == null) {
			Debug.Log("Light 2D: Update Profile is Missing");
			return;
		}
		
		// Set profile also
		profile = setProfile;
	}

	static public void RemoveProfile() {
		profile = null;
	}

	// Profile
	static private Profile profile = null;
	static public Profile Profile {
		get {
			if (profile != null) {
				return(profile);
			}

			if (ProjectSettings != null) {
				profile = ProjectSettings.Profile;
			}

			if (profile == null) {
				profile = Resources.Load("Profiles/Default Profile") as Profile;

				if (profile == null) {
					Debug.LogError("Light 2D: Default Profile not found");
				}
			}

			return(profile);
		}
	}

	static private ProjectSettings projectSettings;
	static public ProjectSettings ProjectSettings {
		get {
			if (projectSettings != null) {
				return(projectSettings);
			}

			//MyScriptableObjectClass asset = ScriptableObject.CreateInstance<MyScriptableObjectClass>();

			//AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
			//AssetDatabase.SaveAssets();

			//EditorUtility.FocusProjectWindow();

			projectSettings = Resources.Load("Settings/Project Settings") as ProjectSettings;

			if (projectSettings == null) {
				Debug.LogError("Light 2D: Project Settings not found");
				return(null);
			}
		
			return(projectSettings);
		}
	}
}