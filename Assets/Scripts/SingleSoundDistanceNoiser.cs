using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Synthesizer;

public class SingleSoundDistanceNoiser : MonoBehaviour
{

    private Player _player;
    private Synthesizer _synthesizer;

    private Frequency[] _frequencies = {new Frequency(100, 0)};
    
    private void Start()
    {
        _player = GetComponentInParent<Player>();
        _synthesizer = GetComponent<Synthesizer>();
        _synthesizer.frequencies = _frequencies;
    }

    private void Update()
    {
        _frequencies[0].amplitude = _player.distanceChange > 0 ? 1 : 0;
    }
}
