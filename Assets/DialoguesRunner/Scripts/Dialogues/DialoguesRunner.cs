using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

[Serializable]
public class DialogueManagerConfig
{
    public GameObject managerGameObject;
    public RunMode runMode;
    public bool autoAdvance;
}

public enum RunMode
{
    Parallel,
    Sequential
}

[Serializable]
public class DialogueManagerGroup
{
    public List<DialogueManagerConfig> managerConfigs;
}

public class DialoguesRunner : MonoBehaviour
{
    [SerializeField] private List<DialogueManagerGroup> managerGroups;

    // UnityEvent to trigger after the last manager has finished
    [SerializeField] private UnityEvent onAllManagersFinished;

    private List<IDialogue> activeManagers;
    private int currentGroupIndex = 0;
    private int currentSequentialManagerIndex = 0;
    private bool isInitialized = false;

    void Start()
    {
        activeManagers = new List<IDialogue>();
        StartCoroutine(StartManagersWithDelay(3f));
    }

    private IEnumerator StartManagersWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCurrentGroupManagers();
        isInitialized = true;
    }

    private void StartCurrentGroupManagers()
    {
        if (currentGroupIndex >= managerGroups.Count) return;

        var currentGroup = managerGroups[currentGroupIndex];

        foreach (var config in currentGroup.managerConfigs)
        {
            var dialogueManager = config.managerGameObject.GetComponent<IDialogue>();
            if (dialogueManager != null)
            {
                if (config.runMode == RunMode.Parallel)
                {
                    ActivateManager(dialogueManager, config.managerGameObject);
                    activeManagers.Add(dialogueManager);
                }
                else if (config.runMode == RunMode.Sequential && activeManagers.Count == 0)
                {
                    ActivateManager(dialogueManager, config.managerGameObject);
                    activeManagers.Add(dialogueManager);
                    currentSequentialManagerIndex++;
                }
            }
        }
    }

    void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        UpdateActiveManagers();

        if (currentGroupIndex < managerGroups.Count && activeManagers.Count == 0)
        {
            currentGroupIndex++;
            currentSequentialManagerIndex = 0;
            StartCurrentGroupManagers();
        }
    }

    private void ActivateManager(IDialogue manager, GameObject managerGameObject)
    {
        if (manager == null) return;
        managerGameObject.SetActive(true);
    }

    private void UpdateActiveManagers()
    {
        bool allSegmentsFinished = true;
        bool allManagersFinished = true;

        for (int i = activeManagers.Count - 1; i >= 0; i--)
        {
            var manager = activeManagers[i];
            if (!manager.IsSegmentFinished())
            {
                allSegmentsFinished = false;
            }

            if (!manager.HasFinished())
            {
                allManagersFinished = false;
                manager.Update();
            }
        }

        bool autoAdvance = activeManagers
            .Select(manager => managerGroups
                .SelectMany(group => group.managerConfigs)
                .FirstOrDefault(config => config.managerGameObject.GetComponent<IDialogue>() == manager))
            .All(config => config != null && config.autoAdvance);

        if (allSegmentsFinished && !allManagersFinished && autoAdvance)
        {
            AdvanceAllManagers();
        }

        if (allManagersFinished)
        {
            for (int i = activeManagers.Count - 1; i >= 0; i--)
            {
                var manager = activeManagers[i];
                manager.OnFinish();
                var managerGameObject = managerGroups
                    .SelectMany(group => group.managerConfigs)
                    .FirstOrDefault(config => config.managerGameObject.GetComponent<IDialogue>() == manager).managerGameObject;
                managerGameObject.SetActive(false);
                activeManagers.RemoveAt(i);
            }

            // If all managers in all groups have finished, trigger the event
            if (currentGroupIndex >= managerGroups.Count)
            {
                OnAllManagersFinished();
            }
        }
    }

    public void AdvanceAllManagers()
    {
        foreach (var manager in activeManagers)
        {
            Debug.Log("Advancing manager: " + manager);
            manager.GoToNextSegment();
        }
    }

    // New function to invoke the UnityEvent when all managers are finished
    private void OnAllManagersFinished()
    {
        Debug.Log("All managers finished. Triggering final event.");
        if (onAllManagersFinished != null)
        {
            onAllManagersFinished.Invoke();
        }
    }

    public T GetActiveManager<T>() where T : class, IDialogue
    {
        foreach (var manager in activeManagers)
        {
            if (manager is T)
            {
                return manager as T;
            }
        }
        return null;
    }
}
