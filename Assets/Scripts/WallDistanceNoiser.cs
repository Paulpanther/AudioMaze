﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/*To make this work you have to have the player object be unaffected by raycasts, put the player object 
 * in the 'ignore raycasts' layer and make sure the objects you want to detect are tagged wall
 * 
 * Then: Apply this script to the player object
 * Create an audio source
 * Fine tune the values for triggerDistance and soundVolume
 * */

public class WallDistanceNoiser : MonoBehaviour
{
    public AudioMixerGroup audioOut;

    //Resolution of Ray casting
    public int numRays = 12;

    //distance where sound begins to play
    public float triggerDistance = 10;
    //Way how wall distance is mapped to sound volume
    public DistanceMapping distanceMapping = DistanceMapping.INVERSE_QUADRATIC;

   //Audio Clip for Wall Noise
    public AudioClip wallSound;
    //Audio Sources for directions of wall noise
    private AudioSource[] _wallNoise;

    private float _playerRadius = 0.2f; //TODO make dynamic

    private int _wallLayer;
    
    private void Start()
    {
        _wallLayer = LayerMask.NameToLayer("Walls");
        
        var audioGroup = new GameObject("Wall Distance Audio Group");
        audioGroup.transform.parent = transform;
        _wallNoise = new AudioSource[numRays];
        for(int i = 0; i < numRays; i++) {
            Vector2 direction = Direction(i);
            AudioSource audioSource = audioGroup.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioOut;
            audioSource.clip = wallSound;
            audioSource.pitch = 1.0f + direction.y * 0.5f;
            audioSource.panStereo = direction.x;
            _wallNoise[i] = audioSource;
        }
    }

    private void UpdateSound(int rayIndex, float distance)
    {
        AudioSource noiseOfDirection = _wallNoise[rayIndex];
        if (distance <= triggerDistance)
        {
            noiseOfDirection.volume = MapDistance(distance);
            if(!noiseOfDirection.isPlaying) noiseOfDirection.Play();
        } 
        else 
        {
            if(noiseOfDirection.isPlaying) noiseOfDirection.Stop();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        for(int i = 0; i < numRays; i++)
        {
            Vector2 direction = Direction(i);
            Vector2 rayStart = pos + direction * _playerRadius;
            RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, 100, 1 << _wallLayer);
            Debug.DrawRay(rayStart, direction * triggerDistance, Color.red);
            UpdateSound(i, hit.distance);
            
            if (hit.collider && hit.distance <= triggerDistance)
            {
                Debug.DrawRay(rayStart, hit.point - rayStart, Color.green);
            }
        }
    }

    Vector2 Direction(int rayIndex) 
    {
        float currentAngle = -transform.rotation.eulerAngles.z;

        float angle = currentAngle + 360f / numRays * (rayIndex + 0.5f);
        float radians = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        return direction;
    }

    public enum DistanceMapping 
    {
        LINEAR_DECREASE, INVERSE_LINEAR, INVERSE_QUADRATIC
    }

    private float MapDistance(float distance) 
    {
        switch(distanceMapping) 
        {
            case DistanceMapping.LINEAR_DECREASE : 
                return (1f - distance/triggerDistance);
            
            case DistanceMapping.INVERSE_LINEAR : 
                return 1f / (distance + 1f);
            
            case DistanceMapping.INVERSE_QUADRATIC : default :
                return 1f / (distance * distance + 1f);
        }
    }
}
