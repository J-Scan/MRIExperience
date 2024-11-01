using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountdownAudioPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] UnityEvent OnFinishPlayback;

    private Coroutine countdownCoroutine;
    private Coroutine audioLoopCoroutine;
    private float countdownTime = 30f;
    private bool isStoppedExternally = false;
    private int currentClipIndex = 0;

    bool playing;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned or found!");
            }
        }
    }

    public void SetCurrentClipIndex(int clipIndex)
    {
        this.currentClipIndex = clipIndex;
    }

    public void SetCountDownTimer(float countDownTime)
    {
        this.countdownTime = countDownTime;
    }

    /*
    public void PlayNextClipAfterCountdownUntilStop(int clipIndexStart)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        currentClipIndex = clipIndexStart;
        countdownCoroutine = StartCoroutine(CountdownRoutineUntilStop());
    }
    */


    public void StopCountdownExternally()
    {
        if (countdownCoroutine != null)
        {
            isStoppedExternally = true;
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        if (audioLoopCoroutine != null)
        {
            StopCoroutine(audioLoopCoroutine);
            audioLoopCoroutine = null;
        }
    }
    /*
    private void PlayNextAudioClip()
    {
        if (audioClips != null && audioClips.Count > 0)
        {
            if (audioSource != null)
            {
                currentClipIndex = (currentClipIndex + 1) % audioClips.Count;
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
                playing = true;
            }
        }
    }
    */

    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            playing = false;
            //OnFinishPlayback.Invoke();
        }
    }

    public void PlaySpecificClipAfterCountdown(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= audioClips.Count)
        {
            return;
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(PlaySpecificClipAfterCountdownRoutine(clipIndex));
    }

    private IEnumerator PlaySpecificClipAfterCountdownRoutine(int clipIndex)
    {
        float timer = countdownTime;
        isStoppedExternally = false;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer--;

            if (isStoppedExternally)
            {
                countdownCoroutine = null;
                yield break;
            }
        }

        if (!isStoppedExternally)
        {
            audioSource.clip = audioClips[clipIndex];
            audioSource.Play();
            playing = true;
        }
    }

    public void PlaySpecificClipInLoopAfterCountdown(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= audioClips.Count)
        {
            return;
        }

        isStoppedExternally = false;
        Debug.Log("Before coroutine");

        if (countdownCoroutine != null)
        {
            StopCountdownExternally();
        }

        countdownCoroutine = StartCoroutine(PlaySpecificClipInLoopAfterCountdownRoutine(clipIndex));
    }

    private IEnumerator PlaySpecificClipInLoopAfterCountdownRoutine(int clipIndex)
    {
        Debug.Log("Started coroutine");
        while (!isStoppedExternally)
        {
            // Attendre que le clip se termine
            yield return new WaitWhile(() => audioSource.isPlaying);

            // Attendre 30 secondes
            yield return new WaitForSeconds(countdownTime);

            if (!isStoppedExternally)
            {
                audioSource.clip = audioClips[clipIndex];
                audioSource.Play();
                playing = true;
            }
        }
        countdownCoroutine = null;
    }

    public void PlayNextClipAfterCountdown()
    {
        Debug.Log("Before coroutine");
        isStoppedExternally = false;
        if (countdownCoroutine != null)
        {
            StopCountdownExternally();
        }

        countdownCoroutine = StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        Debug.Log("Started coroutine");
        if (!isStoppedExternally)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);

            yield return new WaitForSeconds(countdownTime);

            if (currentClipIndex >= audioClips.Count)
            {
                currentClipIndex = 0;
            }
            if (!isStoppedExternally)
            {
                Debug.Log("is playing: " + audioClips[currentClipIndex] + "at index: " + currentClipIndex);
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
                playing = true;
                currentClipIndex = currentClipIndex + 1;
            }
        }
        Debug.Log("Was stopped externally");
        countdownCoroutine = null;
    }

    public void PlayNextClipAfterCountdownUntilStop()
    {
        Debug.Log("Before coroutine");
        isStoppedExternally = false;
        if (countdownCoroutine != null)
        {
            StopCountdownExternally();
        }

        countdownCoroutine = StartCoroutine(CountdownRoutineUntilStop());
    }

    private IEnumerator CountdownRoutineUntilStop()
    {
        Debug.Log("Started coroutine");
        while (!isStoppedExternally)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);

            yield return new WaitForSeconds(countdownTime);

            if (currentClipIndex >= audioClips.Count)
            {
                currentClipIndex = 0;
            }
            if (!isStoppedExternally)
            {
                Debug.Log("is playing: " + audioClips[currentClipIndex] + "at index: " + currentClipIndex);
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
                playing = true;
                currentClipIndex = currentClipIndex + 1;
            }
        }
        Debug.Log("Was stopped externally");
        countdownCoroutine = null;
    }


    void Update()
    {
        if (!playing) return;

        // Check if the AudioSource is still playing
        if (!audioSource.isPlaying)
        {
            playing = false;
            isStoppedExternally = false;
            OnFinishPlayback.Invoke();
        }
    }
}