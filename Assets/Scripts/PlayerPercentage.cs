using TMPro;
using UnityEngine;

public class PlayerPercentage : MonoBehaviour
{
	public TextMeshProUGUI percentageText;
	public Player player;
	public bool visible = false;

	public void Update()
	{
		if (Input.GetKeyDown("p"))
		{
			visible = !visible;
		}
		
		if (visible)
		{
			percentageText.enabled = true;
			percentageText.text = (player.distancePercentage * 100).ToString("0.");
		}
		else
		{
			percentageText.enabled = false;
		}
	}
}
