using UnityEngine;
using UnityEngine.Audio;

public class WalkingSound : MonoBehaviour
{
    public AudioMixerGroup audioOut;
    
    // all available clips for walking (will choose 1 of them randomly)
    public AudioClip[] audioClips;
    
    private bool _isWalking = true;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.spatialize = true;
        _audioSource.spatialBlend = 1f;
        _audioSource.transform.Translate(0, -0.5f, 0);
        _audioSource.minDistance = 0.5f;
        _audioSource.maxDistance = 1f;
        _audioSource.outputAudioMixerGroup = audioOut;
    }

    void Update()
    {
        if (_isWalking && !_audioSource.isPlaying)
        {
            var clipIndex = Random.Range(0, audioClips.Length);
            // Debug.Log("Play walking clip #" + clipIndex);
            _audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }

    public void SetWalking(bool walking)
    {
        _isWalking = walking;
    }
}