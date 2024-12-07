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
    private float previousAngle = -1f;

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

    public void Recenter(Transform dest)
    {
        // Calculez l'offset horizontal (sans inclure la hauteur)
        Vector3 offset = head.position - origin.position;
        offset.y = 0;

        // Positionnez l'origine horizontalement à la destination cible
        origin.position = dest.position - offset;

        // Ajustez la hauteur dynamique pour compenser les changements (assise ou debout)
        float currentUserHeight = head.position.y; // Hauteur actuelle de la tête
        float targetHeight = dest.position.y; // Hauteur cible
        float heightAdjustment = targetHeight - currentUserHeight;

        // Appliquez l'ajustement de hauteur à l'origine
        origin.position += new Vector3(0, heightAdjustment, 0);

        // Calculez la rotation cible
        Vector3 targetForward = dest.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        // Appliquez la rotation autour de la position de la tête
        origin.RotateAround(head.position, Vector3.up, angle);
    }

    public void Recenter()
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;

        Transform target = locations[locationIndex % locations.Length];

        // Recentrer la position
        origin.position = target.position - offset;

        // Ajuster la hauteur pour aligner les niveaux
        float heightAdjustment = target.position.y - head.position.y;
        origin.position += new Vector3(0, heightAdjustment, 0);

        // Calcul des vecteurs forward
        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Correction pour les inclinaisons importantes
        float tiltAngle = Vector3.Angle(Vector3.up, head.up);

        if (tiltAngle > 80f) // Cas de bascule latérale ou arrière
        {
            Debug.Log("Inclinaison importante détectée. Correction appliquée.");

            // Calculer la direction vers l'origine pour ramener le head
            Vector3 directionToOrigin = (origin.position - head.position).normalized;
            directionToOrigin.y = 0; // Conserver uniquement le plan horizontal

            // Ajuster cameraForward pour pointer vers l'origine
            cameraForward = Vector3.ProjectOnPlane(directionToOrigin, Vector3.up).normalized;

            Debug.Log($"Correction du forward: CameraForward={cameraForward}");
        }

        // Projet sur le plan horizontal pour stabiliser
        cameraForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized;

        // Calculer l'angle entre les directions camera et cible
        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        // Vérification pour éviter les rotations inutiles
        if (Mathf.Abs(angle) < 1f || Mathf.Abs(360f - Mathf.Abs(angle)) < 1f)
        {
            Debug.Log("Angle trop petit ou proche de 360°, recentrage ignoré.");
            return;
        }

        // Correction des angles proches de ±180°
        if (Mathf.Abs(angle) > 170f)
        {
            Debug.LogWarning("Angle proche de ±180°, correction appliquée.");
            angle = Mathf.Sign(angle) * (angle - 180f);
        }

        // Vérification pour éviter de recalculer trop souvent
        if (Mathf.Abs(angle - previousAngle) < 5f)
        {
            Debug.Log("Angle similaire à l'angle précédent, recentrage ignoré.");
            return;
        }

        // Rotation autour de l'origine
        origin.RotateAround(head.position, Vector3.up, angle);

        // Mise à jour de l'angle précédent
        previousAngle = angle;

        Debug.Log($"Recentered: Angle={angle}, PreviousAngle={previousAngle}, CameraForward={cameraForward}, TargetForward={targetForward}");
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
