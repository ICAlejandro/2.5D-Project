using UnityEngine;

public class DeliveryZoneBlocker : MonoBehaviour
{
    private BoxCollider blockingCollider;

    void Start()
    {
        blockingCollider = GetComponent<BoxCollider>();
        
        EvaluateBlockerState();
    }

    public void EvaluateBlockerState()
    {
        if (blockingCollider == null) return;

        Delivery[] remainingPackages = FindObjectsByType<Delivery>(FindObjectsSortMode.None);

        int activePackagesInZone = 0;
        foreach (Delivery package in remainingPackages)
        {
            if (Vector3.Distance(transform.position, package.transform.position) < 1.5f)
            {
                activePackagesInZone++;
            }
        }

        if (activePackagesInZone > 0)
        {
            blockingCollider.isTrigger = false;
        }
        else
        {
            blockingCollider.isTrigger = true;
        }
    }
}