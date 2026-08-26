using System;
using System.Collections.Generic;

public interface IInventory
{
    int  GoldCount { get; }
    int  CropCount { get; }
    bool HasWater  { get; }

    List<SeedInventoryEntry> SeedInventory { get; }

    int GetSeedAmount(SeedData seed);

    bool UseSeed(SeedData seed);

    void AddSeeds(SeedData seed, int amount);

    void AddCrop(int amount);
    void SellCrops(int amount, int pricePerCrop);
    void SpendGold(int amount);

    event Action OnInventoryChanged;
}
