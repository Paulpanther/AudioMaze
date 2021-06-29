using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

    public static bool IsWin = false;
    private Action winCallback = () => {};

    private void OnTriggerEnter2D(Collider2D other)
    {
        IsWin = true;
        winCallback();
    }

    public void RegisterWinCallback(Action winCallback)
    {
        this.winCallback = winCallback;
    }
}

