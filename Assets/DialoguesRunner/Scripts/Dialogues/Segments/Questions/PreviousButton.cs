using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PreviousButton : MonoBehaviour, INavigationButton
{
    public void PerformAction(DialoguesRunner dialoguesRunner)
    {
        var questionManager = dialoguesRunner.GetActiveManager<QuestionManager>();
        var audioManager = dialoguesRunner.GetActiveManager<AudioManager>();
        var animManager = dialoguesRunner.GetActiveManager<AnimationManager>();

        questionManager.FinishSegmentAndStepBack();
        audioManager.GoToPreviousSegment();
        animManager.GoToPreviousSegment();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
