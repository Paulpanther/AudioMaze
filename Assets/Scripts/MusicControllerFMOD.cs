using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControllerFMOD : MonoBehaviour
{
    // Start is called before the first frame update
    public string music;
    private Rigidbody2D cachedRigidBody;
    public Player player;
    public WallOpeningSound wos;

    public FMOD.Studio.EventInstance musicEV;
    //private int currentCheckpoint = 0;
    //string[] parameters= { "checkpoint1", "checkpoint2", "checkpoint3", "checkpoint4", "checkpoint5", "checkpoint6","emptyParam"};
    //private CallUpdate currentUpdate;
    FMOD.Studio.PLAYBACK_STATE plb;

    public void NewMusicLevel(string musicStr, Player pl)
    {
        Debug.Log("instantiating music");

        if (plb != FMOD.Studio.PLAYBACK_STATE.STOPPED) {
            musicEV.stop(0);
        }
        if (musicStr != null)
        {

            Debug.Log(musicStr, pl);
            music = musicStr;
            player = pl;
            cachedRigidBody = pl.GetComponent<Rigidbody2D>();
            //musicEV.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pl.gameObject, cachedRigidBody));
            musicEV = FMODUnity.RuntimeManager.CreateInstance(musicStr);
            musicEV.start();
            musicEV.getPlaybackState(out plb);
            Debug.Log(plb);
        }
    }
    void Start()
    {
        //Debug.Log(plb);
        //
    }

   

    // Update is called once per frame
    void Update()
    {

        // Debug.Log(musicEV);
        if (player != null)
        {
            
            //Debug.Log(player);
            //Debug.Log(player.GoalOrientation);
            musicEV.setParameterByName("progress", 1-player.distancePercentage);
            musicEV.setParameterByName("orientation", player.GoalOrientation);


        }
        wos.UpdateBools(out var left, out var right);
        musicEV.setParameterByName("OpenLeft", left?1:0);
        musicEV.setParameterByName("OpenRight", right?1:0);
    }

    /*
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
    */
}
