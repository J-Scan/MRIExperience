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
        // Calculez l'offset horizontal (sans inclure la hauteur)
        Vector3 offset = head.position - origin.position;
        offset.y = 0;

        // Positionnez l'origine horizontalement à la destination cible
        origin.position = locations[locationIndex % locations.Length].position - offset;

        // Ajustez la hauteur dynamique pour compenser les changements (assise ou debout)
        float currentUserHeight = head.position.y; // Hauteur actuelle de la tête
        float targetHeight = locations[locationIndex % locations.Length].position.y; // Hauteur cible
        float heightAdjustment = targetHeight - currentUserHeight;

        // Appliquez l'ajustement de hauteur à l'origine
        origin.position += new Vector3(0, heightAdjustment, 0);

        // Calculez la rotation cible
        Vector3 targetForward = locations[locationIndex % locations.Length].forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        // Appliquez la rotation autour de la position de la tête
        origin.RotateAround(head.position, Vector3.up, angle);
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
