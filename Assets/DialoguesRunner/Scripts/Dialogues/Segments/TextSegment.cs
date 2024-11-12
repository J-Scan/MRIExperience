using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextSegment : MonoBehaviour, IDialogueSegment
{
    private string NPCname;
    private string[] textToWriteArray;
    private TextMeshProUGUI uiText;

    private int segmentIndex;
    private int characterIndex;
    private float timePerCharacter;
    private float timer;
    private bool invisibleCharacters;
    private bool isFinished = false;
    private int maxCharactersPerPage = 150;

    private bool isPaused = false;  // For handling pauses between sentences
    private float pauseDuration = 2f;  // Duration of the pause after punctuation
    private float pauseTimer = 0f;  // Timer to track the pause

    public void InitializeSegment(TextMeshProUGUI uiText, string textToWrite, float timePerCharacter, string NPCname = "", bool invisibleCharacters = true)
    {
        this.uiText = uiText;
        this.timePerCharacter = timePerCharacter;
        this.NPCname = NPCname;
        this.invisibleCharacters = invisibleCharacters;
        SplitTextIntoSubSegments(textToWrite);
        segmentIndex = 0;
        characterIndex = 0;
    }

    public void SplitTextIntoSubSegments(string text)
    {
        char[] punctuationMarks = { '.', '!', '?' };

        List<string> segments = new List<string>();
        int startIndex = 0;
        while (startIndex < text.Length)
        {
            int length = Mathf.Min(maxCharactersPerPage, text.Length - startIndex);
            int nextLength = length;

            // Ensure we don't cut words by searching for a space or punctuation before max length
            int spaceIndex = FindLastSpaceOrPunctuation(text, startIndex, length);
            if (spaceIndex != -1)
            {
                nextLength = spaceIndex - startIndex + 1;
            }

            segments.Add(text.Substring(startIndex, nextLength));
            startIndex += nextLength;
        }
        textToWriteArray = segments.ToArray();
    }

    // Find the last space or punctuation mark to avoid cutting words
    public int FindLastSpaceOrPunctuation(string text, int startIndex, int length)
    {
        int lastIndex = -1;
        for (int i = startIndex + length - 1; i >= startIndex; i--)
        {
            if (text[i] == ' ' || text[i] == '.' || text[i] == '!' || text[i] == '?')
            {
                lastIndex = i;
                break;
            }
        }
        return lastIndex;
    }

    public void Advance()
    {
        // If the segment is paused, manage the pause timer
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;  // Pause is over, continue displaying text
            }
            return;  // Don't proceed until the pause is over
        }

        if (uiText != null && !isFinished)
        {
            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                timer += timePerCharacter;

                if (segmentIndex >= textToWriteArray.Length)
                {
                    SetIsFinished(true);
                    return;
                }

                string currentSubSegment = textToWriteArray[segmentIndex];

                if (characterIndex < currentSubSegment.Length)
                {
                    DisplayNextCharacter(currentSubSegment);

                    // Check if the current character is a punctuation mark, if so, pause
                    if (IsPunctuation(currentSubSegment[characterIndex - 1]))
                    {
                        isPaused = true;
                        pauseTimer = pauseDuration;  // Set the pause duration to 2 seconds
                    }
                }
                else
                {
                    MoveToNextSubSegment();
                }
            }
        }
    }

    // Check if the character is a punctuation mark that should trigger a pause
    public bool IsPunctuation(char character)
    {
        return character == '.' || character == '!' || character == '?';
    }

    public void DisplayNextCharacter(string currentSubSegment)
    {
        string text = currentSubSegment.Substring(0, characterIndex + 1);

        if (invisibleCharacters && characterIndex < currentSubSegment.Length - 1)
        {
            text += "<color=#00000000>" + currentSubSegment.Substring(characterIndex + 1) + "</color>";
        }

        uiText.text = text;
        characterIndex++;
    }

    public void MoveToNextSubSegment()
    {
        segmentIndex++;
        characterIndex = 0;
    }

    public void Update()
    {
        Advance();
    }

    public void Retreat()
    {
        // Implement functionality if needed
    }

    public void SetIsFinished(bool isFinished)
    {
        this.isFinished = isFinished;
    }

    public bool HasFinished()
    {
        return isFinished;
    }
}
