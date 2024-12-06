using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignCanvasWithView : MonoBehaviour
{
    [SerializeField] private Transform vrCamera; // La cam�ra ou le casque VR
    [SerializeField] private Transform canvasTransform; // Le Transform du Canvas � aligner
    [SerializeField] private float smoothSpeed = 5f; // Vitesse de glissement
    [SerializeField] private float distanceFromCamera = 2f; // Distance du Canvas par rapport � la cam�ra
    [SerializeField] private Vector3 offset = Vector3.zero; // D�calage personnalis� (optionnel)

    private Vector3 targetPosition;

    void Start()
    {
        if (vrCamera == null)
        {
            Debug.LogError("La cam�ra VR n'est pas assign�e !");
        }
        if (canvasTransform == null)
        {
            Debug.LogError("Le Transform du Canvas n'est pas assign� !");
        }
    }

    void Update()
    {
        if (vrCamera == null || canvasTransform == null) return;

        // Calculer la position cible devant la cam�ra
        targetPosition = vrCamera.position + vrCamera.forward * distanceFromCamera + offset;

        // Interpolation lin�aire pour d�placer le Canvas
        canvasTransform.position = Vector3.Lerp(canvasTransform.position, targetPosition, Time.unscaledDeltaTime * smoothSpeed);

        // Faire face � la cam�ra avec interpolation
        Quaternion targetRotation = Quaternion.LookRotation(canvasTransform.position - vrCamera.position);
        canvasTransform.rotation = Quaternion.Slerp(canvasTransform.rotation, targetRotation, Time.unscaledDeltaTime * smoothSpeed);
    }
}
