﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControllerFMOD : MonoBehaviour
{
    // Start is called before the first frame update
    public string music = "event:/MainMusic";
    public Rigidbody2D cachedRigidBody;
    public Level level;

    [NonSerialized] private CallUpdate[] sources;

    FMOD.Studio.EventInstance musicEV;
    private int currentCheckpoint = 0;
    string[] parameters= { "checkpoint1", "checkpoint2", "checkpoint3", "checkpoint4", "checkpoint5", "checkpoint6","emptyParam"};
    private CallUpdate currentUpdate;
    FMOD.Studio.PLAYBACK_STATE plb;

    void Start()
    {
        AssignChildren();
        musicEV = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEV.getPlaybackState(out plb);
        //Debug.Log(plb);
        musicEV.start();
        musicEV.getPlaybackState(out plb);
        //Debug.Log(plb);
        currentUpdate = sources[0];
        musicEV.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(sources[currentCheckpoint+1].gameObject, cachedRigidBody));
    }

    private void AssignChildren()
    {
        var children = GetComponentsInChildren<CallUpdate>();
        sources = children;
        CallUpdate lastChild = null;
        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            child.level = level;
            child.prevCheckpoint = lastChild;
            child.CheckPointNumber = i;
            lastChild = child;
        }
    }

    // Update is called once per frame
    void Update()
    {
        musicEV.getPlaybackState(out plb);
        //Debug.Log(plb);
        float updateVal = currentUpdate.GetVolume();
        //Debug.Log("updateVal " + updateVal+" "+currentCheckpoint);
        musicEV.setParameterByName(parameters[currentCheckpoint], updateVal);

    }
    public void CheckpointReached(int checkNum)
    {
        if (checkNum > currentCheckpoint)
        {
            //Remove the sound effects effecting current checkpoint audio
            for (int i = 0; i < checkNum && i < sources.Length; i++)
            {
                musicEV.setParameterByName(parameters[i], 1f);
                
            }
            currentCheckpoint = checkNum;
            if (checkNum < sources.Length)
            {
                musicEV.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(sources[currentCheckpoint + 1].gameObject, cachedRigidBody));
            }


            if (currentCheckpoint < sources.Length)
            {
                Debug.Log("Starting to play next CP audio");
                currentUpdate = sources[currentCheckpoint];
            }

        }
    }
}
