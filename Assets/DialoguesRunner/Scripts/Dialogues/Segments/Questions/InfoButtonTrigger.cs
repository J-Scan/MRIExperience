using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonTrigger : MonoBehaviour
{
    [SerializeField] GameObject infoCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState()
    {
        infoCanvas.SetActive(!infoCanvas.activeSelf);
    }
}
