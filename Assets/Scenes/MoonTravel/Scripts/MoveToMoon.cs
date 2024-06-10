using UnityEngine;

public class MoveToMoon : MonoBehaviour
{
    private float duration = 62.0f;
    private float endZ = -56f;

    private float startZ;
    private float speed;
    private float elapsedTime = 0.0f;

    void Start()
    {
        startZ = transform.position.z;

        float distance = endZ - startZ;
        speed = distance / duration;

        Debug.Log("startZ: " + startZ);
        Debug.Log("endZ: " + endZ);
        Debug.Log("speed: " + speed);
    }

    void FixedUpdate()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float moveAmount = speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + moveAmount);
        }
        else
        {
            enabled = false;
        }
    }
}
