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

    public void PlayNextClipAfterCountdownUntilStop(int clipIndexStart)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        currentClipIndex = clipIndexStart;
        countdownCoroutine = StartCoroutine(CountdownRoutineUntilStop());
    }


    public void PlayNextClipAfterCountdownUntilStop()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(CountdownRoutineUntilStop());
    }

    private IEnumerator CountdownRoutineUntilStop()
    {
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
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
                playing = true;
                currentClipIndex = currentClipIndex + 1;
            }
        }
    }

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

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(PlaySpecificClipInLoopAfterCountdownRoutine(clipIndex));
    }

    private IEnumerator PlaySpecificClipInLoopAfterCountdownRoutine(int clipIndex)
    {
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