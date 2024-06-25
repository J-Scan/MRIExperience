using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ControllerInputDetector : MonoBehaviour
{
    UnityEngine.XR.InputDevice left;
    UnityEngine.XR.InputDevice right;
    bool leftInitialized = false;
    bool rightInitialized = false;
    [SerializeField] private InputActionReference leftJoystickAction;
    [SerializeField] private InputActionReference leftJoystickActionSimulator;

    void Start()
    {
        DisableLeftJoystick();
        DisableSimulatorLeftJoystick();
        InitializeDevices();
    }

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
                GetComponent<LocationTransition>().Recenter();
                return;
            }
        }

        if (leftInitialized)
        {
            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedL);
            if (isPressedL)
            {
                GetComponent<LocationTransition>().Recenter();
            }
        }
    }
}