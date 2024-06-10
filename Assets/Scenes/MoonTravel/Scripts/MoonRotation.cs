using UnityEngine;

public class MoonRotation : MonoBehaviour
{
    // La Terre ou le centre de l'orbite de la Lune
    public Transform earth;

    // Vitesse de rotation autour de la Terre
    public float orbitSpeed = 10.0f;

    // Vitesse de rotation sur elle-même
    public float selfRotationSpeed = 5.0f;

    // Distance de la Lune par rapport à la Terre
    public float orbitRadius = 5.0f;

    private Vector3 orbitAxis = Vector3.up; // Axe d'orbite (autour de l'axe Y par défaut)
    private Vector3 selfRotationAxis = Vector3.up; // Axe de rotation propre (autour de l'axe Y par défaut)

    void Start()
    {
        if (earth == null)
        {
            Debug.LogError("La Terre (ou le centre de l'orbite) n'est pas assignée.");
            return;
        }

        // Placer la Lune à la bonne distance de la Terre
        transform.position = earth.position + (transform.position - earth.position).normalized * orbitRadius;
    }

    void Update()
    {
        // Faire tourner la Lune autour de la Terre
        transform.RotateAround(earth.position, orbitAxis, orbitSpeed * Time.deltaTime);

        // Faire tourner la Lune sur elle-même
        transform.Rotate(selfRotationAxis, selfRotationSpeed * Time.deltaTime);
    }
}
