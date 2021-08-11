using UnityEngine;
using UnityEngine.Audio;

public class WalkingSound : MonoBehaviour
{

    public float maxStepSize = 1.5f;
    public float stopSpeed = 0.1f;
    public string footstepSoundName = "Footsteps";
    AudioOut.Instance soundInstance;
    
    private float _walkingSpeed;
    private float _timeOfLastStep;

    void Start()
    {
        soundInstance = AudioOut.CreateInstance(footstepSoundName);
        _timeOfLastStep = Time.fixedTime;
    }

    void Update()
    {
        if (_walkingSpeed > stopSpeed && !soundInstance.IsPlaying())
        {
            if (_timeOfLastStep + maxStepSize * (1 - _walkingSpeed) <= Time.fixedTime)
            {
                soundInstance.Start();
                _timeOfLastStep = Time.fixedTime;
            }
        }
    }

    public void SetWalking(float walkingSpeed)
    {
        _walkingSpeed = walkingSpeed;
    }
}
