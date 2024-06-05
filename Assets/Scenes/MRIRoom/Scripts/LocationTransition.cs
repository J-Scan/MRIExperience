using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTransition : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private Transform[] locations;
    private int locationIndex = 0;

    [SerializeField] private Transform head;
    [SerializeField] private Transform origin;

    public void Start()
    {
        GoToFirstLocation();
    }

    public void Recenter()
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;
        origin.position = locations[locationIndex % locations.Length].position - offset;

        Vector3 targetForward = locations[locationIndex % locations.Length].forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        origin.RotateAround(head.position, Vector3.up, angle);
    }

    public void GoToFirstLocation()
    {
        //XRRig.transform.position = locations[locationIndex % locations.Length].position;
        //XRRig.transform.rotation = locations[locationIndex % locations.Length].rotation;

        Recenter();
    }

    public void GoToNextLocation()
    {
        locationIndex++;
        Debug.Log("GoToNextLocation called");
        StartCoroutine(GoToLocation(locations[locationIndex % locations.Length]));
    }

    public void ResetCurrentLocation()
    {
        Recenter();
    }

    public IEnumerator GoToLocation(Transform newLocation)
    {
        Debug.Log("GoToLocation called");
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //XRRig.transform.position = newLocation.position;
        //XRRig.transform.rotation = newLocation.rotation;

        Recenter();
        Debug.Log("swtiched");
        fadeScreen.FadeIn();
    }
}