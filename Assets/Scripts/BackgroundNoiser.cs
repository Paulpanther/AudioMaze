using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Synthesizer;
using Random = UnityEngine.Random;

public class BackgroundNoiser : MonoBehaviour
{
    public Synthesizer synth;
    public float changeRate = 0.1f;
    public int changeTime = 2000;
    public float lowFreqBound = 70;
    public float highFreqBound = 200;

    private readonly Frequency[] _frequencies = {
        new Frequency(97.9989f, 1),
        new Frequency(123.471f, 1),
        new Frequency(146.832f, 1),
        new Frequency(195.998f, 1)
    };

    private float[] _nextGoal;
    
    private async void Start()
    {
        synth.frequencies = _frequencies;
        _nextGoal = _frequencies.Select(f => f.frequency).ToArray();
        synth.AddPreReadAction(OnAudioUpdate);
        
        while (true)
        {
            await Task.Delay(changeTime);
            var rIndex = Random.Range(0, _frequencies.Length);
            _nextGoal[rIndex] = Random.Range(lowFreqBound, highFreqBound);
        }
    }

    private void OnAudioUpdate()
    {
        for (var i = 0; i < _frequencies.Length; i++)
        {
            _frequencies[i].frequency += (_nextGoal[i] - _frequencies[i].frequency) * changeRate;
        }
    }
}
