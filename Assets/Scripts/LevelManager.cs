﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public Player player;
	public string menu;
	public Level[] levels;

	private int currentLevelIndex = -1;
	private Level currentLevel = null;

	private void Start()
	{
		NextLevel();
	}

	private void NextLevel()
	{
		if (currentLevelIndex + 1 >= levels.Length)
		{
			SceneManager.LoadScene(menu);
			return;
		}

		if (currentLevel != null) currentLevel.Destroy();
		currentLevel = Instantiate(levels[++currentLevelIndex]);
		currentLevel.win.RegisterWinCallback(NextLevel);
		player.RegisterLevel(currentLevel);
	}
}