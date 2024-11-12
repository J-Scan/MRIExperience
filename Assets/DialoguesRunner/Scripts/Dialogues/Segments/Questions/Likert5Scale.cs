using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Likert5Scale : MonoBehaviour, IQuestion
{
    private GameObject likerScalePrefab;
    private ToggleGroup toggleGroup;
    private Transform questionPosition;
    private float startTime = -99f;
    private float answerTime = -99f;

    public delegate void LikertOptionSelectedHandler();
    public event LikertOptionSelectedHandler OnLikertOptionSelectedEvent;

    public void Start()
    {
        
    }

    public string GetQuestionType()
    {
        return "Likert5Scale";
    }

    public void ResetButtons()
    {
        Transform tog = this.likerScalePrefab.transform.GetChild(1);
        foreach (Transform child in tog)
        {
            UnityEngine.UI.Toggle toggle = child.GetComponent<UnityEngine.UI.Toggle>();
            toggle.isOn = false;
        }

    }

    public void SetRadiosButtonPrefab(GameObject likerScalePrefab)
    {
        this.likerScalePrefab = likerScalePrefab;
    }

    public void DisplayQuestion(GameObject questionGO)
    {
        this.likerScalePrefab = questionGO;

        if (likerScalePrefab != null)
        {
            startTime = Time.time;
            ResetButtons();

            toggleGroup = likerScalePrefab.GetComponentInChildren<ToggleGroup>();

            likerScalePrefab.SetActive(true);
        }
        else
        {
            Debug.LogError("Radios button prefab is not set in Likert5Scale.");
        }
    }

    public void OnLikertOptionSelected()
    {
        answerTime = Time.time;
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        OnLikertOptionSelectedEvent?.Invoke();
    }

    public float GetAnswerTime()
    {
        if (answerTime != -99f)
        {
            return answerTime - startTime;
        }
        else return -99f;
    }


    public string GetAnswer()
    {
        UnityEngine.UI.Toggle[] toggles = toggleGroup.GetComponentsInChildren<UnityEngine.UI.Toggle>();
        float cpt = 1;
        foreach (var t in toggles)
        {
            if (t.isOn)
            {
                return cpt.ToString();
            }
            cpt++;
        }

        return "-99";
    }
}

