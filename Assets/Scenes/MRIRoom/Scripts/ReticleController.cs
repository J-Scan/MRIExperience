using UnityEngine;

public class ReticleController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor; // The XRRayInteractor to follow

    [SerializeField]
    private GameObject crosshair; // The GameObject of the reticle

    [SerializeField]
    private float maxDistance = 10f; // Maximum distance of the ray

    [SerializeField]
    private LayerMask collisionMask; // Layer mask to filter collisions

    [SerializeField]
    private Camera CameraFacing;

    private Vector3 originalScale;

    //private float fixedDistance = 2f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    /*
        private void Update()
        {
            if (rayInteractor == null || crosshair == null)
            {
                Debug.LogError("RayInteractor or Crosshair is not assigned.");
                return;
            }

            // Positionne le réticule à une distance fixe devant la caméra
            Vector3 fixedPosition = CameraFacing.transform.position + CameraFacing.transform.forward * fixedDistance;

            // Positionne et oriente le réticule vers la caméra
            transform.position = fixedPosition;
            transform.LookAt(CameraFacing.transform.position);
            transform.Rotate(0f, 180f, 0f);

            // Conserve la taille d'origine du réticule
            transform.localScale = originalScale;
        }
    */

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
        //if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, maxDistance, collisionMask))
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo))
        {
            // If a collision is detected, position the reticle at the collision point
            //crosshair.transform.position = hitInfo.point;

            distance = hitInfo.distance;
        }
        else
        {
            // Otherwise, position the reticle at the maximum distance of the ray
            //crosshair.transform.position = rayOrigin + rayDirection * maxDistance;
            distance = CameraFacing.farClipPlane * 0.55f;
        }

        // Ensure the reticle is oriented towards the camera for consistent visibility
        //crosshair.transform.rotation = Quaternion.LookRotation(rayDirection, Vector3.up);

        transform.position = CameraFacing.transform.position + 
            CameraFacing.transform.rotation * Vector3.forward * distance;
        transform.LookAt(CameraFacing.transform.position);
        transform.Rotate(0f, 180f, 0f);
        //
        //if (distance < 10)
        //{
        //    distance *= 1 + 5*Mathf.Exp(-distance);
        //}
        
        transform.localScale = originalScale * distance;
    }

}
