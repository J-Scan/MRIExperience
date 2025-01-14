using System.Collections;
using UnityEngine;

public class LocationTransition : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private Transform[] locations;
    private int locationIndex = 0;

    [SerializeField] private Transform head;
    [SerializeField] private Transform origin;
    [SerializeField] private float locationTransitionDuration = 2f;

    public void Awake()
    {
        fadeScreen.FadeIn();
    }

    public void Start()
    {
        StartCoroutine(GoToFirstLocation(1));
    }

    public void HandleScannerTopCollision()
    {
        Transform newTransform = origin;
        Vector3 newPosition = newTransform.position;
        newPosition.y = -.35f;
        origin.position = newPosition;
        newTransform.position = newPosition;
        //Recenter(newTransform);
    }

    public void HandleScannerBottomCollision()
    {
        Transform newTransform = origin;
        Vector3 newPosition = newTransform.position;
        newPosition.y = 0.175f;
        origin.position = newPosition;
        newTransform.position = newPosition;
        //Recenter(newTransform);
    }

    /* Old recenter
     *    public void Recenter(Transform dest)
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;

        Transform target = dest;

        origin.position = target.position - offset;

        float heightAdjustment = target.position.y - head.position.y;
        origin.position += new Vector3(0, heightAdjustment, 0);

        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Correction for important tilts
        float tiltAngle = Vector3.Angle(Vector3.up, head.up);

        if (tiltAngle > 80f)
        {
            Vector3 directionToOrigin = (origin.position - head.position).normalized;
            directionToOrigin.y = 0;

            cameraForward = Vector3.ProjectOnPlane(directionToOrigin, Vector3.up).normalized;

        }

        cameraForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        origin.RotateAround(head.position, Vector3.up, angle);
    }

    /*
     * Recentering player so that they are aligned with a target
     * 
     * Update: Made this work even if the player tilts backwards past 90 degres
     * I first detected the head tilt by measuring the angle between the normal vertical direction (Vector3.up) and the orientation of the head (head.up). 
     * When the player is tilted more than 80 degrees, I calculate a direction vector from the center of the XR Origin to the head and then "flatten"
     * it to keep it level with the ground (project it onto the horizontal plane). 
     * This helps the system determine which way the player is facing and ensures proper alignment before recentering..
     * 
     * Update:
     * Changed everything, now the world rotates to be in front of the camera
     * 
     */
    public void Recenter(Transform dest)
    {
        // Calculer le décalage entre le joueur et l'origine
        Vector3 offset = head.position - origin.position;
        offset.y = 0;

        // Appliquer le décalage pour que le joueur soit aligné avec le point cible
        Vector3 newOriginPosition = dest.position - offset;
        newOriginPosition.y = origin.position.y; // Garder la hauteur initiale de l'origine
        float heightAdjustment = dest.position.y - head.position.y;
        newOriginPosition.y += heightAdjustment;

        // Ajuster la rotation pour aligner le joueur avec le target
        Vector3 targetForward = dest.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Calculer l'angle de rotation nécessaire
        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        // Réaligner l'environnement complet
        Transform environment = origin; // Assumer que "origin" est le parent de tout l'environnement
        environment.position = newOriginPosition;
        environment.RotateAround(head.position, Vector3.up, angle);

        // Gestion spécifique pour les inclinaisons > 80 degrés
        float tiltAngle = Vector3.Angle(Vector3.up, head.up);
        if (tiltAngle > 80f)
        {
            // Réinitialiser complètement la position et l'orientation de l'environnement
            environment.position = -head.position;
            environment.Rotate(Vector3.up, -angle);

            // Alignement final avec le target
            Vector3 directionToTarget = dest.position - head.position;
            //directionToTarget.y = 0;
            environment.position += directionToTarget;
        }

    }

    public void Recenter()
    {
        Transform target = locations[locationIndex % locations.Length];
        Recenter(target);
    }

    public IEnumerator GoToFirstLocation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Recenter();
    }

    public void IncrementLocation()
    {
        locationIndex++;
    }

    public void SetLocationIndex(int index)
    {
        locationIndex = index;
    }

    public void GoToSpecificLocation(int index)
    {
        locationIndex = index;
        StartCoroutine(GoToLocation(locations[locationIndex % locations.Length]));
    }

    public void GoToNextLocation()
    {
        locationIndex++;
        //Debug.Log("GoToNextLocation called");
        StartCoroutine(GoToLocation(locations[locationIndex % locations.Length]));
    }

    public void ResetCurrentLocation()
    {
        Recenter();
    }

    public void EndLocation()
    {
        StartCoroutine(PerformEndLocation());
    }

    public IEnumerator GoToLocation(Transform newLocation)
    {
        //Debug.Log("GoToLocation called");
        fadeScreen.SetFadeRation(locationTransitionDuration);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(locationTransitionDuration);

        Recenter();
        //Debug.Log("switched");
        fadeScreen.FadeIn();
    }

    public IEnumerator PerformEndLocation()
    {
        fadeScreen.SetFadeRation(locationTransitionDuration);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.GetFadeDuration()-0.5f);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
