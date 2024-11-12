using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMuter : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    // This function finds the active AudioSource in the scene and toggles its mute state
    public void MuteDemuteActiveAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            if (audioSource == null) return;
        }

        if (audioSource != null)
        {
            // Toggle mute state
            audioSource.mute = !audioSource.mute;
        }
        else
        {
            Debug.LogWarning("No active AudioSource found in the scene.");
        }
    }

    // This function takes a GameObject and toggles its active state (enabled/disabled)
    public void ToggleGameObjectActiveState(GameObject targetObject)
    {
        if (targetObject != null)
        {
            // Toggle the active state
            targetObject.SetActive(!targetObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("GameObject provided is null.");
        }
    }
}
