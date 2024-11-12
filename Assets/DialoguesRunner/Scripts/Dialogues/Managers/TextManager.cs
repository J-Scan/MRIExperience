using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class TextSegmentAttributes
{
    [TextArea(3, 10)]
    public string text;
    public Canvas canvas;
    public TextMeshProUGUI uiText;
    public float timePerCharacter = 0.1f;
    public string NPCname = "";
}

public enum DisplayMode
{
    Sequential,
    Parallel
}

public class TextManager : MonoBehaviour, IDialogue
{
    [SerializeField] private TextSegmentAttributes[] textSegmentsAttributes;
    [SerializeField] private DisplayMode displayMode = DisplayMode.Sequential;

    private List<TextSegment> textSegments;
    private int currentSegmentIndex = 0;

    private bool hasFinished = false;

    private void Awake()
    {
        textSegments = new List<TextSegment>();
        DeactivateAllCanvases();
    }

    public void Start()
    {
        ActivateRelevantCanvases();
        if (displayMode == DisplayMode.Parallel)
        {
            CreateAllParallelSegments();
        }

        if (currentSegmentIndex < textSegmentsAttributes.Length && displayMode == DisplayMode.Sequential)
        {
            CreateNextSequentialSegment();
        }
    }

    public void Update()
    {
        //UpdateTextSegments();
        UpdateCurrentTextSegment();
    }

    public void GoToNextSegment()
    {
        
        if (currentSegmentIndex < textSegmentsAttributes.Length && displayMode == DisplayMode.Sequential)
        {
            CreateNextSequentialSegment();
            //RemoveSegment(currentSegmentIndex-2);
        }

        else if(currentSegmentIndex >= textSegmentsAttributes.Length){
            SetHasFinished(true);
        }
    }

    public void GoToPreviousSegment()
    {
        
        if (currentSegmentIndex > 0 && displayMode == DisplayMode.Sequential)
        {
            currentSegmentIndex--;
            CreateNextSequentialSegment();
            //RemoveSegment(currentSegmentIndex-2);
        }

    }

    public void RemoveSegment(int index){
        textSegments.RemoveAt(index);
    }

    private void UpdateCurrentTextSegment()
    {
        if (currentSegmentIndex !=0 && !textSegments[currentSegmentIndex-1].HasFinished())
        {
            textSegments[currentSegmentIndex-1].Update();
        }
    }

    public void UpdateTextSegments()
    {
        for (int i = 0; i < textSegments.Count; i++)
        {
            textSegments[i].Update();
            if (textSegments[i].HasFinished())
            {
                textSegments.RemoveAt(i);
                i--;
            }
        }
    }

    public void CreateNextSequentialSegment()
    {
        if (textSegments.Count == 0 || textSegments[textSegments.Count - 1].HasFinished())
        {
            CreateTextSegment(textSegmentsAttributes[currentSegmentIndex]);
            currentSegmentIndex++;
        }
    }

    public void CreateAllParallelSegments()
    {
        if (textSegments.Count == 0)
        {
            for (int i = currentSegmentIndex; i < textSegmentsAttributes.Length; i++)
            {
                CreateTextSegment(textSegmentsAttributes[i]);
            }
            currentSegmentIndex = textSegmentsAttributes.Length;
        }
    }

    public void CreateTextSegment(TextSegmentAttributes attributes)
    {
        if (attributes.canvas != null)
        {
            attributes.canvas.gameObject.SetActive(true);
        }

        TextSegment newSegment = new TextSegment();
        newSegment.InitializeSegment(attributes.uiText, attributes.text, attributes.timePerCharacter, attributes.NPCname, true);
        textSegments.Add(newSegment);
    }

    public void SetHasFinished(bool val){
        hasFinished = val;
    }

    public bool HasFinished()
    {
        //Debug.Log("Checking if the following class has finished: " + this.GetType().Name + "currentSegmentIndex " + currentSegmentIndex);
        return hasFinished;
    }

    public bool IsSegmentFinished(){
        if (currentSegmentIndex <= 0 || currentSegmentIndex>textSegments.Count){
            return false;
        }
        else return textSegments[currentSegmentIndex-1].HasFinished();
    }

    public void ActivateRelevantCanvases()
    {
        foreach (var attr in textSegmentsAttributes)
        {
            if (attr.canvas != null && !attr.canvas.gameObject.activeSelf)
            {
                attr.canvas.gameObject.SetActive(true);
            }
        }
    }

    public void DeactivateAllCanvases()
    {
        foreach (var attr in textSegmentsAttributes)
        {
            if (attr.canvas != null)
            {
                attr.canvas.gameObject.SetActive(false);
            }
        }
    }

    public void OnFinish()
    {
        DeactivateAllCanvases();
    }
}
