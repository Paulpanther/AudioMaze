using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

	public static bool IsWin = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Enter");
		IsWin = true;
    //SceneManager.LoadScene("MenuScene");
    }
}

