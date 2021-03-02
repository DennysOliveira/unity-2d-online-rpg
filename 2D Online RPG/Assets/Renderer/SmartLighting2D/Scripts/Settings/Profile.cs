using UnityEngine;

namespace LightingSettings {
	[CreateAssetMenu(fileName = "Data", menuName = "Light 2D/Profile", order = 1)]

	public class Profile : ScriptableObject {
		public BufferPresetList bufferPresets;

		public LightPresetList lightPresets;

		public EventPresetList eventPresets;

		public QualitySettings qualitySettings;

		public DayLightingSettings dayLightingSettings;

		public FogOfWar fogOfWar;

		public Layers layers;

		public bool disable = false;

		public Color DarknessColor
		{
			get => bufferPresets.list[0].darknessColor;

			set => bufferPresets.list[0].darknessColor = value;
		}

		public Profile() {
			layers = new Layers();

			qualitySettings = new QualitySettings();

			bufferPresets = new BufferPresetList();
			bufferPresets.list[0] = new BufferPreset(0);
			bufferPresets.list[0].darknessColor = new Color(0, 0, 0, 1);

			lightPresets = new LightPresetList();
			lightPresets.list[0] = new LightPreset(0);

			eventPresets = new EventPresetList();
			eventPresets.list[0] = new EventPreset(0);
			eventPresets.list[1] = new EventPreset(1);
			
			dayLightingSettings = new DayLightingSettings();

			fogOfWar = new FogOfWar();
		}
	}
}