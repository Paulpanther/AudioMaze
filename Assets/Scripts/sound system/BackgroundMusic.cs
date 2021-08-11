﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public float smoothOpenSpeed, smoothOrientationK;
    private float smoothOpenLeft, smoothOpenRight, smoothOrientation;

    private string musicEventName;
    // private Rigidbody2D cachedRigidBody;
    private Player player;
    private WallOpeningDetector wallOpeningDetector;

    private AudioOut.Instance musicInstance;

    public void StopBackgroundMusic() {
        musicInstance.Stop();
    }

    public void UpdateBackgroundMusic(string musicEventName, Player player)
    {
        this.musicEventName = musicEventName;
        this.player = player;
        wallOpeningDetector = player.GetComponent<WallOpeningDetector>();
        // cachedRigidBody = player.GetComponent<Rigidbody2D>();
        // musicInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pl.gameObject, cachedRigidBody));
        musicInstance = AudioOut.StartInstance(musicEventName);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) UpdateBackgroundMusic();
    }

    private void UpdateBackgroundMusic() {
        musicInstance.fmodInstance.setParameterByName("progress", 1-player.distancePercentage);
        // var goalOrientation = player.GoalOrientation;
        // var factor = goalOrientation < smoothOrientation ? -1 : 1;
        // smoothOrientation += factor * smoothOrientationK * Time.deltaTime;
        // if (factor > 0) smoothOrientation = Mathf.Min(smoothOrientation, goalOrientation);
        // if (factor < 0) smoothOrientation = Mathf.Max(smoothOrientation, goalOrientation);
        smoothOrientation = smoothOrientation * smoothOrientationK +
                            player.GoalOrientation * (1 - smoothOrientationK);
        musicInstance.fmodInstance.setParameterByName("orientation", smoothOrientation);

        wallOpeningDetector.GetSideStatus(out var left, out var right);

        smoothOpenLeft = Mathf.Clamp01(smoothOpenLeft + (left ? 1 : -1) * smoothOpenSpeed * Time.deltaTime);
        smoothOpenRight = Mathf.Clamp01(smoothOpenRight + (right ? 1 : -1) * smoothOpenSpeed * Time.deltaTime);
        musicInstance.fmodInstance.setParameterByName("OpenLeft", smoothOpenLeft);
        musicInstance.fmodInstance.setParameterByName("OpenRight", smoothOpenRight);
    }
}