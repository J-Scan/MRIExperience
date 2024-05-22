using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTransition : MonoBehaviour
{
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private Transform[] locations;
    private int locationIndex = 0;

    public void Start()
    {
        GoToFirstLocation();
    }

    public void GoToFirstLocation()
    {
        XRRig.transform.position = locations[locationIndex % locations.Length].position;
        XRRig.transform.rotation = locations[locationIndex % locations.Length].rotation;
        locationIndex++;
    }

    public void GoToNextLocation()
    {
        Debug.Log("GoToNextLocation called");
        StartCoroutine(GoToLocation(locations[locationIndex % locations.Length]));
        locationIndex++;
    }

    public IEnumerator GoToLocation(Transform newLocation)
    {
        Debug.Log("GoToLocation called");
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        XRRig.transform.position = newLocation.position;
        XRRig.transform.rotation = newLocation.rotation;
        Debug.Log("swtiched");
        fadeScreen.FadeIn();
    }
}