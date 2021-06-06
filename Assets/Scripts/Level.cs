using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public Win win;

	public void RegisterWinCallback(Action callback)
	{
		win.RegisterCallback(callback);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
