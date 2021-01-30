using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SmackYourFace : MonoBehaviour
{
    public AudioMixerGroup audioOut;

    // all available clips for hitting the wall (will choose 1 of them randomly)
    public AudioClip[] audioClips;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.spatialize = true;
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 0.5f;
        _audioSource.maxDistance = 1f;
        _audioSource.outputAudioMixerGroup = audioOut;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // TODO: fix broken collision
        Debug.Log("Ouch, I hit my nose.");
        Vector3 location = other.GetContact(0).point;
        if (audioClips.Length > 0)
        {
            var clipIndex = Random.Range(0, audioClips.Length);
            // _audioSource.transform.position = location;
            _audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }
}