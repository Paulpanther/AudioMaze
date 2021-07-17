using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public Player player;
	public string menu;
	public Level[] levels;
	public MusicControllerFMOD fmodmusic;
	private int currentLevelIndex = -1;
	private Level currentLevel = null;

	private void Start()
	{
		NextLevel();
	}

	private void NextLevel()
	{
		Debug.Log("Next Level");
		if (currentLevelIndex + 1 >= levels.Length)
		{
			SceneManager.LoadScene(menu);
			Debug.Log("loading menu");
			fmodmusic.NewMusicLevel(null,null);
			return;
		}

		if (currentLevel != null) 
		{ 
			currentLevel.Destroy(); 
		}
		currentLevel = Instantiate(levels[++currentLevelIndex]);
		Debug.Log("calling music inst");
		fmodmusic.NewMusicLevel(currentLevel.music, player);
		currentLevel.win.RegisterWinCallback(NextLevel);
		player.RegisterLevel(currentLevel);
        EventLogging.logEvent(new LevelEvent(currentLevel.name));
		
	}
}
