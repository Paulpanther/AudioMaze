using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AreaTriggeredSound : MonoBehaviour
{
    public AudioMixerGroup audioOut;

    public String areaName;

    public Vector3 relativeAudioSourceOffset = new Vector3(0, 1, 0);

    // all available clips for entering the area (will choose 1 of them randomly)
    public AudioClip[] enteringAudioClips;

    // all available clips for idling in the area (will choose 1 of them randomly)
    public AudioClip[] idlingAudioClips;

    public float idleMinDelay;
    public float idleMaxDelay;

    // all available clips for leaving the area (will choose 1 of them randomly)
    public AudioClip[] leavingAudioClips;

    public Sprite crosshairSprite;

    private AudioSource _audioSource;
    // private Collider2D _collider;

    // private Player _player;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.outputAudioMixerGroup = audioOut;
        // _audioSource.spatialize = false;
        // _audioSource.reverbZoneMix = 1.0f;

        var x = new GameObject("Audio Offset Marker");
        x.transform.parent = transform;
        var audioSourceOffsetSprite = x.AddComponent<SpriteRenderer>();
        audioSourceOffsetSprite.sprite = crosshairSprite;
        audioSourceOffsetSprite.transform.localPosition = relativeAudioSourceOffset;

        // _collider = gameObject.GetComponent<Collider2D>();
        // _player = FindObjectOfType<Player>();
    }


    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + 2 * relativeAudioSourceOffset, Color.red);
    }

    private float _timeOfNextIdleSound = 0;

    /// <summary>
    /// calculates the start time of the next idle sound
    /// which results in playing the sound if the player still is in the area at this time
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered area \"" + areaName + "\"");
        if(enteringAudioClips.Length > 0)
        {
            var clipIndex = Random.Range(0, enteringAudioClips.Length);
            _audioSource.PlayOneShot(enteringAudioClips[clipIndex]);
        }
        RequestNextIdleSound();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!_audioSource.isPlaying && Time.fixedTime > _timeOfNextIdleSound)
        {
            if (idlingAudioClips.Length > 0)
            {
                var clipIndex = Random.Range(0, idlingAudioClips.Length);
                _audioSource.PlayOneShot(idlingAudioClips[clipIndex]);
            }

            Debug.Log("Player listens to area \"" + areaName + "\"");
            RequestNextIdleSound();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player left area \"" + areaName + "\"");
        if (leavingAudioClips.Length > 0)
        {
            var clipIndex = Random.Range(0, leavingAudioClips.Length);
            _audioSource.PlayOneShot(leavingAudioClips[clipIndex]);
        }
    }
}