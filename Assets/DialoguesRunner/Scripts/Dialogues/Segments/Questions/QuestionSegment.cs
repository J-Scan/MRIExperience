using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestionSegmentAttributes;

public class QuestionSegment : MonoBehaviour, IDialogueSegment
{
    private string title;
    private string statement;
    private List<SingleQuestion> questions = new List<SingleQuestion>();
    private List<GameObject> navigationButtons = new List<GameObject>();
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI statementText;
    private bool isFinished = false;

    private bool isAdvancing = false;
    private Coroutine clickWithDelayCoroutine;

    public void InitializeSegment(TextMeshProUGUI titleText, string title, TextMeshProUGUI statementText, string statement, SingleQuestionAttributes[] questionAttributesArray, GameObject questionTypesPrefab, GameObject likertScalePreab, GameObject openAnswerPrefab, QuestionSegmentAttributes.ButtonType buttonType, GameObject[] navigationButtonGameObjects)
    {
        this.titleText = titleText;
        this.title = title;
        this.titleText.text = this.title;

        this.statementText = statementText;
        this.statement = statement;
        this.statementText.text = this.statement;
        this.isAdvancing = false;

        //DestroyOldPrefabs(questionTypesPrefab);
        DisableOldPrefabs(questionTypesPrefab.transform.GetChild(0).gameObject);
        DisableOldPrefabs(questionTypesPrefab.transform.GetChild(1).gameObject);

        ClearNavigationButtons(navigationButtonGameObjects);

        //navigationButtonGameObjects[2].GetComponent<Button>().interactable = false;
        navigationButtons = NavigationButton.CreateButtons(navigationButtonGameObjects, buttonType);
        UpdateNavigationButtons(buttonType);

        InstanciateSingleQuestions(questionAttributesArray, questionTypesPrefab, likertScalePreab, openAnswerPrefab);
        ManageNextButton();
    }

    public GameObject GetNextButtonGO()
    {
        foreach (var buttonGameObject in navigationButtons)
        {
            NextButton nextBut = buttonGameObject.GetComponent<NextButton>();
            if (nextBut)
            {
                return buttonGameObject;
            }
        }

        return null;
    }

