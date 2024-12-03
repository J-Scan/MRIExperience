using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class ManagedAudioSource : MonoBehaviour
{
    [SerializeField] AudioSource src;
    [SerializeField] UnityEvent OnStartPlayback;
    [SerializeField] UnityEvent OnFinishPlayback;

    bool playing;

    void Awake()
    {
        if (src == null) src = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float vol = 1)
    {
        // Set the clip to the AudioSource and play it
        src.clip = clip;
        src.volume = vol;
        src.Play();

        playing = true;
        OnStartPlayback.Invoke();
    }

    public void PlayFromInspector(AudioClip clip)
    {
        this.Play(clip, 1);
    }

    public void StopAudio()
    {
        if (src != null && src.isPlaying && Time.timeScale != 0f)
        {
            src.Stop();
            playing = false;  // Update the playing flag when stopped
            OnFinishPlayback.Invoke();  // Manually invoke the event when audio stops
        }
    }

    void Update()
    {
        if (!playing) return;

        // Check if the AudioSource is still playing
        if (!src.isPlaying && Time.timeScale != 0f)
        {
            playing = false;
            OnFinishPlayback.Invoke();
        }
    }
}
