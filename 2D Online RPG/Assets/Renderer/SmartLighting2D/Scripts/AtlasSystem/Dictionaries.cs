using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtlasSystem {
    
    public class Dictionaries {
        private static Dictionary<Sprite, Sprite> defaultMask = new Dictionary<Sprite, Sprite>();
        private static Dictionary<Sprite, Sprite> whiteMask = new Dictionary<Sprite, Sprite>();
        private static Dictionary<Sprite, Sprite> blackMask = new Dictionary<Sprite, Sprite>();

        // Normal / White Mask / Black Mask
        public static List<Sprite> spritesDefaultMask = new List<Sprite>();
        public static List<Sprite> spritesWhiteMask = new List<Sprite>();
        public static List<Sprite> spritesBlackMask = new List<Sprite>();

        public void Clear() {
            defaultMask.Clear();
            whiteMask.Clear();
            blackMask.Clear();

            spritesDefaultMask.Clear();
            spritesWhiteMask.Clear();
            spritesBlackMask.Clear();
        }

        public Dictionary<Sprite, Sprite> Get(Request.Type type) {
            Dictionary<Sprite, Sprite> dictionary = null;
            
            switch(type) {
                case Request.Type.Normal:
                    dictionary = defaultMask;
                    break;

                case Request.Type.WhiteMask:
                    dictionary = whiteMask;
                    break;

                case Request.Type.BlackMask:
                    dictionary = blackMask; 
                    break;
            }

            return(dictionary);
        }

        public List<Sprite> GetList(Request.Type type) {
            List<Sprite> list = null;
            
            switch(type) {
                case Request.Type.Normal:
                    list = spritesDefaultMask;
                    break;

                case Request.Type.WhiteMask:
                    list = spritesWhiteMask;
                    break;

                case Request.Type.BlackMask:
                    list = spritesBlackMask; 
                    break;
            }

            return(list);
        }
    }  
}