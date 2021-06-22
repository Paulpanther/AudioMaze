using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class WallSoundsFMOD : MonoBehaviour
{

    public string wallhits = "event:/WallSounds";
    public string wallscratch = "event:/WallScratch";
    FMOD.Studio.EventInstance wallScratchEV;

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
        _player = GameObject.Find("Player").GetComponent<Player>();
        _playerBody = GetComponent<Rigidbody2D>();

        wallScratchEV = FMODUnity.RuntimeManager.CreateInstance(wallscratch);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = _playerBody.velocity;
        float speed = vel.magnitude;
        speed = speed / 1.5f;
        wallScratchEV.setParameterByName("ScratchPitch", speed);

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Time.fixedTime <= timeOfLastWallContact + hitCooldown)
            return;

        timeOfWallHit = Time.fixedTime;
        FMODUnity.RuntimeManager.PlayOneShot(wallhits, gameObject.transform.position);
        

    }

    
    
    private void OnCollisionStay2D(Collision2D other)
    {
        
        if (Time.fixedTime <= timeOfWallHit + scrapingDelay)
            return;
        if (IsPlaying(wallScratchEV))
        {


        }
        else
        {
            Debug.Log("scratching!");
            wallScratchEV.start();
        }
      
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        wallScratchEV.stop(0);

        timeOfLastWallContact = Time.fixedTime;
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
