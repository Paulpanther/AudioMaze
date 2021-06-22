using UnityEngine;
using UnityEngine.Audio;

public class WalkingSound : MonoBehaviour
{

    public float maxStepSize = 1.5f;
    public float stopSpeed = 0.1f;
    public string footsteps = "event:/Footsteps";
    FMOD.Studio.EventInstance footEV;

    // all available clips for walking (will choose 1 of them randomly)
    // public AudioClip[] audioClips;

    private float _walkingSpeed;

    //private AudioSource _audioSource;

    private float _timeOfLastStep;

    void Start()
    {

        //_audioSource = gameObject.AddComponent<AudioSource>();
        //_audioSource.loop = false;
        //_audioSource.spatialize = true;
        //_audioSource.spatialBlend = 1f;
        //_audioSource.transform.Translate(0, -0.5f, 0);
        //_audioSource.minDistance = 0.5f;
        //_audioSource.maxDistance = 1f;
        //_audioSource.outputAudioMixerGroup = audioOut;
        footEV = FMODUnity.RuntimeManager.CreateInstance(footsteps);
        _timeOfLastStep = Time.fixedTime;
    }

    void Update()
    {
        if (_walkingSpeed > stopSpeed && !IsPlaying(footEV))
        {
            if (_timeOfLastStep + maxStepSize * (1 - _walkingSpeed) <= Time.fixedTime)
            {
                //var clipIndex = Random.Range(0, audioClips.Length);
                // Debug.Log("Play walking clip #" + clipIndex);
                footEV.start();
                //FMODUnity.RuntimeManager.PlayOneShot(footsteps,GetComponent<Transform>().position);
                _timeOfLastStep = Time.fixedTime;
            }
        }
    }

    public void SetWalking(float walkingSpeed)
    {
        _walkingSpeed = walkingSpeed;
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}