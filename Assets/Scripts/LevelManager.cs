using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public Player player;
	public string menu;
	public Level[] levels;
	public BackgroundMusic fmodmusic;
	private int currentLevelIndex = -1;
	private Level currentLevel = null;
	public string levelCompletedSoundName = "LevelCompleted";

	private void Start()
	{
		NextLevel();
	}

	private void NextLevel()
	{
		if (currentLevelIndex + 1 >= levels.Length)
		{
        	EventLogging.logEvent(new LevelEvent("<COMPLETED>"));
			SceneManager.LoadScene(menu);
			return;
		}

		if (currentLevel != null) {
			fmodmusic.StopBackgroundMusic();
			currentLevel.Destroy();
			PlayLevelCompletedSound();
		}
		currentLevel = Instantiate(levels[++currentLevelIndex]);
        EventLogging.logEvent(new LevelEvent(currentLevel.name));
		fmodmusic.UpdateBackgroundMusic(currentLevel.musicName, player);
		currentLevel.win.RegisterWinCallback(NextLevel);
		player.RegisterLevel(currentLevel);
	}

    private void PlayLevelCompletedSound() {
        AudioOut.PlayOneShotAttached(levelCompletedSoundName, player.gameObject);
    }
}
