﻿using UnityEngine;
using UnityEngine.Audio;

public class WalkingSound : MonoBehaviour
{
    // sound volume of walking sound
    public float soundVolume = 0.5f;
    
    // all available clips for walking (will choose 1 of them randomly)
    public AudioClip[] audioClips;
    public AudioMixerGroup audioOut;
    private bool _isWalking = true;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.spatialize = false;
        _audioSource.reverbZoneMix = 1.0f;
        _audioSource.volume = soundVolume;
        _audioSource.outputAudioMixerGroup = audioOut;
    }

    void Update()
    {
        if (_isWalking && !_audioSource.isPlaying)
        {
            var clipIndex = Random.Range(0, audioClips.Length);
            // Debug.Log("Play walking clip #" + clipIndex);
            _audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }

    public void SetWalking(bool walking)
    {
        _isWalking = walking;
    }
}