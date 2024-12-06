using UnityEngine;

public class ManualRealigner : MonoBehaviour
{
    [SerializeField] private Transform origin;        // Le transform � d�placer
    [SerializeField] private Transform cameraTransform; // La cam�ra pour d�terminer la direction
    [SerializeField] private float movementSpeed = 2f; // Vitesse de d�placement
    [SerializeField] private float rotationSpeed = 45f; // Vitesse de rotation (en degr�s par seconde)

    private bool moveUp = false;
    private bool moveDown = false;
    private bool moveForward = false;
    private bool moveBackward = false;
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool rotateLeft = false;
    private bool rotateRight = false;

    private float cameraRotationAngle = 0f; // Rotation accumul�e de la cam�ra

    private void Update()
    {
        HandleRepositioning();
    }

    private void HandleRepositioning()
    {
        // Recalcule les axes locaux en fonction de la rotation de la cam�ra
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        // Mouvement vertical
        if (moveUp) Move(Vector3.up);
        if (moveDown) Move(Vector3.down);

        // Mouvement avant/arri�re
        if (moveForward) Move(forward);
        if (moveBackward) Move(-forward);

        // Mouvement gauche/droite
        if (moveLeft) Move(-right);
        if (moveRight) Move(right);

        // Rotation
        if (rotateLeft) Rotate(-1);
        if (rotateRight) Rotate(1);
    }

    private void Move(Vector3 direction)
    {
        Vector3 moveDirection = direction * movementSpeed * Time.unscaledDeltaTime;
        origin.position += moveDirection;
    }

    private void Rotate(float direction)
    {
        float rotationAmount = direction * rotationSpeed * Time.unscaledDeltaTime;
        origin.Rotate(cameraTransform.up.normalized, rotationAmount, Space.Self);
    }

    // M�thodes � lier aux boutons
    public void OnMoveUp(bool isPressed) => moveUp = isPressed;
    public void OnMoveDown(bool isPressed) => moveDown = isPressed;
    public void OnMoveForward(bool isPressed) => moveForward = isPressed;
    public void OnMoveBackward(bool isPressed) => moveBackward = isPressed;
    public void OnMoveLeft(bool isPressed) => moveLeft = isPressed;
    public void OnMoveRight(bool isPressed) => moveRight = isPressed;
    public void OnRotateLeft(bool isPressed) => rotateLeft = isPressed;
    public void OnRotateRight(bool isPressed) => rotateRight = isPressed;
}
