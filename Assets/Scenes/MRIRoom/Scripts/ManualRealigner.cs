using UnityEngine;

public class ManualRealigner : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private float movementSpeed = 0.1f;
    [SerializeField] private float rotationSpeed = 45f;

    public void HandleManualAlignment(Vector2 moveInput, Vector2 rotateInput)
    {
        // Déplacement avec le joystick gauche
        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0) * movementSpeed * Time.unscaledDeltaTime;
            origin.position += moveDirection;
        }

        // Rotation avec le joystick droit
        if (rotateInput != Vector2.zero)
        {
            // Rotation horizontale
            float rotation = rotateInput.x * rotationSpeed * Time.unscaledDeltaTime;
            origin.Rotate(Vector3.up, rotation);

            // Inclinaison verticale
            float tilt = rotateInput.y * rotationSpeed * Time.unscaledDeltaTime;
            origin.Rotate(Vector3.right, -tilt);
        }
    }
}
