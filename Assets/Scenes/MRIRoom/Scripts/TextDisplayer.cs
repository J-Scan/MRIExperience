using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Segment
{
    public string title;
    [TextArea(3, 10)]
    public string shortStatement;
    [TextArea(6, 10)]
    public string longStatement;
    public GameObject buttonSet;
}

public class TextDisplayer : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI shortStatementText;
    public TextMeshProUGUI longStatementText;
    private GameObject currrentButtonSet = null;

    [Header("Content Array")]
    public Segment[] segments;

    private int currentIndex = 0;

    public void InitText()
    {
        currentIndex = 0;
        DisableGO();
        UpdateSegment();
    }

    public void NextSegment()
    {
        if (currentIndex < segments.Length - 1)
        {
            DisableGO();
            currentIndex++;
            UpdateSegment();
        }
        else
        {
            Debug.Log("Reached the end of segments.");
        }
    }

    public void DisableGO()
    {
        titleText.gameObject.SetActive(false);
        shortStatementText.gameObject.SetActive(false);
        longStatementText.gameObject.SetActive(false);
        if(currrentButtonSet != null)
        {
            currrentButtonSet.SetActive(false);
        }
    }

    private void UpdateSegment()
    {
        // Accéder aux informations du segment courant
        Segment currentSegment = segments[currentIndex];
        titleText.text = currentSegment.title;
        // Mise à jour du titre et des déclarations
        if (currentSegment.title != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = currentSegment.title;
        }
        if (currentSegment.shortStatement != null)
        {
            shortStatementText.gameObject.SetActive(true);
            shortStatementText.text = currentSegment.shortStatement;
        }
        if (currentSegment.longStatement != null)
        {
            longStatementText.gameObject.SetActive(true);
            longStatementText.text = currentSegment.longStatement;
        }
        if (currentSegment.buttonSet != null)
        {
            currrentButtonSet = currentSegment.buttonSet;
            currrentButtonSet.gameObject.SetActive(true);
        }

    }
}
