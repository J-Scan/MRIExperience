using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class AudioSegmentAttributes
{
    public AudioClip audioClip;
    public float playbackSpeed = 1.0f;
}

public class AudioManager : MonoBehaviour, IDialogue
{
    [SerializeField] private AudioSegmentAttributes[] audioSegmentsAttributes;
    //private bool isParallelMode;
    private List<AudioSegment> audioSegments;
    private int currentSegmentIndex = 0;
    //private bool canPlayNextSegment = false;

    [SerializeField] private AudioSource audioSource;
    private bool hasFinished = false;

    private void Awake()
    {
        audioSegments = new List<AudioSegment>();
    }

    public void Start()
    {
        try
        {
            CreateAndPlayNextSegment();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create and play the next segment: " + e.Message);
        }
    }

    public void Update()
    {
        /*if (isParallelMode && canPlayNextSegment)
        {
            CreateAndPlayNextSegment();
            canPlayNextSegment = false;
        }
        */
        //UpdateCurrentAudioSegment();
        
    }

/*
    public void UpdateCurrentAudioSegment(){
        if (currentSegmentIndex !=0 && !audioSegments[currentSegmentIndex-1].HasFinished())
        {
            audioSegments[currentSegmentIndex-1].Update();
        }
    }


    public void UpdateAudioSegments(){
        for (int i = audioSegments.Count - 1; i >= 0; i--)
        {
            var segment = audioSegments[i];
            if (segment.HasFinished())
            {
                audioSegments.RemoveAt(i);
                Destroy(segment.gameObject);
            }
            else
            {
                segment.Update();
            }
        }
    }

        public void CreateAllSegments()
    {
        foreach (var attributes in audioSegmentsAttributes)
        {
            GameObject segmentObject = new GameObject("AudioSegment_" + currentSegmentIndex);
            AudioSegment newSegment = segmentObject.AddComponent<AudioSegment>();
            
            newSegment.InitializeSegment(audioSource, attributes.audioClip, attributes.playbackSpeed);
            audioSegments.Add(newSegment);
        }
    }

    */

    public void CreateAndPlayNextSegment()
    {
        if (currentSegmentIndex < audioSegmentsAttributes.Length)
        {
            var attributes = audioSegmentsAttributes[currentSegmentIndex];
            //Debug.Log($"Creating segment {currentSegmentIndex}");

            GameObject segmentObject = new GameObject("AudioSegment_" + currentSegmentIndex);
            AudioSegment newSegment = segmentObject.AddComponent<AudioSegment>();
            
            newSegment.InitializeSegment(audioSource, attributes.audioClip, attributes.playbackSpeed);
            audioSegments.Add(newSegment);
            newSegment.Play();

            currentSegmentIndex++;
        }
    }


    public void GoToNextSegment()
    {
        if (currentSegmentIndex < audioSegmentsAttributes.Length){
            CreateAndPlayNextSegment();
            //RemoveSegment(currentSegmentIndex-2);
        }
        else{
            SetHasFinished(true);
        }
    }

    public void GoToPreviousSegment()
    {
        if (currentSegmentIndex > 0)
        {
            currentSegmentIndex = currentSegmentIndex-2;
            CreateAndPlayNextSegment();
        }
    }

    public void RemoveSegment(int index){
        audioSegments.RemoveAt(index);
    }

    public void SetHasFinished(bool val){
        hasFinished = val;
    }

    public bool HasFinished()
    {
        return hasFinished;
    }

    public bool IsSegmentFinished(){
        if (currentSegmentIndex <= 0 || currentSegmentIndex>audioSegments.Count){
            return false;
        }
        else return audioSegments[currentSegmentIndex-1].HasFinished();
    }

    public void OnFinish()
    {
        foreach (var segment in audioSegments)
        {
            Destroy(segment.gameObject);
        }
        audioSegments.Clear();
    }
}
