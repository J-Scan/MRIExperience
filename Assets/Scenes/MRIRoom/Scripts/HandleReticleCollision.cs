using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandleReticleCollision : MonoBehaviour
{
    [SerializeField] private GameObject MoonTarget;
    [SerializeField] private GameObject FeedbackGO;
    [SerializeField] private GameObject FixationCross;
    [SerializeField] private Material CorrectFeedbackMaterial;
    [SerializeField] private Material IncorrectFeedbackMaterial;

    private XRRayInteractor rayInteractor;
    private XRSimpleInteractable moonInteractable;
    private Material fixationCross;
    private bool isHoveringMoonTarget = false;

    private void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("XRRayInteractor component not found on the GameObject.");
            return;
        }

        moonInteractable = MoonTarget.GetComponent<XRSimpleInteractable>();
        if (moonInteractable == null)
        {
            Debug.LogError("XRSimpleInteractable component not found on the MoonTarget.");
            return;
        }

        fixationCross = FixationCross.GetComponent<Renderer>().material;
        if (fixationCross == null)
        {
            Debug.LogError("SpriteRenderer component not found on the FixationCross.");
            return;
        }

        // Subscribe to interaction events
        rayInteractor.hoverEntered.AddListener(OnInteractorHoverEntered);
        rayInteractor.hoverExited.AddListener(OnInteractorHoverExited);
    }

    void OnInteractorHoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactableObject == moonInteractable)
        {
            isHoveringMoonTarget = true;
            HandleCorrectFeedback();
        }
        else
        {
            HandleIncorrectFeedback();
        }
    }

    void OnInteractorHoverExited(HoverExitEventArgs args)
    {
        if (args.interactableObject == moonInteractable)
        {
            isHoveringMoonTarget = false;
            HandleIncorrectFeedback();
        }
    }

    private void HandleCorrectFeedback()
    {
        Debug.Log("Inside Handle Correct Feedback");
        foreach (Transform child in FeedbackGO.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = CorrectFeedbackMaterial;
            }
        }
        fixationCross.SetColor("_Color", Color.green);
    }

    private void HandleIncorrectFeedback()
    {
        Debug.Log("Inside Handle Incorrect Feedback");
        if (!isHoveringMoonTarget)
        {
            foreach (Transform child in FeedbackGO.transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.material = IncorrectFeedbackMaterial;
                }
            }
        }
        fixationCross.SetColor("_Color", Color.red);
    }
}
