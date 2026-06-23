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
        // Resolve from ServiceLocator instead of searching the scene
        if (playerInventory == null)
            playerInventory = ServiceLocator.Get<PlayerInventory>();

        timeManager = ServiceLocator.Get<TimeManager>();

        if (playerInventory != null)
            playerInventory.OnInventoryChanged += UpdateHUDVisuals;

        UpdateHUDVisuals();
    }

    void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= UpdateHUDVisuals;
    }

    void Update()
    {
        // Time still needs to poll since it updates continuously every frame
        UpdateTimeDisplay();
    }

    public void UpdateHUDVisuals()
    {
        if (playerInventory == null) return;

        if (goldText  != null) goldText.text  = "Gold: "      + playerInventory.goldCount;
        if (seedText  != null) seedText.text  = "Seeds: "     + playerInventory.seedCount;
        if (waterText != null) waterText.text = "Water Can: " + (playerInventory.hasWater ? "Full" : "Empty");
        if (cropText  != null) cropText.text  = "Crops: "     + playerInventory.cropCount;
    }

    private void UpdateTimeDisplay()
    {
        if (timeManager == null) return;

        if (dateText != null) dateText.text = "Date: " + timeManager.GetFormattedDate();
        if (timeText != null) timeText.text = "Time: " + timeManager.GetFormattedTime();
    }
}
