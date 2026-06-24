using UnityEngine;

/// <summary>
/// ScriptableObject that holds all data for one seed/crop type.
/// Create a new one in Unity via: Right Click > Create > Farming > Seed Data
/// Fill in all fields in the Inspector — no coding needed to add new crops.
/// </summary>
[CreateAssetMenu(fileName = "NewSeedData", menuName = "Farming/Seed Data")]
public class SeedData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("The name shown to the player in the UI.")]
    public string seedName = "Unknown Seed";

    [Tooltip("A short description shown in the seed selection list.")]
    [TextArea(2, 4)]
    public string description = "";

    [Header("Growth Settings")]
    [Tooltip("How many in-game days this seed takes to fully grow.")]
    public float daysToGrow = 1f;

    [Tooltip("How many crops this seed yields when harvested.")]
    public int cropYieldAmount = 1;

    [Header("Visuals")]
    [Tooltip("The sprite shown on the plant while it is growing or ready.")]
    public Sprite plantSprite;

    [Tooltip("The icon shown in the seed selection list.")]
    public Sprite seedIcon;
}
