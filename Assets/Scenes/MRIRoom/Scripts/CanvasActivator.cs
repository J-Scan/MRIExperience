using System.Collections.Generic;
using UnityEngine;

public class CanvasActivator : MonoBehaviour
{
    [System.Serializable]
    public class IgnoredCanvas
    {
        public Canvas canvas; // Canvas à ignorer
        public bool ignoreChildren; // Si true, ignore tous les canvases enfants
    }

    [SerializeField] private List<IgnoredCanvas> ignoredCanvases = new List<IgnoredCanvas>(); // Liste des canvases ignorés
    private List<Canvas> allCanvases = new List<Canvas>(); // Liste des canvases trouvés

    public void FindAllActiveCanvases()
    {
        // Trouve tous les Canvas actifs dans la hiérarchie
        Canvas[] canvases = FindObjectsOfType<Canvas>(false); // Exclut les GameObjects désactivés
        allCanvases.Clear();

        foreach (Canvas canvas in canvases)
        {
            if (!ShouldIgnoreCanvas(canvas))
            {
                allCanvases.Add(canvas);
            }
        }
    }

    /// <summary>
    /// Vérifie si un canvas ou ses enfants doivent être ignorés.
    /// </summary>
    private bool ShouldIgnoreCanvas(Canvas canvas)
    {
        foreach (var ignored in ignoredCanvases)
        {
            if (canvas == ignored.canvas)
            {
                return true; // Ignorer ce canvas
            }

            if (ignored.ignoreChildren && IsChildOf(canvas.transform, ignored.canvas.transform))
            {
                return true; // Ignorer les enfants de ce canvas
            }
        }
        return false;
    }

    /// <summary>
    /// Vérifie si un Transform est enfant d'un autre Transform.
    /// </summary>
    private bool IsChildOf(Transform child, Transform parent)
    {
        while (child != null)
        {
            if (child == parent)
            {
                return true;
            }
            child = child.parent;
        }
        return false;
    }

    /// <summary>
    /// Désactive tous les canvases sauf ceux à ignorer.
    /// </summary>
    public void DisableAllCanvasesExceptIgnored()
    {
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }

        // Réactive les canvases ignorés
        foreach (var ignored in ignoredCanvases)
        {
            if (ignored.canvas != null)
            {
                ignored.canvas.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Active tous les canvases trouvés.
    /// </summary>
    public void EnableAllCanvases()
    {
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(true);
        }

        // Assurez-vous que les canvases ignorés restent actifs
        foreach (var ignored in ignoredCanvases)
        {
            if (ignored.canvas != null)
            {
                ignored.canvas.gameObject.SetActive(true);
            }
        }
    }
}
