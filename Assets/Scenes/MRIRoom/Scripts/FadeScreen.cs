using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public float fadeDuration = 2f;
    public Color fadeColor;
    public Renderer rend;
    public bool fadeOnStart = true;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn();
        }
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
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public void Fade(float alphaOut)
    {
        StartCoroutine(FadeRoutine(rend.material.color.a, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        GetComponent<MeshRenderer>().enabled = true;
        float timer = 0;
        while (timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);

            rend.material.SetColor("_Color", newColor);

            timer += Time.deltaTime;
            yield return null;
        }

        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_Color", newColor2);
        GetComponent<MeshRenderer>().enabled = false;
    }
}