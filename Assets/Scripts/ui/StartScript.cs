using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{

    public string winTitleText;
    public string winButtonText;
    public TextMeshProUGUI title;
    public Button button;

    void Start()
    {
        if (Win.IsWin)
        {
            title.text = winTitleText;
            button.GetComponentInChildren<TextMeshProUGUI>().text = winButtonText;
        }
    }

    public void OnClick()
    {
        Win.IsWin = false;
        SceneManager.LoadScene("MainScene");
    }
}
