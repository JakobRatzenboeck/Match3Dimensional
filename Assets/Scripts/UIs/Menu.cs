using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    GameManager GM;

    void Awake()
    {
        GM = GameManager.Instance;
        GM.OnStateChange += OnStateChangeBack;
    }

    public void OnStateChangeBack()
    {
        GM.SetGameState(GameState.Hub);
    }
}
