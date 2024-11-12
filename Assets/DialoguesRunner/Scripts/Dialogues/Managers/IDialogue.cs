using UnityEngine;

public interface IDialogue {
    void Start();

    void Update();

    bool IsSegmentFinished();

    void GoToNextSegment();

    void GoToPreviousSegment();

    bool HasFinished();

    void OnFinish();
}
