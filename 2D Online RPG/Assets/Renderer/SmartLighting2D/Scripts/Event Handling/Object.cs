using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;
using LightingSettings;

namespace EventHandling {

    public class Object {
        public List<LightCollider2D> listenersCache = new List<LightCollider2D>();
		
		public List<LightCollision2D> listenersInLight = new List<LightCollision2D>();
		public List<LightCollider2D> listenersInLightColliders = new List<LightCollider2D>();

		public void Update(Light2D light, EventPreset eventPreset) {
			if (light == null) {
				return;
			}

			listenersInLight.Clear();

			// Get Event Receivers
			LightCollider.GetCollisions(listenersInLight, light);

			// Remove Event Receiver Vertices with Shadows
			LightCollider.RemoveHiddenCollisions(listenersInLight, light, eventPreset);
			LightTilemap.RemoveHiddenCollisions(listenersInLight, light, eventPreset);

			if (listenersInLight.Count < 1) {
		
				for(int i = 0; i < listenersCache.Count; i++) {
					LightCollider2D collider = listenersCache[i];
					
					LightCollision2D collision = new LightCollision2D();
					collision.light = light;
					collision.collider = collider;
					collision.points = null;
					collision.state = LightEventState.OnCollisionExit;

					collider.CollisionEvent(collision);
				}

				listenersCache.Clear();

				return;
			}
				
			listenersInLightColliders.Clear();

			foreach(LightCollision2D collision in listenersInLight) {
				listenersInLightColliders.Add(collision.collider);
			}

			for(int i = 0; i < listenersCache.Count; i++) {
				LightCollider2D collider = listenersCache[i];
				if (listenersInLightColliders.Contains(collider) == false) {
					
					LightCollision2D collision = new LightCollision2D();
					collision.light = light;
					collision.collider = collider;
					collision.points = null;
					collision.state = LightEventState.OnCollisionExit;

					collider.CollisionEvent(collision);
					
					listenersCache.Remove(collider);
				}
			}

			for(int i = 0; i < listenersInLight.Count; i++) {
				LightCollision2D collision = listenersInLight[i];
				
				if (listenersCache.Contains(collision.collider)) {
					collision.state = LightEventState.OnCollision;
				} else {
					collision.state = LightEventState.OnCollisionEnter;
					listenersCache.Add(collision.collider);
				}
			
				collision.collider.CollisionEvent(collision);
			}
		}
	}
}