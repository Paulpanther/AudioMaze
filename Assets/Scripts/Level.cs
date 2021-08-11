using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
	public MazeSolver mazeSolver => GetComponentInChildren<MazeSolver>();
	public Tilemap map => GetComponentInChildren<Tilemap>();
	public Win win => GetComponentInChildren<Win>();

	public string musicName = "MainMusic";
	public bool isVisible = false;

	//[NonSerialized] 
	public Player player;

	public void Start() {
		if (isVisible) {
			player.visualization.visualization = VisualizationToggle.VisualizationMode.Everything;
		}
	}

	public void Destroy()
	{
		player.visualization.visualization = VisualizationToggle.VisualizationMode.Nothing;
		Destroy(gameObject);
	}
}
