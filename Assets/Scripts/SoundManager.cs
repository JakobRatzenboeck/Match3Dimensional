using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    GameSettings GS;
    public AudioClip crincleAudioClip;
    public float volume;
    AudioSource crincle;

    void Awake()
    {
        GS = GameSettings.Instance;
        crincle = AddAudio(crincleAudioClip);
        crincle.volume = volume;
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

    void Update()
    {
        if(crincle.volume != volume)
            crincle.volume = volume;
    }
}