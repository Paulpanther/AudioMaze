using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSoundGenerator : MonoBehaviour
{
    public int sampleRate = 44100;

    /**
     * Used by the Synthesizer
     * Do not use for volume control - add a AudioMixerGroup to your AudioSource instead!
     */
    [Range(0, 1)] public float baseAmplitude = 1.0f;
    
    public int channels = 1;

    private List<Action> _preReadActions = new List<Action>();

    public void Start()
    {
        AudioClip myClip = AudioClip.Create(
            "SynthesizerGenerated",
            SampleLength(),
            channels,
            sampleRate,
            true,
            OnAudioRead
        );
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }

    private void OnAudioRead(float[] data)
    {
        foreach (var action in _preReadActions)
        {
            action();
        }

        CalculateIncrement();

        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = 0;
            data[i] += NextSample();

            if (channels == 2) data[i + 1] = data[i];
        }
    }

    public void AddPreReadAction(Action action)
    {
        _preReadActions.Add(action);
    }

    public void RemovePreReadAction(Action action)
    {
        _preReadActions.Remove(action);
    }

    protected abstract void CalculateIncrement();

    protected abstract float NextSample();

    protected int SampleLength() {
        return sampleRate * 2;
    }
}