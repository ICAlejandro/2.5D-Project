using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int goldCount = 0;
    public int harvestedCrops = 0;

    public void AddCrop(int marketValue)
    {
        harvestedCrops += 1;
        goldCount += marketValue;
    }
}