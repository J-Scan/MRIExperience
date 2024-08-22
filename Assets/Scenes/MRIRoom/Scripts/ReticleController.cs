using UnityEngine;

public class ReticleController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor; // The XRRayInteractor to follow

    [SerializeField]
    private GameObject crosshair; // The GameObject of the reticle

    [SerializeField]
    private Camera CameraFacing;

    private Vector3 originalScale;
    private Vector3 currentScale;
    private Vector3 lastKnownPosition;
    private float lastKnownDistance;

    void Start()
    {
        originalScale = transform.localScale;
        currentScale = originalScale;
        lastKnownPosition = transform.position;
        lastKnownDistance = CameraFacing.farClipPlane * 0.55f;
    }

    private void Update()
    {
        float distance;
        if (rayInteractor == null || crosshair == null)
        {
            Debug.LogError("RayInteractor or Crosshair is not assigned.");
            return;
        }

        // Obtain the position and direction of the ray
        Vector3 rayOrigin = rayInteractor.transform.position;
        Vector3 rayDirection = rayInteractor.transform.forward;

        // Raycast to detect collisions
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            // If a collision is detected, smoothly transition to the new position
            lastKnownPosition = hitInfo.point;
            lastKnownDistance = hitInfo.distance;
        }
        else
        {
            // Otherwise, use the last known position, extending towards the far clip plane
            lastKnownPosition = rayOrigin + rayDirection * CameraFacing.farClipPlane * 0.95f;
            lastKnownDistance = CameraFacing.farClipPlane * 0.95f;
        }

        // Non-linear scaling factor to reduce "slipping" effect
        if (lastKnownDistance < 10)
        {
            lastKnownDistance *= 1 + 5 * Mathf.Exp(-lastKnownDistance);
        }

        // Smoothly adjust the scale to reduce abrupt changes
        float smoothFactor = 0.1f; // Adjust the smoothness as needed
        currentScale = Vector3.Lerp(currentScale, originalScale * lastKnownDistance, smoothFactor);
        transform.localScale = currentScale;

        // Smoothly transition to the new reticle position
        transform.position = Vector3.Lerp(transform.position, lastKnownPosition, smoothFactor);

        // Ensure the reticle is oriented towards the camera for consistent visibility
        transform.LookAt(CameraFacing.transform.position);
        transform.Rotate(0f, 180f, 0f);
    }
}
