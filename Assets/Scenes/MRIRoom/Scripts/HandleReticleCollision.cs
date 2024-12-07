using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;

public class HandleReticleCollision : MonoBehaviour
{
    [SerializeField] private GameObject MoonTarget;
    [SerializeField] private GameObject FeedbackGO;
    [SerializeField] private GameObject FixationCross;
    [SerializeField] private Material CorrectFeedbackMaterial;
    [SerializeField] private Material IncorrectFeedbackMaterial;
    [SerializeField] UnityEvent OnStart;
    [SerializeField] UnityEvent OnCorrectFeedback;
    [SerializeField] UnityEvent OnIncorrectFeedback;
    [SerializeField] UnityEvent OnTrajectoryCorrected;
    [SerializeField] UnityEvent OnExit;

    private XRRayInteractor rayInteractor;
    private XRSimpleInteractable moonInteractable;
    private Material fixationCross;
    private bool isHoveringMoonTarget = false;
    private bool previousFeedbackWasIncorrect = false;

    private bool isCorrectFeedbackActive = false;
    private bool isIncorrectFeedbackActive = false;
    private bool hasAudioFeedback = true;

    public void Start()
    {

    }

    public void SetAudioFeedbackState(bool state)
    {
        this.hasAudioFeedback = state;
    }

    public void Init()
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

        OnStart.Invoke();

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
        foreach (Transform child in FeedbackGO.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = CorrectFeedbackMaterial;
            }
        }
        fixationCross.SetColor("_Color", Color.green);

        if (hasAudioFeedback) HandleCorrectAudio();
    }

    private void HandleIncorrectFeedback()
    {
        foreach (Transform child in FeedbackGO.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = IncorrectFeedbackMaterial;
            }
        }
        fixationCross.SetColor("_Color", Color.red);

        if (hasAudioFeedback) HandleIncorrectAudio();
    }

    private void HandleCorrectAudio()
    {
        if (isCorrectFeedbackActive) return;

        if (previousFeedbackWasIncorrect)
        {
            OnTrajectoryCorrected.Invoke();
        }
        OnCorrectFeedback.Invoke();
        previousFeedbackWasIncorrect = false;
        isCorrectFeedbackActive = true;
        isIncorrectFeedbackActive = false;
    }

    private void HandleIncorrectAudio()
    {
        if (isIncorrectFeedbackActive) return;
        OnIncorrectFeedback.Invoke();
        previousFeedbackWasIncorrect = true;
        isIncorrectFeedbackActive = true;
        isCorrectFeedbackActive = false;
    }

    public void OnDisable()
    {
        OnExit.Invoke();
    }
}
