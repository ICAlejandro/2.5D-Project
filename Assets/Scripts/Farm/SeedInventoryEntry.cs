using System;
using UnityEngine;

[Serializable]
public class SeedInventoryEntry
{
    [Tooltip("Drag a SeedData asset here.")]
    public SeedData seedData;

    [Tooltip("How many of this seed the player starts with.")]
    public int amount = 0;
}
