using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControllerInputDetector : MonoBehaviour
{
    UnityEngine.XR.InputDevice left;
    UnityEngine.XR.InputDevice right;
    bool leftInitialized = false;
    bool rightInitialized = false;

    private float holdTimeThreshold = 2.0f;
    private float buttonHoldTimeL = 0.0f;
    private float buttonHoldTimeR = 0.0f;

    //[SerializeField] private InputActionReference leftJoystickAction;
    //[SerializeField] private InputActionReference leftJoystickActionSimulator;

    [SerializeField] UnityEvent OnPrimaryButtonPressed;
    [SerializeField] UnityEvent OnTriggerButtonHeld;

    void Start()
    {
        //DisableLeftJoystick();
        //DisableSimulatorLeftJoystick();
        InitializeDevices();
    }

    /*
    void DisableSimulatorLeftJoystick()
    {
        if (leftJoystickActionSimulator != null)
        {
            leftJoystickActionSimulator.action.Disable();
        }
        else
        {
            Debug.LogError("Left joystick simulator action reference is not set.");
        }
    }

    void DisableLeftJoystick()
    {
        if (leftJoystickAction != null)
        {
            leftJoystickAction.action.Disable();
        }
        else
        {
            Debug.LogError("Left joystick action reference is not set.");
        }
    }
    */

    void InitializeDevices()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0)
        {
            left = devices[0];
            leftInitialized = true;
        }

        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
        {
            right = devices[0];
            rightInitialized = true;
        }
    }

    void Update()
    {
        if (!leftInitialized || !rightInitialized)
        {
            InitializeDevices();
        }

        if (rightInitialized)
        {
            right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedR);
            if (isPressedR)
            {
                //GetComponent<LocationTransition>().Recenter();
                OnPrimaryButtonPressed.Invoke();
                return;
            }

            right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedRT);
            HandleHoldInput(isPressedRT, false);
        }

        if (leftInitialized)
        {
            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedL);
            if (isPressedL)
            {
                //GetComponent<LocationTransition>().Recenter();
                OnPrimaryButtonPressed.Invoke();
                return;
            }

            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedLT);
            HandleHoldInput(isPressedLT, true);
        }
    }

    private void HandleHoldInput(bool isPressed, bool isLeft)
    {
        if (isPressed)
        {
            if (isLeft)
            {
                buttonHoldTimeL += Time.deltaTime;
                Debug.Log("Left button hold time: " + buttonHoldTimeL);

                if (buttonHoldTimeL >= holdTimeThreshold)
                {
                    OnHoldTriggered();
                    buttonHoldTimeL = 0.0f; // Reset after triggering the event
                }
            }
            else
            {
                buttonHoldTimeR += Time.deltaTime;
                Debug.Log("Right button hold time: " + buttonHoldTimeR);

                if (buttonHoldTimeR >= holdTimeThreshold)
                {
                    OnHoldTriggered();
                    buttonHoldTimeR = 0.0f; // Reset after triggering the event
                }
            }
        }
        else
        {
            // Reset hold time only if the button is released
            if (isLeft)
            {
                buttonHoldTimeL = 0.0f;
            }
            else
            {
                buttonHoldTimeR = 0.0f;
            }
        }
    }


    private void OnHoldTriggered()
    {
        Debug.Log("Trigger button held long enough, triggering action.");
        OnTriggerButtonHeld.Invoke();
    }
}