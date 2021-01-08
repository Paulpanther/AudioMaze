using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    private MusicController mus;
    public int CheckPointNumber;
    void Start()
    {
        mus = GetComponentInParent(typeof(MusicController)) as MusicController;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Reached Checkpoint " + CheckPointNumber);
            mus.CheckpointReached(CheckPointNumber);
        }
    }

}
