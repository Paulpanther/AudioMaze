using System.Threading.Tasks;
using UnityEngine;

public class WalkAgainsWall : MonoBehaviour
{
	public Player player;

	private Synthesizer _synth;
	private AudioSource _audio;

	private bool _prev;

	private void Start()
	{
		_synth = GetComponent<Synthesizer>();
		_audio = GetComponent<AudioSource>();
		_synth.frequencies = new[] {
			new Synthesizer.Frequency(60, 1f)
		};
		_audio.mute = true;
		_prev = false;
	}

	private async void Update()
	{
		if (_prev != player.walkingAgainstWall)
		{
			_prev = player.walkingAgainstWall;
			await Task.Delay(50);
			_audio.mute = !player.walkingAgainstWall;
		}
	}
}
