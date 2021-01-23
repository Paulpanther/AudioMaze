using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAudioClip : MonoBehaviour
{
    // all available clips (will choose 1 of them randomly)
    public AudioClip[] idlingAudioClips;

    public float idleMinDelay;
    public float idleMaxDelay;

    private float _timeOfNextIdleSound = 0;

    private AudioSource _audioSource;
    private GameObject _player;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player");

        RequestNextIdleSound();
    }

    /// <summary>
    /// calculates the start time of the next idle sound
    /// </summary>
    private void RequestNextIdleSound()
    {
        float delay = 0;
        if (_audioSource.clip != null)
        {
            // remaining time of current clip
            delay += _audioSource.clip.length - _audioSource.time;
        }

        delay += Random.Range(idleMinDelay, idleMaxDelay);

        _timeOfNextIdleSound = Time.fixedTime + delay;
    }

    private void Update()
    {
        if (idlingAudioClips.Length > 0)
        {
            var dist = _DistanceToPlayer();
            if (_DistanceToPlayer() <= _audioSource.maxDistance)
            {
                if (!_audioSource.enabled)
                {
                    _audioSource.enabled = true;
                }

                if (!_audioSource.isPlaying && Time.fixedTime > _timeOfNextIdleSound)
                {
                    var clipIndex = Random.Range(0, idlingAudioClips.Length);
                    _audioSource.PlayOneShot(idlingAudioClips[clipIndex]);
                    Debug.Log("Player listens to \"" + name + "\" (Distance: " + dist + ", Clip: " + clipIndex + ")");
                    RequestNextIdleSound();
                }
            }
            else if (_audioSource.enabled)
            {
                _audioSource.enabled = false;
            }
        }
    }

    private float _DistanceToPlayer()
    {
        Vector2 audioPos = _audioSource.transform.position;
        Vector2 playerPos = _player.transform.position;
        var vec = audioPos - playerPos;
        return vec.magnitude;
    }
}