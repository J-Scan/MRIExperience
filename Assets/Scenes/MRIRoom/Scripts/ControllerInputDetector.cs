using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControllerInputDetector : MonoBehaviour
{
    public UnityEngine.XR.InputDevice left;
    public UnityEngine.XR.InputDevice right;

    public bool leftInitialized = false;
    public bool rightInitialized = false;

    private float holdTimeThreshold = 2.0f;
    private float buttonHoldTimeL = 0.0f;
    private float buttonHoldTimeR = 0.0f;
    private bool holdingDetected = false;

    private bool holdingEnabled = true;

    private bool wasPrimaryButtonPressedR = false;
    private bool wasPrimaryButtonPressedL = false;

    [SerializeField] UnityEvent OnPrimaryButtonPressed;
    [SerializeField] UnityEvent OnTriggerButtonHeld;

    public void SetHoldingEnabled(bool enabled)
    {
        holdingEnabled = enabled;
    }


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
        
        HandleDefaultInput();
        
    }
    private void HandleDefaultInput()
    {
        // Vérifiez les entrées du contrôleur droit
        if (rightInitialized)
        {
            right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedR);

            // Détecte une transition de "relâché" à "pressé" pour le bouton droit
            if (isPressedR && !wasPrimaryButtonPressedR)
            {
                OnPrimaryButtonPressed.Invoke(); // Appelle l'événement une seule fois
            }
            wasPrimaryButtonPressedR = isPressedR; // Met à jour l'état précédent

            right.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedRT);
            HandleHoldInput(isPressedRT, false);
        }

        // Vérifiez les entrées du contrôleur gauche
        if (leftInitialized)
        {
            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool isPressedL);

            // Détecte une transition de "relâché" à "pressé" pour le bouton gauche
            if (isPressedL && !wasPrimaryButtonPressedL)
            {
                OnPrimaryButtonPressed.Invoke(); // Appelle l'événement une seule fois
            }
            wasPrimaryButtonPressedL = isPressedL; // Met à jour l'état précédent

            left.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedLT);
            HandleHoldInput(isPressedLT, true);
        }
    }


    private void HandleHoldInput(bool isPressed, bool isLeft)
    {
        if (!holdingEnabled)
        {
            buttonHoldTimeL = 0.0f;
            buttonHoldTimeR = 0.0f;
            return;
        }

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