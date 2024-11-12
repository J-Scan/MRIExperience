using System.Collections;
using UnityEngine;

public class AnimationSegment : MonoBehaviour
{
    private AnimationClip animationClip;
    private GameObject targetAnim;
    private Animation animationComponent;
    private bool isFinished;

    //private float animLength;
    private float animSpeed;
    private bool isLooping;

    //private bool runInParallel = false;

    public void InitializeSegment(AnimationClip clip, GameObject target, float speed, bool loop)
    {
        animationClip = clip;
        targetAnim = target;
        animSpeed = speed;
        isFinished = false;
        isLooping = loop;
        SetupAnimationComponent();
    }

    private void SetupAnimationComponent()
    {
        if (animationClip!=null && targetAnim != null){
            animationComponent = targetAnim.GetComponent<Animation>();
            if (animationComponent == null)
            {
                animationComponent = targetAnim.AddComponent<Animation>();
            }
            animationComponent.clip = animationClip;
        }
    }
/*
    public IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(animLength);
        Stop();
    }
*/

    public void Play()
    {
        if (animationComponent != null && animationClip != null)
        {
            animationComponent.Stop();
            if (!animationComponent.GetClip(animationClip.name))
            {
                animationComponent.AddClip(animationClip, animationClip.name);
            }
            animationComponent.clip = animationClip;
            animationComponent[animationClip.name].speed = animSpeed;
            animationComponent.Play();
            SetAutoRepeat();
        }
        else
        {
            Debug.LogWarning("Animation component or clip is null.");
        }
        SetAutoFinish();
    }

    public void Stop()
    {
        if (animationComponent != null)
        {
            animationComponent.Stop();
            isFinished = true;
        }
    }

    public void Update()
    {

    }

    public void SetAutoRepeat(){
        if (isLooping)
        {
            animationComponent[animationClip.name].wrapMode = WrapMode.Loop;
        }
        else
        {
            animationComponent[animationClip.name].wrapMode = WrapMode.Default;
        }
    }

    public void SetAutoFinish(){
        isFinished = true; //unblocking the advancement to the next segment
    }

    public bool HasFinished()
    {
        return isFinished || (animationComponent != null && !animationComponent.IsPlaying(animationClip.name));
    }
}
