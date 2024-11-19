using UnityEngine;
using UnityEngine.Events;

public class ConfirmLookDirection : MonoBehaviour
{
    [SerializeField] private Transform vrCamera; // Cam�ra du joueur (VR ou non)
    [SerializeField] private Vector3 targetDirection = Vector3.up; // Direction cible (par d�faut : vers le haut)
    [SerializeField] private float angleThreshold = 15f; // Angle d'acceptation en degr�s
    [SerializeField] private UnityEvent onLookConfirmed; // �v�nement d�clench� si l'utilisateur regarde dans la bonne direction
    [SerializeField] private UnityEvent onLookUnconfirmed;

    void Start()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR camera is not assigned!");
        }
    }

    void Update()
    {
        if (vrCamera == null) return;

        float angle = Vector3.Angle(vrCamera.forward, targetDirection);

        if (angle <= angleThreshold)
        {
            onLookConfirmed?.Invoke();
        }
        else
        {
            onLookUnconfirmed?.Invoke();
        }
    }

    /// <summary>
    /// Met � jour dynamiquement la direction cible et l'angle d'acceptation.
    /// </summary>
    /// <param name="newDirection">Nouvelle direction cible</param>
    /// <param name="newThreshold">Nouveau seuil d'angle</param>
    public void SetTargetDirection(Vector3 newDirection, float newThreshold = 15f)
    {
        targetDirection = newDirection;
        angleThreshold = newThreshold;
    }
}
