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

    public void Awake()
    {
        fadeScreen.FadeIn();
    }

    public void Start()
    {
        StartCoroutine(GoToFirstLocation(1));
    }

    public void Recenter()
    {
        //Debug.Log("----Recenter----");
        //Debug.Log("Head position: " + head.position);
        //Debug.Log("XRRig Origin position: " + origin.position);
        Vector3 offset = head.position - origin.position;
        //Debug.Log("Offset: " + offset);
        offset.y = 0;
        //Debug.Log("New Offset: " + offset);
        origin.position = locations[locationIndex % locations.Length].position - offset;
        //Debug.Log("New XRRig Origin position: " + origin.position);

        Vector3 targetForward = locations[locationIndex % locations.Length].forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

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
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        Recenter();
        //Debug.Log("switched");
        fadeScreen.FadeIn();
    }

    public IEnumerator PerformEndLocation()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration-0.5f);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
