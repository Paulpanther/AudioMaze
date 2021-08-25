using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public Player player;
	public string menu;
    public Level training;
    public Level level1;
    public Level[] levels2;
    public Level[] levels3;
    public Level[] levels4;
	public BackgroundMusic fmodmusic;
	private Level[] levels;
	private int currentLevelIndex = -1;
	private Level currentLevel = null;
	private bool firstScenarioIs1;
	public string levelCompletedSoundName = "LevelCompleted";

	private void Start()
	{
        var levelString = StartScript.levelCode;
        firstScenarioIs1 = levelString[0] == '1';
        var secondLevelIsA = levelString[1] == 'a';
        var thirdLevelIsA = levelString[2] == 'a';
        var fourthLevelIsA = levelString[3] == 'a';
        levels = new [] {
	        training,
	        level1,
	        secondLevelIsA ? levels2[0] : levels2[1],
	        thirdLevelIsA ? levels3[0] : levels3[1],
	        fourthLevelIsA ? levels4[0] : levels4[1],
	        training,
	        level1,
	        !secondLevelIsA ? levels2[0] : levels2[1],
	        !thirdLevelIsA ? levels3[0] : levels3[1],
	        !fourthLevelIsA ? levels4[0] : levels4[1],
        };
		EventLogging.logEvent(new GameStartEvent(levelString));
        NextLevel();
	}

	private void NextLevel()
	{
		if (currentLevelIndex + 1 >= levels.Length)
		{
        	EventLogging.logEvent(new LevelEvent(null));
			SceneManager.LoadScene(menu);
			return;
		}

		if (currentLevel != null) {
			fmodmusic.StopBackgroundMusic();
			currentLevel.Destroy();
			PlayLevelCompletedSound();
		}

		var scenario1 = firstScenarioIs1;
		if (currentLevelIndex + 1 >= 5) {
			scenario1 = !scenario1;
		}

		if(!scenario1) player.SetRelativeGoalOrientation();
		else           player.SetAbsoluteGoalOrientation();

		currentLevel = Instantiate(levels[++currentLevelIndex]);
        EventLogging.logEvent(new LevelEvent(currentLevel));
		fmodmusic.UpdateBackgroundMusic(currentLevel.musicName, player);
		currentLevel.win.RegisterWinCallback(NextLevel);
		player.RegisterLevel(currentLevel);
	}

    private void PlayLevelCompletedSound() {
        AudioOut.PlayOneShotAttached(levelCompletedSoundName, player.gameObject);
    }
}
