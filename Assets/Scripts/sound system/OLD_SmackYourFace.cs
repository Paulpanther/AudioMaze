using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SmackYourFace : MonoBehaviour
{
    // time in seconds before the scraping sound starts playing
    public float scrapingDelay = 0.020f;

    // time in seconds before you can hit the wall again after leaving it
    public float hitCooldown = 0.100f;

    // speed below the scraping sound stops
    public float scrapingSpeedCutoff = 0.15f;

    public float scrapingMinSpeedPitch = 0.25f;
    public float scrapingMaxSpeedPitch = 1.0f;

    public float minCollisionPointDistance = 0.1f;

    [Range(0, 1)] public float hitVolume = 1.0f;
    [Range(0, 1)] public float scapeVolume = 1.0f;
    
    public AudioMixerGroup audioOut;

    // all available clips for hitting the wall (will choose 1 of them randomly)
    public AudioClip[] hitAudioClips;

    // all available clips for scraping the wall (will choose 1 of them randomly)
    public AudioClip[] scrapeAudioClips;

    private const byte MaximumDirections = 2;
    private bool _audioActive = false;
    private AudioSource[] _audioSource = new AudioSource[MaximumDirections];

    private Player _player;
    private Rigidbody2D _playerBody;

    // relative to player center
    private List<Vector2> _lastHits = new List<Vector2>();
    private float timeOfWallHit = 0;
    private float timeOfLastWallContact = 0;

    private Vector2 _facingDirection;
    private Vector2 _movementDirection;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _playerBody = GetComponent<Rigidbody2D>();
        var audioGroup = new GameObject("Wall Collision Audio Group");
        audioGroup.transform.parent = transform;
        for (byte i = 0; i < MaximumDirections; ++i)
        {
            _audioSource[i] = audioGroup.AddComponent<AudioSource>();
            _audioSource[i].enabled = false;
            _audioSource[i].loop = false;
            _audioSource[i].spatialize = true;
            _audioSource[i].spatialBlend = 1f;
            _audioSource[i].minDistance = 0.5f;
            _audioSource[i].maxDistance = 1f;
            _audioSource[i].outputAudioMixerGroup = audioOut;
        }
    }

    private void FixedUpdate()
    {
        if (_playerBody.velocity.magnitude > 0.01)
        {
            _movementDirection = _playerBody.velocity.normalized;
        }

        _facingDirection = gameObject.transform.up;

        if (_audioActive)
        {
            bool anyActive = false;
            for (int i = 0; i < MaximumDirections; i++)
            {
                if (!_audioSource[i].isPlaying)
                {
                    _audioSource[i].enabled = false;
                }
                else
                {
                    anyActive = true;
                }
            }

            _audioActive = anyActive;
            if (!_audioActive)
            {
                _lastHits.Clear();
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Vector2 pos2d = transform.position;
        Vector2 tmp1 = Vector2.one / 5;
        Vector2 tmp2 = new Vector2(-1, 1) / 5;
        for (byte i = 0; i < _lastHits.Count; i++)
        {
            Debug.DrawLine(pos2d + _lastHits[i] + tmp1, pos2d + _lastHits[i] - tmp1, Color.yellow);
            Debug.DrawLine(pos2d + _lastHits[i] + tmp2, pos2d + _lastHits[i] - tmp2, Color.yellow);
        }
        
        Debug.DrawRay(transform.position, _facingDirection, Color.red);
        Debug.DrawRay(transform.position, _movementDirection, Color.blue);
    }

    private void UpdateAudio(
        List<Vector2> offsets,
        AudioClip[] clips,
        float volume,
        bool speedChangesPitch = false
    )
    {
        for (byte i = 0; i < offsets.Count; i++)
        {
            _audioSource[i].transform.position = (Vector2) transform.position + offsets[i];
            _audioSource[i].enabled = true;
            _audioSource[i].volume = volume;
            if (!_audioSource[i].isPlaying)
            {
                var clipIndex = Random.Range(0, clips.Length);
                _audioSource[i].PlayOneShot(clips[clipIndex]);
            }
            _audioActive = true;

            Vector2 normOffset = offsets[i].normalized;
            Vector2 vel = _playerBody.velocity;
            float relativeSpeed = Math.Abs(normOffset.x * vel.y + normOffset.y * vel.x);
            if (speedChangesPitch)
            {
                if (relativeSpeed <= scrapingSpeedCutoff)
                {
                    _audioSource[i].Stop();
                }
                else
                {
                    float speedValue = (relativeSpeed - scrapingSpeedCutoff) / (_player.maxSpeed - scrapingSpeedCutoff);
                    _audioSource[i].pitch = scrapingMinSpeedPitch +
                                            speedValue * (scrapingMaxSpeedPitch - scrapingMinSpeedPitch);
                }
            }
            else
            {
                _audioSource[i].pitch = 1;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Time.fixedTime <= timeOfLastWallContact + hitCooldown)
            return;

        timeOfWallHit = Time.fixedTime;
        if (hitAudioClips.Length == 0)
            return;

        var directions = (byte) Math.Min(MaximumDirections, other.contactCount);
        _lastHits.Clear();
        for (byte i = 0; i < directions; i++)
        {
            Vector2 hitLocation = other.GetContact(i).point - (Vector2) transform.position;
            if (i == 0 || (_lastHits[0] - hitLocation).magnitude > minCollisionPointDistance)
            {
                _lastHits.Add(hitLocation);
            }
        }

        UpdateAudio(_lastHits, hitAudioClips, hitVolume);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (scrapeAudioClips.Length == 0)
            return;
        if (Time.fixedTime <= timeOfWallHit + scrapingDelay)
            return;

        var directions = (byte) Math.Min(MaximumDirections, other.contactCount);
        List<Vector2> collisions = new List<Vector2>(directions);
        for (byte i = 0; i < directions; i++)
        {
            Vector2 hitLocation = other.GetContact(i).point - (Vector2) transform.position;
            if (i == 0 || (collisions[0] - hitLocation).magnitude > minCollisionPointDistance)
            {
                collisions.Add(hitLocation);
            }
        }

        UpdateAudio(collisions, scrapeAudioClips, scapeVolume, true);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (Time.fixedTime >= timeOfWallHit + scrapingDelay)
        {
            // stop scraping sound
            for (int i = 0; i < MaximumDirections; i++)
            {
                _audioSource[i].Stop();
                _audioSource[i].enabled = false;
            }
        }

        timeOfLastWallContact = Time.fixedTime;
    }
}