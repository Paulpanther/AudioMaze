using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControllerFMOD : MonoBehaviour
{
    // Start is called before the first frame update
    public string music = "event:/MainMusic";
    FMOD.Studio.EventInstance musicEV;
    private int currentCheckpoint = 0;
    string[] parameters= { "checkpoint1", "checkpoint2", "checkpoint3", "checkpoint4", "checkpoint5", "checkpoint6","emptyParam"};
    public CallUpdate[] sources;
    private CallUpdate currentUpdate;
    FMOD.Studio.PLAYBACK_STATE plb;
    public Rigidbody2D cachedRigidBody;

    void Start()
    {
        musicEV = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEV.getPlaybackState(out plb);
        //Debug.Log(plb);
        musicEV.start();
        musicEV.getPlaybackState(out plb);
        //Debug.Log(plb);
        currentUpdate = sources[0];
        musicEV.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(sources[currentCheckpoint+1].gameObject, cachedRigidBody));
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
