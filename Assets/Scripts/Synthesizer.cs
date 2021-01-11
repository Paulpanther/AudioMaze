using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{

        public class Frequency
    {
        [Range(50,500)]
        public float frequency;
        [Range(0, 1)]
        public float amplitude;

        private float phase = 0;
        private float increment = 0;

        public Frequency(float frequency, float amplitude)
        {
            this.frequency = frequency;
            this.amplitude = amplitude;
        }

        public void calculateIncrement(float sampleRate)
        {
            increment = frequency * 2 * Mathf.PI / (float)sampleRate;
        }

        public float nextSample()
        {
            phase += increment;
            float nextSample = Mathf.Sin(phase) * amplitude;
            phase %= 2 * Mathf.PI;
            return nextSample;
        }
    }

    public static int sampleRate = 44100;

    [Range(0, 1)]
    public float baseAmplitude = 0.2f;


    public Frequency[] frequencies = {//TODO make configurable with unity UI
        new Frequency(97.9989f, 1),
        new Frequency(123.471f , 1),
        new Frequency(146.832f, 1),
        new Frequency(195.998f, 1)
    };

    public int channels = 1;

    void Start()
    {
        AudioClip myClip = AudioClip.Create("SynthesizerGenerated", sampleRate * 2, channels, sampleRate, true, OnAudioRead);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }

    void OnAudioRead(float[] data)
    {
        int channels = 1;
        for(int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i].calculateIncrement(sampleRate);
        }

        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = 0;
            for(int j = 0; j < frequencies.Length; j++)
            {
                data[i] += frequencies[j].nextSample() * baseAmplitude;
            }
            if (channels == 2) data[i + 1] = data[i];
        }
    }

}