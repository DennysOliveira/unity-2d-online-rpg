using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtlasSystem {

    public class Request {

        static public List<Request> requestList = new List<Request>();

        // Normal = Black Alpha
        public enum Type {Normal, WhiteMask, BlackMask};
        public Sprite sprite;
        public Type type;

        public Request (Sprite s, Type t) {
            sprite = s;
            type = t;
        }

        static public void Update() {
            foreach(Request req in requestList) {
                float timer = Time.realtimeSinceStartup;

                AtlasSystem.Manager.RequestAccess(req.sprite, req.type);

                LightingDebug.atlasTimer += (Time.realtimeSinceStartup - timer);
            }

            requestList.Clear();
        }
    }
}