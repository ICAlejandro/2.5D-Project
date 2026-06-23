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
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    private TimeManager timeManager;

    void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        }

        timeManager = FindFirstObjectByType<TimeManager>();

        // Subscribe to inventory changes so the HUD only updates when something actually changes
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateHUDVisuals;
        }

        UpdateHUDVisuals();
    }

    void OnDestroy()
    {
        // Always unsubscribe to avoid errors if the HUD is destroyed before the inventory
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateHUDVisuals;
        }
    }

    void Update()
    {
        // Time still needs to poll since it updates continuously every frame
        UpdateTimeDisplay();
    }

    // Called by OnInventoryChanged event — only runs when inventory actually changes
    public void UpdateHUDVisuals()
    {
        if (playerInventory == null) return;

        if (goldText != null) goldText.text = "Gold: " + playerInventory.goldCount;
        if (seedText != null) seedText.text = "Seeds: " + playerInventory.seedCount;
        if (waterText != null) waterText.text = "Water Can: " + (playerInventory.hasWater ? "Full" : "Empty");
        if (cropText != null) cropText.text = "Crops: " + playerInventory.cropCount;
    }

    // Called every frame in Update — time needs continuous refreshing
    private void UpdateTimeDisplay()
    {
        if (timeManager == null) return;

        if (dateText != null) dateText.text = "Date: " + timeManager.GetFormattedDate();
        if (timeText != null) timeText.text = "Time: " + timeManager.GetFormattedTime();
    }
}