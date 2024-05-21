using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAllChild : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    AudioSource[] audioSources;
    private void Awake()
    {
        particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        audioSources = transform.GetComponentsInChildren<AudioSource>();
    }
    private void OnEnable()
    {
        Play();
    }

    public void Play()
    {
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Play();
        }
        foreach (var audioSource in audioSources)
        {
            audioSource.Play();
        }
    }
    public void Pause()
    {
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Pause();
        }
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }
}
