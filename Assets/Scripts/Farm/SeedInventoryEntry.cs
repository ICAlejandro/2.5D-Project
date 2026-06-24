using System;
using UnityEngine;

/// <summary>
/// Pairs a SeedData asset with how many the player currently owns.
/// Set up your starting seeds in the PlayerInventory Inspector
/// by adding entries to the Seed Inventory list.
/// </summary>
[Serializable]
public class SeedInventoryEntry
{
    [Tooltip("Drag a SeedData asset here.")]
    public SeedData seedData;

    [Tooltip("How many of this seed the player starts with.")]
    public int amount = 0;
}
