using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandleCollisionDetectionWithEvents : MonoBehaviour
{

    [SerializeField] UnityEvent onCollisionEnterEvent;
    [SerializeField] UnityEvent onCollisionStayEvent;
    [SerializeField] UnityEvent onCollisionExitEvent;

    [SerializeField] Collider[] collidersToMonitor;

    void Start()
    {
        this.enabled = false;
    }

    public void EnableCollisionDetection()
    {
        this.enabled = true;
    }

    public void DisableCollisionDetection()
    {
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onCollisionEnterEvent != null)
        {
            //Debug.Log("Collision detected with: " + other.gameObject.name);
            foreach (Collider collider in collidersToMonitor)
            {
                if (other.GetComponent<Collider>() == collider)
                {
                    onCollisionEnterEvent.Invoke();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (onCollisionExitEvent != null)
        {
            foreach (Collider collider in collidersToMonitor)
            {
                if (other.GetComponent<Collider>() == collider)
                {
                    onCollisionExitEvent.Invoke();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (onCollisionStayEvent != null)
        {
            foreach (Collider collider in collidersToMonitor)
            {
                if (other.GetComponent<Collider>() == collider)
                {
                    onCollisionStayEvent.Invoke();
                }
            }
        }
    }
}
