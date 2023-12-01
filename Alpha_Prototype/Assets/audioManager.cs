using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public AudioSource SFX;
    public AudioSource TrainSFX;
    public AudioSource Ambient;

    public AudioClip buttonClip;
    public AudioClip digClip;
    public AudioClip popClip;
    public AudioClip dragClip;
    public AudioClip happyClip;
    public AudioClip sadClip;

    public void playButton()
    {
        SFX.PlayOneShot(buttonClip);
    }

    public void playDig()
    {
        SFX.PlayOneShot(digClip);
    }

    public void playPop()
    {
        SFX.PlayOneShot(popClip);
    }

    public void playDrag()
    {
        SFX.PlayOneShot(dragClip);
    }

    public void playHappy()
    {
        SFX.PlayOneShot(happyClip);
    }

    public void playSad()
    {
        SFX.PlayOneShot(sadClip);
    }

    public void playTrain()
    {
        StartCoroutine(FadeInTrain());
    }

    public void stopTrain()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutTrain());
    }

    IEnumerator FadeInTrain()
    {
        float volume = TrainSFX.volume;
        while(volume < 0.3f)
        {
            volume += Time.deltaTime;
            TrainSFX.volume = volume;

            yield return null;
        }
        TrainSFX.volume = 0.3f;
        yield return null;
    }

    IEnumerator FadeOutTrain()
    {
        float volume = TrainSFX.volume;
        while (volume > 0f)
        {
            volume -= Time.deltaTime;
            TrainSFX.volume = volume;

            yield return null;
        }
        TrainSFX.volume = 0.0f;
        yield return null;
    }
}
