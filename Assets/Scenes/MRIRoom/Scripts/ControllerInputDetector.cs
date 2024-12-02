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
    private bool holdingDetected = false;

    //[SerializeField] private InputActionReference leftJoystickAction;
    //[SerializeField] private InputActionReference leftJoystickActionSimulator;

    private bool performManualRealignment = false; // Toggles manual realignment mode
    [SerializeField] private Transform origin; // Transform to adjust
    [SerializeField] private float movementSpeed = 0.1f; // Movement speed
    [SerializeField] private float rotationSpeed = 45f;  // Rotation speed

    [SerializeField] UnityEvent OnPrimaryButtonPressed;
    [SerializeField] UnityEvent OnTriggerButtonHeld;
    [SerializeField] UnityEvent<Vector2, Vector2> OnManualAlignmentWithJoysticks;

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

    public void SetManualRealignement(bool value)
    {
        this.performManualRealignment = value;
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

        if (performManualRealignment)
        {
            HandleManualRealignmentInput();
        }
        
        HandleDefaultInput();
        
    }

    private void HandleDefaultInput()
    {
        if (rightInitialized)
        {
            right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedR);
            if (isPressedR)
            {
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
                OnPrimaryButtonPressed.Invoke();
                return;
            }

            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedLT);
            HandleHoldInput(isPressedLT, true);
        }
    }

    private void HandleManualRealignmentInput()
    {
        Vector2 leftJoystickInput = Vector2.zero;
        Vector2 rightJoystickInput = Vector2.zero;

        // Obtenez les valeurs des joysticks
        if (leftInitialized && left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 leftInput))
        {
            if (leftInput.magnitude > 0.1f)
            {
                leftJoystickInput = leftInput;
            }
        }

        if (rightInitialized && right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 rightInput))
        {
            if (rightInput.magnitude > 0.1f)
            {
                rightJoystickInput = rightInput;
            }
        }

        // Invoquez l'événement avec les données des joysticks
        if (leftJoystickInput != Vector2.zero || rightJoystickInput != Vector2.zero)
        {
            OnManualAlignmentWithJoysticks.Invoke(leftJoystickInput, rightJoystickInput);
        }
    }


    private void HandleHoldInput(bool isPressed, bool isLeft)
    {
        if (isPressed && !holdingDetected)
        {
            if (isLeft)
            {
                buttonHoldTimeL += Time.unscaledDeltaTime;
                //Debug.Log("Left button hold time: " + buttonHoldTimeL);

                if (buttonHoldTimeL >= holdTimeThreshold)
                {
                    OnHoldTriggered();
                    holdingDetected = true;
                    buttonHoldTimeL = 0.0f;
                }
            }
            else
            {
                buttonHoldTimeR += Time.unscaledDeltaTime;
                //Debug.Log("Right button hold time: " + buttonHoldTimeR);

                if (buttonHoldTimeR >= holdTimeThreshold)
                {
                    OnHoldTriggered();
                    holdingDetected = true;
                    buttonHoldTimeR = 0.0f;
                }
            }
        }
        else if (!isPressed)
        {
            if (isLeft)
            {
                buttonHoldTimeL = 0.0f;
            }
            else
            {
                buttonHoldTimeR = 0.0f;
            }
            holdingDetected = false;
        }
    }




    private void OnHoldTriggered()
    {
        OnTriggerButtonHeld.Invoke();
        holdingDetected = true;
    }
}