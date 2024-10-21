using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownAudioPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;  // List of audio clips to play, set in the inspector
    [SerializeField] private AudioSource audioSource;     // The AudioSource component, set in the inspector or automatically found

    private Coroutine countdownCoroutine;                 // Coroutine for countdown
    private float countdownTime = 30f;                    // The countdown duration (30 seconds)
    private bool isStoppedExternally = false;             // To track if the countdown was stopped externally
    private int currentClipIndex = 0;                     // Index to track the current audio clip

    void Start()
    {
        // Ensure the AudioSource component is available (auto-assign if not set in inspector)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned or found!");
            }
        }
    }

    public void SetCountDownTimer(float countDownTime)
    {
        this.countdownTime = countDownTime;
    }

    // Function to start the countdown
    public void StartCountdown()
    {
        // If a countdown is already running, stop it before starting a new one
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        // Start a new countdown
        countdownCoroutine = StartCoroutine(CountdownRoutine());
    }

    // Countdown routine that runs for 30 seconds
    private IEnumerator CountdownRoutine()
    {
        float timer = countdownTime;
        isStoppedExternally = false;  // Reset external stop flag

        while (timer > 0f)
        {
            // Wait for 1 second each frame to reduce the timer
            yield return new WaitForSeconds(1f);
            timer--;

            // If stopped externally, reset the countdown and stop here
            if (isStoppedExternally)
            {
                Debug.Log("Countdown stopped externally and reset.");
                yield break;
            }
        }

        // After 30 seconds, if no external stop occurred, play the current audio clip
        PlayNextAudioClip();
    }

    // Stop the countdown from an external trigger
    public void StopCountdownExternally()
    {
        if (countdownCoroutine != null)
        {
            isStoppedExternally = true;  // Mark it as externally stopped
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    // Function to play the next audio clip in the list
    private void PlayNextAudioClip()
    {
        if (audioClips != null && audioClips.Count > 0)
        {
            if (audioSource != null)
            {
                // Play the current clip based on the currentClipIndex
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
                Debug.Log($"Playing audio clip {currentClipIndex + 1}: {audioClips[currentClipIndex].name}");

                // Move to the next clip in the list, looping back to the first if at the end
                currentClipIndex = (currentClipIndex + 1) % audioClips.Count;
            }
        }
        else
        {
            Debug.LogWarning("No audio clips are set in the list.");
        }
    }

    // Optional: Function to stop the current audio being played
    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
