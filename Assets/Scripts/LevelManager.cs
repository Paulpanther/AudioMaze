
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public List<Level> levels;
	public Transform player;

	private int currentLevel = -1;
	private Level level = null;

	private void Start()
	{
		NextLevel();
	}

	private void NextLevel()
	{
		if (currentLevel + 1 >= levels.Count)
		{
			SceneManager.LoadScene("MenuScene");
			return;
		}

		if (level != null) level.Destroy();
		level = Instantiate(levels[++currentLevel]);
		level.RegisterWinCallback(NextLevel);
		player.position = Vector3.zero;
	}
}
