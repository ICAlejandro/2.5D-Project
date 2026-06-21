using System.Collections.Generic;
using UnityEngine;

public class InteractableGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    [ColorUsage(true, true)] 
    public Color glowColor = Color.cyan;
    
    [Tooltip("The distance from the player where this object starts glowing.")]
    public float interactionDistance = 3f;

    [Header("Player Target override (Optional)")]
    public Transform playerTransform;
    
    private Collider objectCollider; 
    private List<Material> targetMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private bool isGlowing = false;

    void Start()
    {
        // Fallback: If no player transform is manually assigned, search by tag
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"InteractableGlow on {gameObject.name} cannot find a GameObject with the 'Player' tag!");
            }
        }

        objectCollider = GetComponent<Collider>();

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in allRenderers)
        {
            Material[] mats = ren.materials;
            foreach (Material mat in mats)
            {
                if (mat != null)
                {
                    targetMaterials.Add(mat);
                    
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        originalColors.Add(mat.GetColor("_EmissionColor"));
                    }
                    else
                    {
                        originalColors.Add(Color.black);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || targetMaterials.Count == 0) return;

        float distanceToPlayer = 0f;

        if (objectCollider != null)
        {
            Vector3 closestPointOnBounds = objectCollider.bounds.ClosestPoint(playerTransform.position);
            
            if (closestPointOnBounds == playerTransform.position)
            {
                distanceToPlayer = Vector3.Distance(objectCollider.bounds.center, playerTransform.position);
            }
            else
            {
                distanceToPlayer = Vector3.Distance(closestPointOnBounds, playerTransform.position);
            }
        }
        else
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        }

        // Uses the standalone threshold variable instead of checking an external player script
        if (distanceToPlayer <= interactionDistance)
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
        for (int i = 0; i < targetMaterials.Count; i++)
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
        for (int i = 0; i < targetMaterials.Count; i++)
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
        if (targetMaterials != null)
        {
            for (int i = 0; i < targetMaterials.Count; i++)
            {
                if (targetMaterials[i] != null)
                {
                    Destroy(targetMaterials[i]);
                }
            }
        }
    }
}