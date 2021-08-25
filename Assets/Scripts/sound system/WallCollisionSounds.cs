using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class WallCollisionSounds : MonoBehaviour
{

    public string wallHitSoundName = "WallSounds";
    public string wallScratchSoundName = "WallScratch";
    AudioOut.Instance wallScratchInstance;

    public float scrapingDelay = 0.020f;

    // time in seconds before you can hit the wall again after leaving it
    public float hitCooldown = 0.100f;
    private Player _player;
    private Rigidbody2D _playerBody;
    private float timeOfLastWallContact = 0;
    private float timeOfWallHit = 0;


    public float scrapingSpeedCutoff = 0.15f;

    public float scrapingMinSpeedPitch = 0.25f;
    public float scrapingMaxSpeedPitch = 1.0f;

    private Vector2 _facingDirection;
    private Vector2 _movementDirection;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<Player>();
        _playerBody = GetComponent<Rigidbody2D>();

        wallScratchInstance = AudioOut.CreateInstance(wallScratchSoundName);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = _playerBody.velocity;
        float speed = vel.magnitude;
        speed = speed / 1.5f;
        wallScratchInstance.fmodInstance.setParameterByName("ScratchPitch", speed);

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EventLogging.logEvent(new CollisionEvent(AbstractEvent.Action.Started));
        if (Time.fixedTime <= timeOfLastWallContact + hitCooldown)
            return;

        timeOfWallHit = Time.fixedTime;
        AudioOut.PlayOneShotAttached(wallHitSoundName, gameObject);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (Time.fixedTime <= timeOfWallHit + scrapingDelay)
            return;
        if (!wallScratchInstance.IsPlaying()) {
            wallScratchInstance.Start();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        EventLogging.logEvent(new CollisionEvent(AbstractEvent.Action.Stopped));
        wallScratchInstance.Stop();
        timeOfLastWallContact = Time.fixedTime;
    }
}
