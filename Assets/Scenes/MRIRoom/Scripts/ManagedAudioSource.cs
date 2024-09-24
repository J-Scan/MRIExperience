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
            src.PlayOneShot(clip, vol);
            playing = true;
            OnStartPlayback.Invoke();
        }
        public void PlayFromInspector(AudioClip clip)
        {
            this.Play(clip, 1);
        }

        void Update()
        {
            if (!playing) return;

            if (!src.isPlaying)
            {
                playing = false;
                OnFinishPlayback.Invoke();
            }
        }
    }
