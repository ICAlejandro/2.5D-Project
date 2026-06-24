using System;
using System.Collections.Generic;

/// <summary>
/// Contract for any inventory system.
/// </summary>
public interface IInventory
{
    int  GoldCount { get; }
    int  CropCount { get; }
    bool HasWater  { get; }

    // ── Seed list ──────────────────────────────────────────────────────────
    /// <summary>All seed types the player owns (even if amount is 0).</summary>
    List<SeedInventoryEntry> SeedInventory { get; }

    /// <summary>Returns how many of a specific seed the player owns.</summary>
    int GetSeedAmount(SeedData seed);

    /// <summary>Deducts one of the specified seed type.</summary>
    bool UseSeed(SeedData seed);

    /// <summary>Adds seeds of the specified type.</summary>
    void AddSeeds(SeedData seed, int amount);

    // ── Other mutators ─────────────────────────────────────────────────────
    void AddCrop(int amount);
    void SellCrops(int amount, int pricePerCrop);
    void SpendGold(int amount);

    event Action OnInventoryChanged;
}
