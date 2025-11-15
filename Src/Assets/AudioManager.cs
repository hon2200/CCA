using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    public List<AudioClip> audioClips;
    public AudioClip victoryAudio;
    public AudioSource audioSource;
    public int currentPlayingOrder = 0;
    public int currentPlayingType = 0;
    public bool silence = false;
    public void Next()
    {
        currentPlayingOrder++;
        if(currentPlayingOrder >= audioClips.Count)
            currentPlayingOrder = 0;
        audioSource.clip = audioClips[currentPlayingOrder];
        audioSource.Play();
    }

    public void Silence()
    {
        silence = true;
        audioSource.Stop();
    }
    public void Wakeup()
    {
        silence = false;
        audioSource.Play();
    }

    public void SceneAudioPlay()
    {
        if (silence || currentPlayingType == 0)
            return;
        currentPlayingType = 0;
        audioSource.clip = audioClips[currentPlayingOrder];
        audioSource.Play();
    }

    public void VictoryAudioPlay()
    {
        if (silence || currentPlayingType == 1)
            return;
        currentPlayingType = 1;
        audioSource.clip = victoryAudio;
        audioSource.Play();
    }

}
