using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPeer : MonoBehaviour {

    [HideInInspector]
    public AudioSource audioSource;
    public static float[] samples;
    public static float[] freqBand = new float[8];
    public static float[] bandBuffer = new float[8];

    private float[] bufferDecrease = new float[8];
    private float sampleRate;

    private const int SAMPLE_COUNT = 512;
    private const int BAND_COUNT = 8;

	void Start () {
        samples = new float[SAMPLE_COUNT];
        sampleRate = AudioSettings.outputSampleRate;
	}
	
	// Update is called once per frame
	void Update () {
        if (audioSource == null)
            return;
        if(audioSource.isPlaying){
            audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
            MakeFrequencyBands();
            BandBuffer();
        }
	}

    void BandBuffer(){
        for (int g = 0; g < 8; g++){
            if(freqBand[g] > bandBuffer[g]){
                bandBuffer[g] = freqBand[g];
                bufferDecrease[g] = 0.005f;
            }
            if (freqBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *= 1.2f;
            }
        }
    }


    void MakeFrequencyBands()
    {
        /*
         * 44100 / 512 = 86 aprox
         * 0 - 2 = 172 Hz
         * 1 - 4 = 344 Hz - 259-602
         * 2 - 8 = 688 Hz - 603-1290
         * 3 - 16 = 1376 Hz - 1291-2666
         * 4 - 32 = 2752 Hz - 2667-5418
         * 5 - 64 = 5504 Hz - 5419-10922
         * 6 - 128 = 11008 Hz - 10923-21930
         * 7 - 256 = 22016 Hz - 21931-43946
         * rest 154
         * 510
         */

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count+1);
                count++;
            }
            average /= count;
            freqBand[i] = average * 10;
        }

    }
}
