using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.Events;

public class MenuPauser : MonoBehaviour
{

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] UnityEvent OnEnterPause;
    [SerializeField] UnityEvent OnFinishPause;
    private bool inPause = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformPauseAction()
    {
        if (inPause)
        {
            ExitPause();
        }
        else
        {
            StartPause();
        }
    }

    public void ExitPause()
    {
        DesactivateMenuInFront();
        UnpauseGame();
        OnFinishPause.Invoke();
        inPause = false;
    }

    public void DesactivateMenuInFront()
    {
        menuCanvas.SetActive(false);
    }

    public void StartPause()
    {
        DisplayMenuInFront();
        PauseGame();
        inPause = true;
    }

    public void DisplayMenuInFront()
    {
        OnEnterPause.Invoke();
        Vector3 vHeadPos = Camera.main.transform.position;
        Vector3 vGazeDir = Camera.main.transform.forward;
        menuCanvas.transform.position = (vHeadPos + vGazeDir * 3.0f) + new Vector3(0.0f, -.40f, 0.0f);
        Vector3 vRot = Camera.main.transform.eulerAngles; vRot.z = 0;
        menuCanvas.transform.eulerAngles = vRot;
        menuCanvas.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        //And you should also cancel any input to the game.
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        //And you should also cancel any input to the game.
    }
}
