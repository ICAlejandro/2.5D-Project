using UnityEngine;

public class DeliveryPackage : MonoBehaviour
{
    [Header("Package Contents")]
    public int seedCountInside = 1;

    [Header("Custom Interaction Range")]
    public float interactionRangeOverride = 8f;

    [Header("Spinning Visuals")]
    [Tooltip("Degrees per second the package rotates around its vertical Y axis.")]
    [SerializeField] private float rotationSpeed = 45f;

    void Update()
    {
        // Smoothly rotate the package on the ground
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void Interact(GameObject playerObject)
    {
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        
        if (inventory != null)
        {
            inventory.AddSeeds(seedCountInside);
            Debug.Log($"Collected package! Added {seedCountInside} seed(s) to inventory.");

            PlayerHUD hud = FindFirstObjectByType<PlayerHUD>();
            if (hud != null)
            {
                hud.UpdateHUDVisuals();
            }

            // Destroys itself cleanly, leaving any packages clipped underneath intact
            Destroy(gameObject);
        }
    }
}