    public void DisableNextButton()
    {
        GameObject nextButton = GetNextButtonGO();
        if (nextButton != null)
        {
            nextButton.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableNextButton()
    {
        GameObject nextButton = GetNextButtonGO();
        if (nextButton != null)
        {
            nextButton.GetComponent<Button>().interactable = true;
        }
    }

    public void ClickOnNextButton()
    {
        if (gameObject.activeInHierarchy)
        {
            clickWithDelayCoroutine = StartCoroutine(ClickWithDelay());
        }
    }

    private IEnumerator ClickWithDelay()
    {
        GameObject nextButton = GetNextButtonGO();

        if (nextButton != null)
        {
            yield return new WaitForSeconds(2f);
            isAdvancing = false;
            nextButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public bool IsSegmentDone()
    {
        if (questions!=null && questions.Count > 0)
        {
            foreach (var question in questions)
            {
                if (question.gameObject.activeInHierarchy)
                {
                    if (question.GetAnswer() == "-99")
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ManageNextButton()
    {
        if (questions.Count > 0)
        {
            foreach (var question in questions)
            {
                if (question.gameObject.activeInHierarchy)
                {
                    if (question.GetQuestionType() == "OpenAnswer")
                    {
                        EnableNextButton();
                        return;
                    }
                    else if (question.GetQuestionType() == "Likert5Scale" && !IsSegmentDone())
                    {
                        DisableNextButton();
                        return;
                    }
                }
            }

        }
        EnableNextButton();
    }

    public void AutoAdvance()
    {
        isAdvancing = true;
        ClickOnNextButton();
    }

    public void InstanciateSingleQuestions(SingleQuestionAttributes[] questionAttributesArray, GameObject questionTypesPrefab, GameObject likertScalePrefab, GameObject openAnswerPrefab)
    {

        foreach (SingleQuestionAttributes questionAttributes in questionAttributesArray)
        {
            SingleQuestion existingQuestion = questions.Find(q => q.GetStatement() == questionAttributes.statement);

            if (existingQuestion != null)
            {
                gameObject.SetActive(true);
                existingQuestion.SetActive(true);
            }
            else
            {
                GameObject singleQuestionGO = new GameObject("SingleQuestion");
                singleQuestionGO.transform.SetParent(transform);

                SingleQuestion newQuestion = singleQuestionGO.AddComponent<SingleQuestion>();

                switch (questionAttributes.type)
                {
                    case SingleQuestionAttributes.QuestionType.Likert5Scale:
                        GameObject likertGOins = GameObject.Instantiate(likertScalePrefab, likertScalePrefab.transform.parent);
                        Likert5Scale likert5ScaleQuestion = likertGOins.transform.GetChild(1).GetComponent<Likert5Scale>();
                        newQuestion = likertGOins.GetComponent<SingleQuestion>();
                        newQuestion.InitializeSingleQuestion(questionAttributes.statement, likert5ScaleQuestion, likertGOins);
                        
                        likert5ScaleQuestion.OnLikertOptionSelectedEvent += () => {
                            PerformAutoAdvance();
                        };
                        
                        break;
                    case SingleQuestionAttributes.QuestionType.OpenAnswer:
                        GameObject openAGOins = GameObject.Instantiate(openAnswerPrefab, openAnswerPrefab.transform.parent);
                        OpenAnswer openAnswerQuestion = openAGOins.transform.GetChild(1).GetComponent<OpenAnswer>();
                        newQuestion = openAGOins.GetComponent<SingleQuestion>();
                        newQuestion.InitializeSingleQuestion(questionAttributes.statement, openAnswerQuestion, openAGOins);
                        break;
                    default:
                        Debug.LogError("Unknown question type: " + questionAttributes.type);
                        break;
                }

                if (newQuestion != null)
                {
                    questions.Add(newQuestion);
                }
            }
        }
    }


    public void DestroyOldPrefabs(GameObject prefab)
    {
        int childCount = prefab.transform.childCount;
        for (int i = 1; i < childCount; i++)
        {
            Transform child = prefab.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void DisableOldPrefabs(GameObject prefab)
    {
        int childCount = prefab.transform.childCount;
        for (int i = 1; i < childCount; i++)
        {
            Transform child = prefab.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }

    public void Advance()
    {
    }

    public void Retreat()
    {
    }

    public void SetHasFinished(bool value)
    {
        isFinished = value;
    }

    public bool HasFinished()
    {
        return isFinished;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Finish()
    {
        this.SetHasFinished(true);
        gameObject.SetActive(false);
    }

    private void UpdateNavigationButtons(QuestionSegmentAttributes.ButtonType buttonType)
    {
        // Deactivate all navigation buttons first
        foreach (var button in navigationButtons)
        {
            button.SetActive(false);
        }

        // Activate buttons based on the provided buttonType
        switch (buttonType)
        {
            case QuestionSegmentAttributes.ButtonType.Start:
                ActivateButtonOfType<StartButton>();
                break;
            case QuestionSegmentAttributes.ButtonType.Previous:
                ActivateButtonOfType<PreviousButton>();
                break;
            case QuestionSegmentAttributes.ButtonType.Next:
                ActivateButtonOfType<NextButton>();
                break;
            case QuestionSegmentAttributes.ButtonType.PreviousAndNext:
                ActivateButtonOfType<PreviousButton>();
                ActivateButtonOfType<NextButton>();
                break;
            case QuestionSegmentAttributes.ButtonType.PreviousAndEnd:
                ActivateButtonOfType<PreviousButton>();
                ActivateButtonOfType<FinishButton>();
                break;
            case QuestionSegmentAttributes.ButtonType.End:
                ActivateButtonOfType<FinishButton>();
                break;
        }
    }

    private void ActivateButtonOfType<T>() where T : MonoBehaviour
    {
        foreach (var button in navigationButtons)
        {
            if (button.GetComponent<T>() != null)
            {
                button.SetActive(true);
                break;
            }
        }
    }


    private void ClearNavigationButtons(GameObject[] navigationButtonGameObjects)
    {
        foreach (GameObject navbut in navigationButtonGameObjects){
            navbut.SetActive(false);
        }
    }

    public List<SingleQuestion> GetQuestions()
    {
        return questions;
    }

    public void PerformAutoAdvance()
    {
        if (IsSegmentDone() && !isAdvancing)
        {
            EnableNextButton();
            AutoAdvance();
        }
        else if(!IsSegmentDone())
        {
            DisableNextButton();
            if (clickWithDelayCoroutine != null)
            {
                StopCoroutine(clickWithDelayCoroutine);
                clickWithDelayCoroutine = null;
            }
            isAdvancing = false;
        }

    }

    public void Update()
    {
        
    }

    public void StopAllCoroutinesBeforeDeactivation()
    {
        if (clickWithDelayCoroutine != null)
        {
            StopCoroutine(clickWithDelayCoroutine);
            clickWithDelayCoroutine = null;
        }
        isAdvancing = false;
    }

    public void OnDisable()
    {
        StopAllCoroutinesBeforeDeactivation();
    }
}
