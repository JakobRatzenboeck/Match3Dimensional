using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioClip crincleAudioClip;
    AudioSource crincle;

    void Awake()
    {
        crincle = AddAudio(crincleAudioClip);
    }

    AudioSource AddAudio(AudioClip audioClip)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
        return audioSource;
    }

    public void PlayCrincle()
    {
        crincle.Play();
    }
}