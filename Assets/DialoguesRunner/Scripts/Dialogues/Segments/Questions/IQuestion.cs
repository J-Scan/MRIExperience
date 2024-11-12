using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestion
{
    void DisplayQuestion(GameObject questionGO);
    string GetAnswer();

    float GetAnswerTime();

    string GetQuestionType();


}