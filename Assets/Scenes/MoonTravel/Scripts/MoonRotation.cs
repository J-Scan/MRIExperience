using UnityEngine;

public class MoonRotation : MonoBehaviour
{
    public Transform earth;

    [SerializeField] private float orbitSpeed = 10.0f;

    [SerializeField] private float selfRotationSpeed = 5.0f;

    [SerializeField] private float orbitRadius = 5.0f;

    private Vector3 orbitAxis = Vector3.up;
    private Vector3 selfRotationAxis = Vector3.up;

    void Start()
    {
        if (earth == null)
        {
            Debug.LogError("La Terre (ou le centre de l'orbite) n'est pas assignée.");
            return;
        }

        transform.position = earth.position + (transform.position - earth.position).normalized * orbitRadius;
    }

    void Update()
    {
        transform.RotateAround(earth.position, orbitAxis, orbitSpeed * Time.deltaTime);

        transform.Rotate(selfRotationAxis, selfRotationSpeed * Time.deltaTime);
    }
}
