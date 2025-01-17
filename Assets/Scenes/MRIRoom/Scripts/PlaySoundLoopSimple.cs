using UnityEngine;

public class PlaySoundLoopSimple : MonoBehaviour
{
    private AudioSource audioSource;

    public void Start()
    {
        // Add an AudioSource component to the GameObject if not already added
        audioSource = gameObject.AddComponent<AudioSource>();

        // Configure the AudioSource for looping
        audioSource.loop = true;
    }

    // Function to play a sound
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("No AudioClip assigned!");
            return;
        }

        // Assign the clip to the AudioSource and play
        audioSource.clip = clip;
        audioSource.volume = 0.15f;
        audioSource.Play();
    }

    // Function to stop playing the sound
    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
