
namespace LightingSettings {

	public enum RenderingMode {
		OnRender = 2,
		OnPostRender = 1,
		OnPreRender = 0	
	}

	public enum CoreAxis {
		XY,
		XYFLIPPED,
		XZ,
		XZFLIPPED
	}

	public enum Projection {
		Orthographic
	}

	public enum LightingSourceTextureSize {
		Custom,
		px2048, 
		px1024, 
		px512, 
		px256, 
		px128,
		PixelPerfect
	}

	public enum ColorSpace {
		Gamma, 
		Linear
	}
}