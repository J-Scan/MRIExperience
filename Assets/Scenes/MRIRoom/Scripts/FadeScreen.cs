using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    private float fadeDuration = 2f;
    public Renderer rend;
    public bool fadeOnStart = true;


    public void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn();
        }
    }

    public float GetFadeDuration()
    {
        return fadeDuration;
    }

    public void SetFadeRation(float duration)
    {
        fadeDuration = duration;
    }

    public void FadeIn()
    {
        Fade(1, 0);
    }

    public void FadeOut()
    {
        Fade(0, 1);
    }

    public void DoFadeInOut()
    {
        StartCoroutine(FadeInOut(fadeDuration));
    }

    public IEnumerator FadeInOut(float fadeTime)
    {
        //Debug.Log("GoToLocation called");
        FadeOut();
        yield return new WaitForSeconds(fadeTime);
        FadeIn();
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        rend.enabled = true;
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }


    public void PerformFade(float targetAlpha)
    {
        rend.enabled = true;

        if (!rend.material.HasProperty("_Color"))
        {
            Debug.LogError("Material does not have the '_Color' property.");
            return;
        }

        float currentAlpha = rend.material.color.a;

        if (Mathf.Approximately(currentAlpha, targetAlpha))
        {
            return;
        }
        StartCoroutine(FadeRoutine(currentAlpha, targetAlpha));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float timer = 0f;

        Color initialColor = rend.material.color;

        while (timer < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);

            rend.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);

            timer += Time.unscaledDeltaTime;

            yield return null;
        }

        rend.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        if (targetAlpha == 0f)
        {
            rend.enabled = false;
        }

    }
}