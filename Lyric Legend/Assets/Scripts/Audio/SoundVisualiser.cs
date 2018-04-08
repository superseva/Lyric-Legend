using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisualiser : MonoBehaviour {

    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    [HideInInspector]
    public AudioSource source;

    private const int SAMPLE_SIZE = 512;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

	void Start () {
        //source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
	}
	
	// Update is called once per frame
	void Update () {
        if (source == null)
            return;
        if(source.isPlaying)
            AnalyzeSound();
	}


    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        // RMS VALUE
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //DB VALUE
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        //GET SOUND SPECTRUM
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //PITCH VALUE
        float maxV = 0;
        int maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        { 
            if(!(spectrum[i]>maxV) || !(spectrum[i] > 0.0f))
                continue;

            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if(maxN > 0 && maxN < SAMPLE_SIZE -1)
        {
            float dL = spectrum[maxN - 1] / spectrum[maxN];
            float dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;// convert index to frequency
    }
}
