using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameManager GM;
    [SerializeField]
    private Button newGame;
    [SerializeField]
    private Button back;


    void Awake()
    {
        GM = GameManager.Instance;
        GM.OnStateChange += OnStateChangeNewGame;
        GM.OnStateChange += OnStateChangeBack;
    }


    public void OnStateChangeNewGame()
    {
        GM.SetGameState(GameState.GameHub);
    }

    public void OnStateChangeBack()
    {
        GM.SetGameState(GameState.Hub);
    }
}
