
using System.IO;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;

public class SingleQuestion : MonoBehaviour
{
    private string statement;
    private TextMeshProUGUI statementText;
    private IQuestion question;
    private GameObject questionPrefab;
    private float ansTime;  
    private static int nextId = 1;
    private int id;

    // TODO: This class needs to Display the Question and the statement above
    public void Start()
    {
        //DisplayQuestion();
        //startTime = Time.time;
    }

    public void InitializeSingleQuestion(string statement, IQuestion question, GameObject questionPrefab)
    {
        this.statementText = questionPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        this.statement = statement;
        this.statementText.text = this.statement;
        this.question = question;
        this.questionPrefab = questionPrefab;
        this.id = nextId++;
        DisplayQuestion();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    // Update is called once per frame
    public void DisplayQuestion()
    {
        question.DisplayQuestion(this.questionPrefab);
    }

    public void SetIQuestion(IQuestion question)
    {
        this.question = question;
    }

    public void SetStatement(string statement)
    {
        this.statement = statement;
    }

    public string GetStatement()
    {
        return this.statement;
    }

    public string GetQuestionType()
    {
        return question.GetQuestionType();
    }

    public IQuestion GetQuestion()
    {
        return this.question;
    }

    public string GetAnswer()
    {
        return this.question.GetAnswer(); // Delegates to the IQuestion implementation.
    }

    public string GetId()
    {
        return id.ToString();
    }

    public float GetResponseTime()
    {
        if (this.question.GetAnswerTime() > 0f)
        {
            return this.question.GetAnswerTime();
        }
        else return this.question.GetAnswerTime();
    }

}