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

public class WallOpeningDetector : MonoBehaviour
{
	private Player player;

	private Tilemap map => player.map;
	private Side[] sides;

	private void Start()
	{
		player = gameObject.GetComponent<Player>();
		sides = new[] {
			new Side(() => -player.transform.right + player.transform.position, "OpenLeft"),
			new Side(() => player.transform.right + player.transform.position, "OpenRight"),
			//new Side(() => player.transform.up + player.transform.position, "HoleFront")
		};
	}

	public void GetSideStatus(out bool left, out bool right)
	{
		left = IsHoly(sides[0]);
		right = IsHoly(sides[1]);
	}

	private bool IsHoly(Side side)
    {
		var hasHole = HasHole(side.position());
		return hasHole;
	}
	private bool HasHole(Vector3 worldPos)
	{
		return !map.HasTile(map.WorldToCell(worldPos));
	}
}
