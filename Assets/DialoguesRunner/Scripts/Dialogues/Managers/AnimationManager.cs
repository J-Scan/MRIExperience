using UnityEngine;
using System.Collections.Generic;
using System;



[System.Serializable]
public class AnimationSegmentAttributes
{
    public AnimationClip animationClip;
    public GameObject targetGameObject;

    public float animationSpeed = 1f;
    public bool loop = false;
}

public class AnimationManager : MonoBehaviour, IDialogue
{
    [SerializeField] private AnimationSegmentAttributes[] animationSegmentsAttributes;

    private List<AnimationSegment> animSegments;
    private int currentSegmentIndex = 0;
    private bool isFinished = false;

    private void Awake()
    {
        animSegments = new List<AnimationSegment>();

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
    }

    public void CreateAndPlayNextSegment(){
        if (currentSegmentIndex < animationSegmentsAttributes.Length && currentSegmentIndex>=0)
        {
            var attributes = animationSegmentsAttributes[currentSegmentIndex];
            //Debug.Log($"Creating segment {currentSegmentIndex}");

            GameObject segmentObject = new GameObject("AnimationSegment" + currentSegmentIndex);
            AnimationSegment newSegment = segmentObject.AddComponent<AnimationSegment>();

            newSegment.InitializeSegment(attributes.animationClip, attributes.targetGameObject, attributes.animationSpeed, attributes.loop);
            animSegments.Add(newSegment);
            newSegment.Play();

            currentSegmentIndex++;
        }
    }

    public void GoToNextSegment()
    {
        if (currentSegmentIndex < animationSegmentsAttributes.Length)
        {
            CreateAndPlayNextSegment();
        }
        else
        {
            isFinished = true;
        }
    }

    public void GoToPreviousSegment()
    {
        if (currentSegmentIndex > 0)
        {
            currentSegmentIndex = currentSegmentIndex - 2;
            CreateAndPlayNextSegment();
        }
    }

    public bool HasFinished()
    {
        return isFinished;
    }

    public void OnFinish()
    {
        foreach (var segment in animSegments)
        {
            Destroy(segment.gameObject);
        }

        foreach (var segmentAttributes in animationSegmentsAttributes)
        {
            GameObject targetGameObject = segmentAttributes.targetGameObject;
            
            if (targetGameObject != null)
            {
                Animation animationComponent = targetGameObject.GetComponent<Animation>();
                animationComponent.Stop();
            }
        }
        animSegments.Clear();
    }

    public bool IsSegmentFinished()
    {
        if (currentSegmentIndex <= 0 || currentSegmentIndex > animSegments.Count)
        {
            return false;
        }

        return animSegments[currentSegmentIndex-1].HasFinished();
    }

}
