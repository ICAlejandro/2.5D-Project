using System;

/// <summary>
/// Contract for any inventory system. Scripts depend on this instead of PlayerInventory directly.
/// Swap out the implementation freely without touching any consumer script.
/// </summary>
public interface IInventory
{
    int  GoldCount { get; }
    int  SeedCount { get; }
    int  CropCount { get; }
    bool HasWater  { get; }

    void AddCrop(int amount);
    void SellCrops(int amount, int pricePerCrop);
    bool UseSeed();
    void AddSeeds(int amount);
    void SpendGold(int amount);

    /// <summary>Fires whenever any inventory value changes.</summary>
    event Action OnInventoryChanged;
}
