using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

[System.Serializable]
public class QuestionSegmentAttributes
{
    public string title;
    [TextArea(3, 10)]
    public string statement;
    public SingleQuestionAttributes[] questions;

    public enum ButtonType { Start, Previous, Next, PreviousAndNext, PreviousAndEnd, End };
    public ButtonType buttonType = ButtonType.PreviousAndNext;
}

[System.Serializable]
public class SingleQuestionAttributes
{
    public enum QuestionType { Likert5Scale, OpenAnswer };
    public QuestionType type;
    public string statement;
}

public class QuestionManager : MonoBehaviour, IDialogue
{
    [SerializeField] private QuestionSegmentAttributes[] questionSegmentsAttributes;
    [SerializeField] private Canvas questionCanvas; // Référence au Canvas
    [SerializeField] private TextMeshProUGUI segmentTitleText;
    [SerializeField] private TextMeshProUGUI segmentStatementText;
    [SerializeField] private TextMeshProUGUI segmentsCount;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject questionTypesPrefab;
    [SerializeField] private GameObject likertScalePrefab;
    [SerializeField] private GameObject openAnswerPrefab;

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject endButton;

    private List<QuestionSegment> questionSegments;
    private int currentSegmentIndex = 0;
    private int nbSegments = 0;

    private bool isFinished = false;
    private bool reachedEnd = false;

    private void Awake()
    {
        questionSegments = new List<QuestionSegment>();
        if (questionCanvas != null)
        {
            questionCanvas.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        try
        {
            this.nbSegments = questionSegmentsAttributes.Length;
            if (progressBar != null)
            {
                progressBar.minValue = 0;
                progressBar.maxValue = nbSegments;
            }

            CreateQuestionSegment(questionSegmentsAttributes[0]);
            currentSegmentIndex = 1;
            UpdateProgress();

            if (questionCanvas != null)
            {
                questionCanvas.gameObject.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create the next segment: " + e.Message);
        }
       
    }

    public void Update()
    {
    }

    public void FinishSegmentAndStepFwd()
    {
        if (currentSegmentIndex > 0 && currentSegmentIndex < questionSegmentsAttributes.Length)
        {
            questionSegments[currentSegmentIndex - 1].Finish();
            GoToNextSegment();
        }
    }

    public void FinishSegmentAndStepBack()
    {
        if (currentSegmentIndex > 1)
        {
            questionSegments[currentSegmentIndex - 1].Finish();
            GoToPreviousSegment();
        }
    }

    public void GoToNextSegment()
    {
        if (currentSegmentIndex < questionSegmentsAttributes.Length)
        {
            CreateQuestionSegment(questionSegmentsAttributes[currentSegmentIndex]);
            currentSegmentIndex++; // Increment after creating the next segment
            UpdateProgress();
        }
    }

    public void GoToPreviousSegment()
    {
        if (currentSegmentIndex > 1)
        {
            currentSegmentIndex -= 2; // Decrement index to point to the previous segment
            CreateQuestionSegment(questionSegmentsAttributes[currentSegmentIndex]);
            //questionSegments.RemoveAt(currentSegmentIndex);
            currentSegmentIndex++; // Increment to correct position after creating the previous segment
            UpdateProgress();
        }
    }

    private void UpdateProgress()
    {
        int progressInPourcent;
        progressInPourcent = (int)Math.Truncate((float)currentSegmentIndex / nbSegments * 100);

        segmentsCount.text = $"{progressInPourcent} %";
        if (progressBar != null)
        {
            progressBar.value = currentSegmentIndex;
        }
    }

    public Dictionary<string, string> GetAllAnswers()
    {
        var allAnswers = new Dictionary<string, string>();

        foreach (var segment in questionSegments)
        {
            foreach (var question in segment.GetQuestions())
            {
                var questionId = question.GetId();
                var statement = question.GetStatement(); // Retrieves the statement
                var answer = question.GetAnswer(); // Retrieves the answer
                var responseTime = question.GetResponseTime(); // Retrieves the response time

                allAnswers[questionId] = answer;
            }
        }

        return allAnswers;
    }

    public void ExportToCSV()
    {
        DateTime now = DateTime.Now;
        string dirName = "ParticipantsData";
        string formattedDateTime = now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = Path.Combine(dirName, $"{formattedDateTime}_answers.csv");

        Directory.CreateDirectory(dirName);

        using (var writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
        {
            writer.WriteLine("id,statement,answer,answer.rt");

            foreach (var segment in questionSegments)
            {
                foreach (var question in segment.GetQuestions())
                {
                    var questionId = question.GetId();
                    var statement = question.GetStatement();
                    var answer = question.GetAnswer();
                    var responseTime = question.GetResponseTime();

                    writer.WriteLine($"{EscapeCsv(questionId)},{EscapeCsv(statement)},{EscapeCsv(answer)},{responseTime}");
                }
            }
        }
    }


    private string EscapeCsv(string value)
    {
        if (value.Contains("\""))
        {
            value = value.Replace("\"", "\"\"");
        }
        if (value.Contains(",") || value.Contains("\n"))
        {
            value = $"\"{value}\"";
        }
        return value;
    }

    public void CreateQuestionSegment(QuestionSegmentAttributes attributes)
    {
        GameObject[] navigationButtons = { startButton, previousButton, nextButton, endButton };

        Debug.Log("current seg index " + currentSegmentIndex);

        if (questionSegments.Count > 0 && currentSegmentIndex < questionSegments.Count)
        {
            QuestionSegment oldSegment = questionSegments[currentSegmentIndex];
            oldSegment.SetActive(true);
            oldSegment.InitializeSegment(segmentTitleText, attributes.title, segmentStatementText, attributes.statement, attributes.questions, questionTypesPrefab, likertScalePrefab, openAnswerPrefab, attributes.buttonType, navigationButtons);
        }

        else
        {
            GameObject segmentObject = new GameObject("QuestionSegment_" + currentSegmentIndex);
            QuestionSegment newSegment = segmentObject.AddComponent<QuestionSegment>();
            newSegment.InitializeSegment(segmentTitleText, attributes.title, segmentStatementText, attributes.statement, attributes.questions, questionTypesPrefab, likertScalePrefab, openAnswerPrefab, attributes.buttonType, navigationButtons);
            questionSegments.Add(newSegment);
        }

    }

    public void SetHasFinished(bool finish)
    {
        isFinished = finish;
    }

    public bool HasFinished()
    {
        return isFinished;
    }

    public bool IsSegmentFinished()
    {
        if (currentSegmentIndex < 1 || currentSegmentIndex > questionSegments.Count)
        {
            return false;
        }
        else return questionSegments[currentSegmentIndex - 1].HasFinished();
    }

    public void DeactivateCanvas()
    {
        if (questionCanvas != null)
        {
            questionCanvas.gameObject.SetActive(false);
        }
    }

    public void DestroyAllSegments()
    {
        foreach (var segment in questionSegments)
        {
            segment.DestroyOldPrefabs(questionTypesPrefab);
            segment.gameObject.SetActive(false);
        }
    }

    public void OnFinish()
    {
        reachedEnd = true;
        ExportToCSV();
        DestroyAllSegments();
        SetHasFinished(true);
        DeactivateCanvas();
    }

    public void OnApplicationQuit()
    {
        if(!reachedEnd)
        {
            OnFinish();
        }
    }
}
