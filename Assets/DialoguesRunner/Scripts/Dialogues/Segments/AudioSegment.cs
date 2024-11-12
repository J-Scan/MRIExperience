using UnityEngine;

public class AudioSegment : MonoBehaviour
{
    private AudioSource audioSource;

    public void Update(){

    }

    public void InitializeSegment(AudioSource audioSource, AudioClip clip, float playbackSpeed)
    {
        this.audioSource = audioSource;
        this.audioSource.clip = clip;
        this.audioSource.pitch = playbackSpeed;
    }

    public void Play()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is not initialized!");
        }
    }

    public bool HasFinished()
    {
        return audioSource != null && !audioSource.isPlaying;
    }
}
