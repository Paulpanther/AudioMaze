using System;
using UnityEngine;
using Random = System.Random;

public class WalkingSound : MonoBehaviour
{
    // sound volume of walking sound
    public float soundVolume = 0.5f;
    
    // all available clips for walking (will choose 1 of them randomly)
    public AudioClip[] audioClips;
    
    private bool _isWalking = true;

    private AudioSource _audioSource;

    private readonly Random _random = new Random();

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.spatialize = false;
        _audioSource.reverbZoneMix = 1.0f;
        _audioSource.volume = soundVolume;
    }

    void Update()
    {
        if (_isWalking && !_audioSource.isPlaying)
        {
            var clipIndex = _random.Next(0, audioClips.Length);
            // Debug.Log("Play walking clip #" + clipIndex);
            _audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }

    public void SetWalking(bool walking)
    {
        _isWalking = walking;
    }
}