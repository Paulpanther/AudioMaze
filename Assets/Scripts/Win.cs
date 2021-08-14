using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

    public static bool IsWin = false;
    public Canvas mapQuestionCanvas;
    public bool showMapQuestion = true;
	public string levelCompletedSoundName = "LevelCompleted";
    private Action winCallback = () => {};

    private void Start() {
        mapQuestionCanvas.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IsWin = true;
        if (showMapQuestion) {
            mapQuestionCanvas.enabled = true;
            AudioOut.PlayOneShotAttached(levelCompletedSoundName, gameObject);
        }
        else {
            winCallback();
        }
    }

    public void OnButtonClick() {
        Debug.Log("On Button Click");
        winCallback();
    }

    public void RegisterWinCallback(Action winCallback)
    {
        this.winCallback = winCallback;
    }
}

