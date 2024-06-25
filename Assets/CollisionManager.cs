using UnityEngine;

//TODO: Transform it so that the parent manages collision of all the children

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private GameObject item1;
    [SerializeField] private GameObject collider1;

    [SerializeField] private LocationTransition locaScript;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with: " + other.gameObject);
        if (other.gameObject == collider1)
        {
            HandlePlayerTriggerEnter();
        }
    }

    public void HandlePlayerTriggerEnter()
    {
        Debug.Log("Player has entered the trigger area.");
        locaScript.HandleScannerTopCollision();
    }
}
