using System.Collections.Generic;
using UnityEngine;

public class NavigationButton
{
    public static List<GameObject> CreateButtons(GameObject[] buttonGameObjects, QuestionSegmentAttributes.ButtonType buttonType)
    {
        var buttons = new List<GameObject>();

        foreach (var buttonGameObject in buttonGameObjects)
        {
            switch (buttonType)
            {
                case QuestionSegmentAttributes.ButtonType.Start:
                    var startButton = buttonGameObject.GetComponent<StartButton>();
                    if (startButton != null)
                        buttons.Add(buttonGameObject);
                    break;
                case QuestionSegmentAttributes.ButtonType.Previous:
                    var previousButton = buttonGameObject.GetComponent<PreviousButton>();
                    if (previousButton != null)
                        buttons.Add(buttonGameObject);
                    break;
                case QuestionSegmentAttributes.ButtonType.Next:
                    var nextButton = buttonGameObject.GetComponent<NextButton>();
                    if (nextButton != null)
                        buttons.Add(buttonGameObject);
                    break;
                case QuestionSegmentAttributes.ButtonType.PreviousAndNext:
                    var previousButtonGO = buttonGameObjects[1].GetComponent<PreviousButton>();
                    var nextButtonGO = buttonGameObjects[2].GetComponent<NextButton>();
                    if (previousButtonGO != null)
                        buttons.Add(buttonGameObjects[1]);
                    if (nextButtonGO != null)
                        buttons.Add(buttonGameObjects[2]);
                    break;
                case QuestionSegmentAttributes.ButtonType.PreviousAndEnd:
                    var previousButtonGO2 = buttonGameObjects[1].GetComponent<PreviousButton>();
                    var finishButton2 = buttonGameObject.GetComponent<FinishButton>();
                    if (previousButtonGO2 != null)
                        buttons.Add(buttonGameObjects[1]);
                    if (finishButton2 != null)
                        buttons.Add(buttonGameObject);
                    break;

                case QuestionSegmentAttributes.ButtonType.End:
                    var finishButton = buttonGameObject.GetComponent<FinishButton>();
                    if (finishButton != null)
                        buttons.Add(buttonGameObject);
                    break;
            }
        }

        return buttons;
    }
}
