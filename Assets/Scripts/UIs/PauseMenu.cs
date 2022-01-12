using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private GameManager GM;
    [SerializeField]
    private TMP_InputField saveName;
    [SerializeField]
    private Canvas pauseMenuCanvas;
    [SerializeField]
    private Transform setFileName;

    void Awake()
    {
        GM = GameManager.Instance;
        GM.OnStateChange += OnStateChangeNewGame;
        GM.OnStateChange += OnStateChangeNewGame;
        GM.OnStateChange += OnStateChangeBack;
    }

    public void OnStateChangeNewGame()
    {
        GM.SetGameState(GameState.GameHub);
    }

    public void OnStateChangeMenu()
    {
        GM.SetGameState(GameState.Menu);
    }

    public void OnStateChangeSaveAndBack()
    {
        if (GameManager.textname.Length > 0)
        {
            saveName.interactable = false;
            saveName.text = GameManager.textname;
        }
        setFileName.gameObject.SetActive(true);
        foreach (Button bn in pauseMenuCanvas.GetComponentsInChildren<Button>())
        {
            if (bn.transform.parent == pauseMenuCanvas.transform)
                bn.interactable = false;
        }
    }

    public void SetAndConfirmSavedFileName()
    {
        BlocksManager bm = FindObjectOfType<BlocksManager>();
        SaveGame sG;
        if (saveName.text.Length > 0)
        {
            sG = new SaveGame(saveName.text, bm.blocks);
            GM.SetGameState(GameState.Hub);
        }
    }

    public void Back()
    {
        saveName.text = "";
        setFileName.gameObject.SetActive(false);
        foreach (Button bn in pauseMenuCanvas.GetComponentsInChildren<Button>())
        {
            if (bn.transform.parent == pauseMenuCanvas.transform)
                bn.interactable = true;
        }
    }

    public void OnStateChangeBack()
    {
        GM.SetGameState(GameState.Hub);
    }

}
