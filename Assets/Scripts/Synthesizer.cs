﻿using System.Linq;
using UnityEngine;

public class Synthesizer : AbstractSoundGenerator
{
    public class Frequency
    {
        [Range(50, 500)] public float frequency;
        [Range(0, 1)] public float amplitude;

        protected float phase = 0;
        protected float increment = 0;

        public Frequency(float frequency, float amplitude)
        {
            this.frequency = frequency;
            this.amplitude = amplitude;
        }

        public virtual void CalculateIncrement(float sampleRate)
        {
            increment = frequency * 2f * Mathf.PI / (float) sampleRate;
        }

        public virtual float NextSample()
        {
            phase += increment;
            float nextSample = Mathf.Sin(phase) * amplitude;
            phase %= 2f * Mathf.PI;
            return nextSample;
        }
    }

    public Frequency[] frequencies = {
        //TODO make configurable with unity UI
        new Frequency(97.9989f, 1),
        new Frequency(123.471f, 1),
        new Frequency(146.832f, 1),
        new Frequency(195.998f, 1)
    };

    protected override void CalculateIncrement()
    {
        foreach (var f in frequencies)
        {
            f.CalculateIncrement(sampleRate);
        }
    }

    protected override float NextSample() => frequencies.Select(f => f.NextSample()).Sum() * baseAmplitude;
}