using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    public bool _Classic = true;
    public bool _Help = true;



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
        return _Help;
    }

}
