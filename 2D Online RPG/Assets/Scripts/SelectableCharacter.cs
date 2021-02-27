using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Mirror;

[RequireComponent(typeof(PlayerIndicator))]
[DisallowMultipleComponent]
public class SelectableCharacter : MonoBehaviour
{
   // index will be set by NetworkManager when creating this script
   public int index = -1;

   void OnMouseDown()
   {
       // set selection index
       ((MyNetworkManager)MyNetworkManager.singleton).selection = index;

       // show selection indicator for better feedback
       GetComponent<PlayerIndicator>().SetViaParent(transform);
   }

   void Update()
   {
       // remove indicator if not selected anymore
       if (((MyNetworkManager)MyNetworkManager.singleton).selection != index)
       {
           GetComponent<PlayerIndicator>().Clear();
       }
   }
}
