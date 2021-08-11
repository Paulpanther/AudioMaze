using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOut
{
    public static Instance CreateInstance(string soundEventName)
    {
        return new Instance(soundEventName, false);
    }
    public static Instance StartInstance(string soundEventName)
    {
        return new Instance(soundEventName, true);
    }
    public static void PlayOneShot(string soundEventName, Vector3 position)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/" + soundEventName, position);
        EventLogging.logEvent(new SoundEvent.Oneshot(soundEventName));
    }
    public static void PlayOneShotAttached(string soundEventName, GameObject gameObject)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/" + soundEventName, gameObject);
        EventLogging.logEvent(new SoundEvent.Oneshot(soundEventName));
    }

    public class Instance
    {
        public string soundEventName;
        public FMOD.Studio.EventInstance fmodInstance;

        public Instance(string soundEventName, bool started = true)
        {
            this.soundEventName = soundEventName;
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance("event:/" + soundEventName);
            if(started) Start();
        }

        public bool Start()
        {
            var wasNotPlaying = fmodState() != FMOD.Studio.PLAYBACK_STATE.PLAYING;
            fmodInstance.start();
            if(wasNotPlaying) {
                EventLogging.logEvent(new SoundEvent.Instance(soundEventName, AbstractEvent.Action.Started));
            }
            return wasNotPlaying;
        }
        public bool Stop()
        {
            var wasNotStopped = fmodState() != FMOD.Studio.PLAYBACK_STATE.STOPPED;
            fmodInstance.stop(0);
            if(wasNotStopped) {
                EventLogging.logEvent(new SoundEvent.Instance(soundEventName, AbstractEvent.Action.Stopped));
            }
            return wasNotStopped;
        }

        public bool IsPlaying()
        {
            return fmodState() == FMOD.Studio.PLAYBACK_STATE.PLAYING;
        }

        public FMOD.Studio.PLAYBACK_STATE fmodState()
        {
            FMOD.Studio.PLAYBACK_STATE fmodState;
            fmodInstance.getPlaybackState(out fmodState);
            return fmodState;
        }
    }
}
