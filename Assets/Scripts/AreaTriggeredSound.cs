using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaTriggeredSound : MonoBehaviour
{
    public String areaName;
    public AudioSource audioSource;

    // all available clips for entering the area (will choose 1 of them randomly)
    public AudioClip[] enteringAudioClips;

    // all available clips for leaving the area (will choose 1 of them randomly)
    public AudioClip[] leavingAudioClips;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered area \"" + areaName + "\"");
        if (enteringAudioClips.Length > 0)
        {
            var clipIndex = Random.Range(0, enteringAudioClips.Length);
            audioSource.PlayOneShot(enteringAudioClips[clipIndex]);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Player left area \"" + areaName + "\"");
        if (leavingAudioClips.Length > 0)
        {
            var clipIndex = Random.Range(0, leavingAudioClips.Length);
            audioSource.PlayOneShot(leavingAudioClips[clipIndex]);
        }
    }
}