using UnityEngine;

public class InteractableGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    [ColorUsage(true, true)] // Enables the HDR intensity picker for a bright neon bloom
    public Color glowColor = Color.cyan;

    private Transform playerTransform;
    private PlayerAction playerAction;
    private Renderer meshRenderer;
    
    // Arrays to store data for objects with multiple materials
    private Material[] targetMaterials;
    private Color[] originalColors;
    private bool isGlowing = false;

    void Start()
    {
        // 1. Find the player in the scene automatically using the PlayerAction component
        playerAction = Object.FindFirstObjectByType<PlayerAction>();
        if (playerAction != null)
        {
            playerTransform = playerAction.transform;
        }
        else
        {
            Debug.LogWarning($"InteractableGlow on {gameObject.name} cannot find a Player object with a PlayerAction script attached to it!");
        }

        // 2. Grab the MeshRenderer attached to this object
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Fallback: If it's an imported FBX mesh container, check child objects for the renderer
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        // 3. Setup multiple material references and cache all starting baseline colors
        if (meshRenderer != null)
        {
            // meshRenderer.materials automatically instantiates and creates an array copy of all materials
            targetMaterials = meshRenderer.materials;
            originalColors = new Color[targetMaterials.Length];

            for (int i = 0; i < targetMaterials.Length; i++)
            {
                if (targetMaterials[i].HasProperty("_EmissionColor"))
                {
                    originalColors[i] = targetMaterials[i].GetColor("_EmissionColor");
                }
                else
                {
                    originalColors[i] = Color.black;
                }
            }
        }
    }

    void Update()
    {
        // Guard clause: stop calculation if player or materials aren't loaded properly
        if (playerTransform == null || playerAction == null || targetMaterials == null) return;

        // Calculate the absolute straight-line distance between the player and this object
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Safely check proximity against the PlayerAction script settings
        if (distanceToPlayer <= playerAction.InteractionDistance)
        {
            if (!isGlowing)
            {
                TurnOnGlow();
            }
        }
        else
        {
            if (isGlowing)
            {
                TurnOffGlow();
            }
        }
    }

    void TurnOnGlow()
    {
        // Loop through every single material slot on the mesh and turn on emission
        for (int i = 0; i < targetMaterials.Length; i++)
        {
            if (targetMaterials[i] != null)
            {
                targetMaterials[i].EnableKeyword("_EMISSION");
                targetMaterials[i].SetColor("_EmissionColor", glowColor);
            }
        }
        isGlowing = true;
    }

    void TurnOffGlow()
    {
        // Loop through every single material slot on the mesh and revert back to its old baseline color
        for (int i = 0; i < targetMaterials.Length; i++)
        {
            if (targetMaterials[i] != null)
            {
                targetMaterials[i].SetColor("_EmissionColor", originalColors[i]);
                
                if (originalColors[i] == Color.black)
                {
                    targetMaterials[i].DisableKeyword("_EMISSION");
                }
            }
        }
        isGlowing = false;
    }

    private void OnDestroy()
    {
        // Clean up all instantiated material copies to prevent memory leaks in your project scene
        if (targetMaterials != null)
        {
            for (int i = 0; i < targetMaterials.Length; i++)
            {
                if (targetMaterials[i] != null)
                {
                    Destroy(targetMaterials[i]);
                }
            }
        }
    }
}