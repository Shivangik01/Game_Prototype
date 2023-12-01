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
    public AudioSource Ambient;

    public AudioClip buttonClip;
    public AudioClip digClip;
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
}
