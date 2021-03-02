using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteExtension {
	
	public static class PhysicsShapeManager {
		static public Dictionary<Sprite, PhysicsShape> dictionary = new Dictionary<Sprite, PhysicsShape>();

		static public void Clear() {
			dictionary = new Dictionary<Sprite, PhysicsShape>();
		}

		static public PhysicsShape RequesCustomShape(Sprite originalSprite) {
			if (originalSprite == null) {
				return(null);
			}
			
			PhysicsShape shape = null;

			bool exist = dictionary.TryGetValue(originalSprite, out shape);

			if (exist) {
				if (shape == null || shape.GetSprite().texture == null) {
					shape = RequestCustomShapeAccess(originalSprite);
				} 
				return(shape);
			} else {
				shape = RequestCustomShapeAccess(originalSprite);
				return(shape);
			}
		}

		static public PhysicsShape RequestCustomShapeAccess(Sprite originalSprite) {
			PhysicsShape shape = null;

			bool exist = dictionary.TryGetValue(originalSprite, out shape);

			if (exist) {
				if (shape == null || shape.GetSprite().texture == null) {
					dictionary.Remove(originalSprite);

					shape = AddShape(originalSprite);

					dictionary.Add(originalSprite, shape);
				} 
				return(shape);
			} else {		
				shape = AddShape(originalSprite);

				dictionary.Add(originalSprite, shape);

				return(shape);
			}
		}

	static private PhysicsShape AddShape(Sprite sprite) {
			if (sprite == null || sprite.texture == null) {
				return(null);
			}

			PhysicsShape shape = new PhysicsShape();
			shape.SetSprite(sprite);

			return(shape);
		}
	}
}