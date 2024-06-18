using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInputDetector : MonoBehaviour
{
    InputDevice left;
    InputDevice right;
    bool leftInitialized = false;
    bool rightInitialized = false;

    void Start()
    {
        InitializeDevices();
    }

    void InitializeDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();

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
            right.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressedR);
            if (isPressedR)
            {
                GetComponent<LocationTransition>().Recenter();
                return;
            }
        }

        if (leftInitialized)
        {
            left.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressedL);
            if (isPressedL)
            {
                GetComponent<LocationTransition>().Recenter();
            }
        }
    }
}