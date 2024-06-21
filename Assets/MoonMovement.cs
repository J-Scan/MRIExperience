using UnityEngine;

public class MoonMovement : MonoBehaviour
{
    private Vector3 startPosition;

    [SerializeField] private Transform moonEndPosition;
    private Vector3 endPosition;
    [SerializeField] private float startScale = 1f;
    [SerializeField] private float endScale = 4f;
    [SerializeField] private float duration = 62f;

    private float startTime;
    private bool isMoving = false;

    void Start()
    {
        startPosition = transform.position;
        endPosition = moonEndPosition.position;

        StartMovement();
    }

    void Update()
    {
        if (isMoving)
        {
            float timeElapsed = Time.time - startTime;
            float t = Mathf.Clamp01(timeElapsed / duration);

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            float scale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = new Vector3(scale, scale, scale);

            if (timeElapsed >= duration)
            {
                isMoving = false;
            }
        }
    }

    public void StartMovement()
    {
        transform.position = startPosition;
        transform.localScale = new Vector3(startScale, startScale, startScale);

        startTime = Time.time;
        isMoving = true;
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
}