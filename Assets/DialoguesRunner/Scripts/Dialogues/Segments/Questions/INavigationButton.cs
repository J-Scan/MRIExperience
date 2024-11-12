using UnityEngine;

public interface INavigationButton
{
    void PerformAction(DialoguesRunner dialoguesRunner);
    void SetActive(bool value);
}