using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class SavedGames : MonoBehaviour
{
    GameManager GM;
    [SerializeField]
    private Transform contentContainer;
    [SerializeField]
    private GameObject containerPref;
    [SerializeField]
    private Canvas menu;


    private TextAsset[] txts;


    void Awake()
    {
        GM = GameManager.Instance;
        var objtxts = Resources.LoadAll(GameManager.Constants._SavedFilesPath);
        txts = new TextAsset[objtxts.Length];
        for (int i = 0; i < objtxts.Length; i++)
        {
            txts[i] = objtxts[i] as TextAsset;
        }
    }

    void Start()
    {
        ClearContentContainer();
        CreateContainer();
    }

    private void ClearContentContainer()
    {
        for (int i = 0; i < contentContainer.childCount; i++)
        {
            Destroy(contentContainer.GetChild(i).gameObject);
        }
    }

    private void CreateContainer()
    {
        for (int i = 0; i < txts.Length; i++)
        {
            Transform temp = Instantiate(containerPref).transform;
            Debug.Log($"{i}: {temp.name} ({temp.position})");  
            temp.name = txts[i].name;
            temp.GetChild(0).GetComponent<TMP_Text>().text = txts[i].name;
            TextAsset txt = Resources.Load(GameManager.Constants._SavedFilesPath + txts[i].name) as TextAsset;
            try
            {
                string[] date = txt.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split(';')[0].Split('.');
                temp.GetChild(1).GetComponent<TMP_Text>().text = $"({date[0]}/{date[1]}/{date[2]})";
                temp.GetComponent<Button>().onClick.AddListener(delegate { openSaveFile(temp.name); });
                temp.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { DeleteSaveFile(temp.name); });
            }
            catch (System.Exception)
            {
                temp.GetChild(1).GetComponent<TMP_Text>().text = "Corrupted";
                temp.GetComponent<Button>().interactable = false;
                temp.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
            }
            temp.SetParent(contentContainer);
        }
    }

    public void openSaveFile(string filename)
    {
        //Debug.Log(filename);
        GameManager.textname = filename;
        GM.SetGameState(GameState.ContinuedGame);
    }

    public void DeleteSaveFile(string filename)
    {
        SelectionConfirmation sC = new SelectionConfirmation("Delete Savefile", "Are you sure that you want to delete this file?\nDeleted files can't be restored.", delegate { DeleteFile(filename); });
    }

    private void DeleteFile(string filename)
    {
        File.Delete(GameManager.Constants._FullSavedFilesPath + filename + ".txt");
        Destroy(contentContainer.Find(filename).gameObject);
        DeleteEntry(filename);
    }

    private void DeleteEntry(string filename)
    {
        int index = 0;
        for (int i = 0; i < txts.Length; i++)
        {
            if (txts[i] != null && txts[i].name == filename)
                index = i;
        }
        for (int i = index; i < txts.Length - 1; i++)
        {
            txts[i] = txts[i + 1];
            txts[i + 1] = null;
        }
    }


    public void closeWindow()
    {
        foreach (Selectable sb in menu.transform.GetComponentsInChildren<Selectable>())
        {
            sb.interactable = true;
        }
        gameObject.SetActive(false);
    }

}
