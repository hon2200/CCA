using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBGM : MonoBehaviour
{
    public List<AudioClip> audioSources;
    public AudioSource audioSource;
    public int currentPlaying = 0;
    public void Next()
    {
        currentPlaying++;
        if(currentPlaying >= audioSources.Count)
            currentPlaying = 0;
        audioSource.clip = audioSources[currentPlaying];
        audioSource.Play();
    }
}
