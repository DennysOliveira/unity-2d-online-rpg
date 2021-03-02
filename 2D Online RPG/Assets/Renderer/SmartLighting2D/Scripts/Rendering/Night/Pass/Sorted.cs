using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night {

    public class Sorted {
        
         static public void Draw(Pass pass) {
             for(int id = 0; id < pass.sortList.count; id++) {
                Sorting.SortObject sortObject = pass.sortList.list[id];

                switch(sortObject.type) {
                     case Sorting.SortObject.Type.TilemapRoom:
                        LightTilemapRoom2D tilemapRoom = (LightTilemapRoom2D)sortObject.lightObject;

                        if (tilemapRoom != null) {
                            TilemapRoom.Draw(tilemapRoom, pass.camera);                            
                        }

                    break;

                    case Sorting.SortObject.Type.Room:
                        LightRoom2D room = (LightRoom2D)sortObject.lightObject;

                        if (room != null) {
                            Room.Draw(room, pass.camera);
                        }

                    break;

                    case Sorting.SortObject.Type.LightSprite:
                        LightSprite2D lightSprite = (LightSprite2D)sortObject.lightObject;

                        if (lightSprite != null) {
                            LightSprite.Draw(lightSprite, pass.camera);
                        }

                    break;

                    case Sorting.SortObject.Type.Light:
                        Light2D light = (Light2D)sortObject.lightObject;

                        if (light != null) {
                            LightSource.Draw(light, pass.camera);
                        }

                    break;
                }
            }
         }
    }
}