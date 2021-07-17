using System;
using UnityEngine;
using UnityEngine.Tilemaps;

internal class Side
{
	public bool lastValue = false;
	public readonly Func<Vector3> position;
	public readonly string key;

	public Side(Func<Vector3> position, string key)
	{
		this.position = position;
		this.key = key;
	}
}

public class WallOpeningSound : MonoBehaviour
{
	public Player player;

	private Tilemap map => player.map;
	private FMODUnity.StudioEventEmitter emitter;
	private Side[] sides;

	private void Start()
	{
		emitter = GetComponent<FMODUnity.StudioEventEmitter>();
		sides = new[] {
			new Side(() => -player.transform.right + player.transform.position, "OpenLeft"),
			new Side(() => player.transform.right + player.transform.position, "OpenRight"),
			//new Side(() => player.transform.up + player.transform.position, "HoleFront")
		};
	}

	public void UpdateBools(out bool left, out bool right)
	{
		left = Holy(sides[0]);
		right = Holy(sides[1]);
		// Debug.Log(map.WorldToCell(sides[0].position()) + " " + map.WorldToCell(sides[1].position())+ " " + map.WorldToCell(sides[2].position()));
	}

	private bool Holy(Side side)
    {
		var hasHole = HasHole(side.position());
		return hasHole;
	}
	private bool HasHole(Vector3 worldPos)
	{
		return !map.HasTile(map.WorldToCell(worldPos));
	}
}
