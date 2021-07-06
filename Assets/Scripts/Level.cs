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

	public string music;

	//[NonSerialized] 
	public Player player;

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
