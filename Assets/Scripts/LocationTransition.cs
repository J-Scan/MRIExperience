using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTransition : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public GameObject XRRig;
    public Transform[] locations;
    public int locationIndex = 0;

    private void Start()
    {
        //Invoke("SetNextLocation", 5);
    }

    public void SetNextLocation()
    {
        Debug.Log("SetNextLocation called");
        locationIndex++;
        XRRig.transform.position = locations[locationIndex % locations.Length].position;
        XRRig.transform.rotation = locations[locationIndex % locations.Length].rotation;
        //GoToLocation(locations[locationIndex]);
        //locationIndex++;
    }


    IEnumerator GoToLocation(Transform newLocation)
    {
        Debug.Log("GoToLocation called");
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //Set to new location
        XRRig.transform.position = newLocation.position;
        XRRig.transform.rotation = newLocation.rotation;
        Debug.Log("swtiched");
    }
}
