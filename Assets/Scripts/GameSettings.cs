using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovingMethod { Continues, Teleportion, Routetracing }
public enum TurningMethod { Continues, Snap, Disabled}

public class GameSettings
{

    public struct Gameplay
    {
        public static bool _Classic = true;
        public static bool _Help = true;
        public static Vector3 _BlockSize = new Vector3(0.5f, 0.5f, 0.5f);
        public static Vector3 _BlockSpacing = new Vector3(0.15f, 0.15f, 0.15f);
        public static bool _AnimateBlocks = true; 
    }
    public struct VR
    {
        public static float _Playerhight = 1.60f;
        public static float _Floorhight = 0f;
        public static MovingMethod _MovingMethod;
        public static TurningMethod _TurningMethod;
    }

    public struct Video
    {

    }

    public struct Audio
    {
        
        public static float _Volume = 1.0f;
    }

    private static GameSettings instance = null;
    public static GameSettings Instance
    {
        get
        {
            if (GameSettings.instance == null)
            {
                GameSettings.instance = new GameSettings();
            }
            return GameSettings.instance;
        }
    }

    public bool get_Help()
    {
        return Gameplay._Help;
    }

}
