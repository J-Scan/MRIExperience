using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FinishButton : MonoBehaviour, INavigationButton
{
    public void PerformAction(DialoguesRunner dialoguesRunner)
    {
        var questionManager = dialoguesRunner.GetActiveManager<QuestionManager>();
        var audioManager = dialoguesRunner.GetActiveManager<AudioManager>();
        var animManager = dialoguesRunner.GetActiveManager<AnimationManager>();

        questionManager.OnFinish();
        audioManager.GoToNextSegment();
        animManager.GoToNextSegment();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
