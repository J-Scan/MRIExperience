using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ManualRealigner : MonoBehaviour
{
    [SerializeField] private Transform origin; // L'origine à ajuster
    [SerializeField] private float movementSpeed = 0.1f; // Vitesse de déplacement
    [SerializeField] private float rotationSpeed = 45f;  // Vitesse de rotation en degrés par seconde

    private InputDevice leftController;
    private InputDevice rightController;
    private bool leftInitialized = false;
    private bool rightInitialized = false;

    private void Start()
    {
        InitializeDevices();
    }

    private void Update()
    {
        if (!leftInitialized || !rightInitialized)
        {
            InitializeDevices();
        }

        HandleInput();
    }

    private void InitializeDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0)
        {
            leftController = devices[0];
            leftInitialized = true;
        }

        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
        {
            rightController = devices[0];
            rightInitialized = true;
        }
    }

    private void HandleInput()
    {
        if (!leftInitialized || !rightInitialized) return;

        // Déplacement avec le joystick gauche
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput))
        {
            if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerPressed) && isLeftTriggerPressed)
            {
                // Déplacement en avant/arrière/gauche/droite (mode local)
                Vector3 moveDirection = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y) * movementSpeed * Time.deltaTime;
                origin.position += origin.TransformDirection(moveDirection);
            }
            else
            {
                // Déplacement en haut/bas/gauche/droite
                Vector3 moveDirection = new Vector3(leftJoystickInput.x, leftJoystickInput.y, 0) * movementSpeed * Time.deltaTime;
                origin.position += moveDirection;
            }
        }

        // Rotation avec le joystick droit
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickInput))
        {
            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerPressed) && isRightTriggerPressed)
            {
                // Rotation rapide (enfoncé)
                float rotation = rightJoystickInput.x * rotationSpeed * 2 * Time.deltaTime;
                origin.Rotate(Vector3.up, rotation);
            }
            else
            {
                // Rotation normale
                float rotation = rightJoystickInput.x * rotationSpeed * Time.deltaTime;
                origin.Rotate(Vector3.up, rotation);
            }
        }
    }
}
