﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource[] sources;
    private int currentCheckpoint = 0;
    private CallUpdate currentUpdate;
    private GameObject currentObject;
    private AudioDistortionFilter currentDist;
    private AudioEchoFilter currentEch;
    private AudioLowPassFilter currentLP;
    private AudioHighPassFilter currentHP;
    private int goodBassCutoff = 5000;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].Play();
            sources[i].loop = true;
            sources[i].volume = 0;

        }
        currentUpdate = sources[currentCheckpoint].gameObject.GetComponent<CallUpdate>();
        currentObject = sources[currentCheckpoint].gameObject;
        AddFX(currentObject);

    }

    private void Update()
    {
        //Call update based on currentCheckpoint to make current CheckpointMusic Louder 

        float updateVal = currentUpdate.GetVolume();
        // Debug.Log(updateVal);
        sources[currentCheckpoint].volume = updateVal;
        currentDist.distortionLevel = 1 - updateVal;
        currentEch.dryMix = updateVal;
        currentEch.wetMix = (1 - updateVal);
        currentLP.cutoffFrequency = 18000*updateVal;
        currentHP.cutoffFrequency = goodBassCutoff * (1-updateVal);
    }

    // Update is called once per frame
    public void CheckpointReached(int checkNum,int goodCutOff)
    {
        if (checkNum > currentCheckpoint)
        {
            //Remove the sound effects effecting current checkpoint audio
            for (int i = 0;i<checkNum && i < sources.Length; i++)
            {
                sources[i].volume = 1;
                if (i < checkNum - 1)
                {
                    sources[i].gameObject.transform.position = sources[checkNum].gameObject.transform.position;
                }
            }
            
            RemoveFx(sources[currentCheckpoint].gameObject);

            currentCheckpoint = checkNum;
            goodBassCutoff = goodCutOff;

            if (currentCheckpoint < sources.Length)
            {
                Debug.Log("Starting to play next CP audio");
                AddFX(sources[currentCheckpoint].gameObject);
                currentUpdate = sources[currentCheckpoint].gameObject.GetComponent<CallUpdate>();
                
                
            }
            
        }
    }

    public void AddFX(GameObject gObj) {
        currentDist = gObj.AddComponent<AudioDistortionFilter>();
        currentEch = gObj.AddComponent<AudioEchoFilter>();
        currentLP = gObj.AddComponent<AudioLowPassFilter>();
        currentHP = gObj.AddComponent<AudioHighPassFilter>();
        currentEch.delay = 200;
        currentEch.decayRatio = 0.2f;





    }
    public void RemoveFx(GameObject gObj)
    {
;       Destroy(sources[currentCheckpoint].GetComponent<AudioDistortionFilter>());
        Destroy(sources[currentCheckpoint].GetComponent<AudioEchoFilter>());
        Destroy(sources[currentCheckpoint].GetComponent<AudioLowPassFilter>());
        Destroy(sources[currentCheckpoint].GetComponent<AudioHighPassFilter>());
    }

}
