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
	public MusicControllerFMOD fmodmusic;
	private Level[] levels;
	private int currentLevelIndex = -1;
	private Level currentLevel = null;
	public string LevelEvent = "";

	private void Start()
	{
        var levelString = "1aba";
        var firstScenarioIs1 = levelString[0] == '1';
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
			FMODUnity.RuntimeManager.PlayOneShot(LevelEvent, transform.position);
		}
		currentLevel = Instantiate(levels[++currentLevelIndex]);
		Debug.Log("calling music inst");
		fmodmusic.NewMusicLevel(currentLevel.music, player);
		currentLevel.win.RegisterWinCallback(NextLevel);
		player.RegisterLevel(currentLevel);
        EventLogging.logEvent(new LevelEvent(currentLevel.name));
		
	}
}
