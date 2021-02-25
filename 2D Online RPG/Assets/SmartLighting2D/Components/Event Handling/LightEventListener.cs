using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

[ExecuteInEditMode]
public class LightEventListener : MonoBehaviour {
    public bool useDistance = false;
    public float visability = 0;
    
    public LightCollision2D CollisionInfo = new LightCollision2D();

    private LightCollider2D lightCollider;

    private void OnEnable() {
        lightCollider = GetComponent<LightCollider2D>();

        if (lightCollider != null) {
            lightCollider.AddEvent(CollisionEvent);
        }
        
    }

    private void OnDisable() {
        if (lightCollider != null) {
            lightCollider.RemoveEvent(CollisionEvent);
        }
    }

    private void CollisionEvent(LightCollision2D collision) {
        if (collision.points != null) {
            if (CollisionInfo.state == LightEventState.None) {
                CollisionInfo = collision;

            } else {
                if (CollisionInfo.points != null) { //?
                    if (collision.points.Count >= CollisionInfo.points.Count) {
                        CollisionInfo = collision;
                    } else if (CollisionInfo.light == collision.light) {
                        CollisionInfo = collision;
                    }
                }
            }

        } else {
            CollisionInfo.state = LightEventState.None;
        }
    }

    private void Update() {
        visability = 0;

        if (CollisionInfo.state == LightEventState.None) {
            return;
        }

        if (CollisionInfo.points != null) {
            Polygon2 polygon = lightCollider.mainShape.GetPolygonsLocal()[0];

            int pointsCount = polygon.points.Length;
            int pointsInView = CollisionInfo.points.Count;

            visability = (((float)pointsInView / pointsCount));

            if (useDistance) {
                if (CollisionInfo.points.Count > 0) {
                    float multiplier = 0;

                    foreach(Vector2 point in CollisionInfo.points) {
                        float distance = Vector2.Distance(Vector2.zero, point);
                        float pointMultipler = ( 1 - (distance / CollisionInfo.light.size) ) * 2;

                        if (pointMultipler > 1) {
                            pointMultipler = 1;
                        }

                        if (pointMultipler < 0) {
                            pointMultipler = 0;
                        }

                        multiplier += pointMultipler;
                    }

                    multiplier /= CollisionInfo.points.Count;

                    visability *= multiplier;
                }
            }
        }
    
        CollisionInfo.state = LightEventState.None;
    }
}