using Unity.VisualScripting;
using UnityEngine;

public class MoveToMoon : MonoBehaviour
{
    private float durationT1 = 52.0f;
    private float durationT2 = 10.0f;
    private float endZ = -58f;

    private float startZ;
    private float speedT1;
    private float elapsedTime = 0.0f;

    private float startFOV;
    private float endFOV = 120f;
    private float speedT2;
    private float elapsedTime2 = 0.0f;

    private float distance2;

    void Start()
    {
        startZ = transform.position.z;

        float distance = endZ - startZ;
        speedT1 = distance / durationT1;

        startFOV = GetComponent<Camera>().fieldOfView;
        distance2 = endFOV - startFOV;
        speedT2 = distance2 / durationT2;

    }

    void FixedUpdate()
    {
        if (elapsedTime < durationT1)
        {
            elapsedTime += Time.deltaTime;
            float moveAmount = speedT1 * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + moveAmount);
        }
        else if(elapsedTime2 < durationT2)
        {
            elapsedTime2 += Time.deltaTime;
            //float zoomAmount = speedT2 * Time.deltaTime;
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, distance2, durationT2);
        }

        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            GetComponent<Camera>().transform.Rotate(5f * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<Camera>().transform.Rotate(-5f * Time.deltaTime, 0, 0);
        }
    }
}