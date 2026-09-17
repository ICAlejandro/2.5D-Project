using UnityEngine;

public class DeliveryZoneBlocker : MonoBehaviour
{
    private BoxCollider blockingCollider;

    void Start()
    {
        // Cache the collider attached to this spawn point
        blockingCollider = GetComponent<BoxCollider>();
        
        // Ensure it starts out as a safe, walk-through trigger zone
        EvaluateBlockerState();
    }

    // Call this whenever packages are spawned or destroyed
    public void EvaluateBlockerState()
    {
        if (blockingCollider == null) return;

        // Count how many objects with the 'Delivery' component are childed or sitting in this area
        // We look for 'Delivery' scripts attached to active packages
        Delivery[] remainingPackages = FindObjectsByType<Delivery>(FindObjectsSortMode.None);

        int activePackagesInZone = 0;
        foreach (Delivery package in remainingPackages)
        {
            // Only count packages that are physically close to this spawn point (within 1.5 units)
            if (Vector3.Distance(transform.position, package.transform.position) < 1.5f)
            {
                activePackagesInZone++;
            }
        }

        if (activePackagesInZone > 0)
        {
            // Packages exist! Make the wall a SOLID physical barrier
            blockingCollider.isTrigger = false;
        }
        else
        {
            // No packages left! Make the wall INTANGIBLE so the player can walk through
            blockingCollider.isTrigger = true;
        }
    }
}