using UnityEngine;

public interface IDialogueSegment {
    void Advance();
    void Retreat();
    bool HasFinished();
}