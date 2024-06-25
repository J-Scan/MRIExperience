using UnityEngine;

//TODO: Transform it so that the parent manages collision of all the children

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private GameObject collider1;
    [SerializeField] private GameObject collider2;

    [SerializeField] private LocationTransition locaScript;

    public void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collision with: " + other.gameObject);
        if (other.gameObject == collider1)
        {
            HandlePlayerTopCollision();
        }

        if (other.gameObject == collider2)
        {
            HandlePlayerBottomCollision();
        }
    }

    public void HandlePlayerTopCollision()
    {
        Debug.Log("Player has entered the trigger area.");
        locaScript.HandleScannerTopCollision();
    }

    public void HandlePlayerBottomCollision()
    {
        Debug.Log("Player has entered the trigger area.");
        locaScript.HandleScannerBottomCollision();
    }
}
