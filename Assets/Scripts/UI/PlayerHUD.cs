using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Inventory Data Source")]
    public PlayerInventory playerInventory;

    [Header("UI Text MeshPro References")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI cropText; 

    [Header("Time Display Settings")]
    [Tooltip("Assign TextMeshPro elements to display the calendar date and clock time.")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    private TimeManager timeManager;

    void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        }

        // Cache the master calendar reference
        timeManager = FindFirstObjectByType<TimeManager>();

        UpdateHUDVisuals();
    }

    void Update()
    {
        UpdateHUDVisuals();
    }

    public void UpdateHUDVisuals()
    {
        // 1. Process Inventory Display Values
        if (playerInventory != null)
        {
            if (goldText != null)
            {
                goldText.text = "Gold: " + playerInventory.goldCount;
            }

            if (seedText != null)
            {
                seedText.text = "Seeds: " + playerInventory.seedCount;
            }

            if (waterText != null)
            {
                waterText.text = "Water Can: " + (playerInventory.hasWater ? "Full" : "Empty");
            }

            if (cropText != null)
            {
                cropText.text = "Crops: " + playerInventory.cropCount;
            }
        }

        // 2. Process Calendar & Time Display Values
        if (timeManager != null)
        {
            if (dateText != null)
            {
                dateText.text = "Date: " + timeManager.GetFormattedDate();
            }

            if (timeText != null)
            {
                timeText.text = "Time: " + timeManager.GetFormattedTime();
            }
        }
    }
}