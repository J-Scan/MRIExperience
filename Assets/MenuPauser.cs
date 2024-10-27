using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.Events;

public class MenuPauser : MonoBehaviour
{

    [SerializeField] private GameObject menuCanvas;
    [SerializeField] UnityEvent OnFinishPlayback;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPause()
    {
        DisplayMenuInFront();
        PauseGame();
        OnFinishPlayback.Invoke();
    }

    public void DisplayMenuInFront()
    {
        Vector3 vHeadPos = Camera.main.transform.position;
        Vector3 vGazeDir = Camera.main.transform.forward;
        menuCanvas.transform.position = (vHeadPos + vGazeDir * 3.0f) + new Vector3(0.0f, -.40f, 0.0f);
        Vector3 vRot = Camera.main.transform.eulerAngles; vRot.z = 0;
        menuCanvas.transform.eulerAngles = vRot;
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        //And you should also cancel any input to the game.
    }
}
