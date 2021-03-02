using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Glow name space
[System.Serializable]
public class GlowManager {
	static public Dictionary<Sprite, GlowObject> dictionary = new Dictionary<Sprite, GlowObject>();

	static public Sprite RequestSprite(Sprite originalSprite, int glowSize, int glowIterations) {
		if (originalSprite == null) {
			Debug.LogError("Blur Manager: Requesting Null Sprite");

			return(null);
		}
		
		GlowObject glowObject = null;

		bool exist = dictionary.TryGetValue(originalSprite, out glowObject);

		if (exist) {

			if (glowObject.sprite == null || glowObject.sprite.texture == null) {
				
				dictionary.Remove(originalSprite);

				glowObject.sprite = LinearBlur.Blur(originalSprite, glowSize, glowIterations, Color.white);
				glowObject.glowSize = glowSize;
				glowObject.glowIterations = glowIterations;

				dictionary.Add(originalSprite, glowObject);

			} else if (glowObject.glowSize != glowSize || glowObject.glowIterations != glowIterations){

				glowObject.sprite = LinearBlur.Blur(originalSprite, glowSize, glowIterations, Color.white);
				glowObject.glowSize = glowSize;
				glowObject.glowIterations = glowIterations;

			}
			
			return(glowObject.sprite);
		} else {		
			Sprite sprite = LinearBlur.Blur(originalSprite, glowSize, glowIterations, Color.white);

			glowObject = new GlowObject(sprite, glowSize, glowIterations);

			dictionary.Add(originalSprite, glowObject);

			//return(null);  
	
			return(glowObject.sprite);
		}
	}
}
