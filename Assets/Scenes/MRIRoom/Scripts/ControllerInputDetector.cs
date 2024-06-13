using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class ControllerInputDetector : MonoBehaviour
{
    InputDevice left;
    InputDevice right;

    void Start()
    {

    }

    void Update()
    {
        right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        right.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressedR);

        left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        left.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressedL);

        if (isPressedR || isPressedL)
        {
            GetComponent<LocationTransition>().Recenter();
        }
    }
}
