using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System;
[InitializeOnLoad]
class Lighting2DStartup {
    static Lighting2DStartup () {

        bool icon_light = UnityEngine.Windows.File.Exists("Assets/Gizmos/light_v2.png");

        if (icon_light == false) {
            
            try {
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos", "Assets/Gizmos");
            } catch {
            }

            try {
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/light_v2.png", "Assets/Gizmos/light_v2.png");
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/fow_v2.png", "Assets/Gizmos/fow_v2.png");
                FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/circle_v2.png", "Assets/Gizmos/circle_v2.png");

            } catch {
            }
        }
    }
}
