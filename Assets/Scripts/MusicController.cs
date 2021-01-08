using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource[] sources;

    private int currentCheckpoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].Play();
            sources[i].loop = true;
            sources[i].volume = 0;

        }
        //Only for now
        sources[currentCheckpoint].volume = 1;
        
    }

    private void Update()
    {
        //Call update based on currentCheckpoint to make current CheckpointMusic Louder
    }

    // Update is called once per frame
    public void CheckpointReached(int checkNum)
    {
        if (checkNum > currentCheckpoint)
        {
            for (int i = currentCheckpoint;i<checkNum && i < sources.Length; i++)
            {
                sources[i].volume = 1;
                RemoveFx(sources[i].gameObject);
            }
            currentCheckpoint = checkNum;

            if (currentCheckpoint < sources.Length)
            {
                Debug.Log("Starting to play next CP audio");
                sources[currentCheckpoint].volume = 1;
                //This is just for this version, later the volume will slide in.

            }
            
        }
    }

    public void RemoveFx(GameObject gObj)
    {
        Debug.Log("Removing Effects of gameObject");
    }

}
