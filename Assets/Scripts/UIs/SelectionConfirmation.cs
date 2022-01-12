using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SelectionConfirmation
{
    private GameObject confirmationInformation;
    private TMP_Text titleT;
    private TMP_Text infoT;
    private Button confirm;
    private Button back;

    public SelectionConfirmation(string title, string info, Action action)
    {
        CreateInfo();
        titleT.name = title;
        infoT.name = info;
        confirm.onClick.AddListener(delegate { Continue(action); });
    }

    public void CreateInfo()
    {
        confirmationInformation = GameObject.Instantiate(Resources.Load("Prefabs/UI/Confirmation") as GameObject);
        titleT = confirmationInformation.transform.Find("Panel/Header/Title").GetComponent<TMP_Text>();
        infoT = confirmationInformation.transform.Find("Panel/Body/Info").GetComponent<TMP_Text>();
        confirm = confirmationInformation.transform.Find("Panel/Body/Options/Continue").GetComponent<Button>();
        back = confirmationInformation.transform.Find("Panel/Body/Options/Back").GetComponent<Button>();
        back.onClick.AddListener(Back);
    }

    public void Continue(Action action)
    {
        action.Invoke();
        Back();
    }

    public void Back()
    {
        GameObject.Destroy(confirmationInformation);
    }
}
