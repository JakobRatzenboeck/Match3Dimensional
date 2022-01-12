using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainHub : MonoBehaviour
{
    GameManager GM;

    void Awake()
    {
        GM = GameManager.Instance;
        GM.OnStateChange += HandleOnMenuChange;
        GM.OnStateChange += HandleOnPlayMenuChange;
    }

    public void HandleOnPlayMenuChange()
    {
        GM.SetGameState(GameState.GameHub);
    }

    public void HandleOnMenuChange()
    {
        GM.SetGameState(GameState.Menu);
    }

    public void HandleOnExitChange()
    {
        if (Application.isEditor)
            EditorApplication.Exit(0);
        else
            Application.Quit();
    }




}
