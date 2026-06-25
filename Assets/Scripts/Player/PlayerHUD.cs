using System.Linq;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI Text MeshPro References")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI cropText;

    [Header("Time Display Settings")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    private IInventory    inventory;
    private ITimeProvider timeProvider;

    void Start()
    {
        inventory    = ServiceLocator.Get<IInventory>();
        timeProvider = ServiceLocator.Get<ITimeProvider>();

        if (inventory != null)
            inventory.OnInventoryChanged += UpdateHUDVisuals;

        UpdateHUDVisuals();
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateHUDVisuals;
    }

    void Update()
    {
        UpdateTimeDisplay();
    }

    public void UpdateHUDVisuals()
    {
        if (inventory == null) return;

        if (goldText  != null) goldText.text  = "Gold: "      + inventory.GoldCount;
        int totalSeeds = inventory.SeedInventory.Sum(e => e.amount);
        if (seedText  != null) seedText.text  = "Seeds: " + totalSeeds;
        if (waterText != null) waterText.text = "Water Can: " + (inventory.HasWater ? "Full" : "Empty");
        if (cropText  != null) cropText.text  = "Crops: "     + inventory.CropCount;
    }

    private void UpdateTimeDisplay()
    {
        if (timeProvider == null) return;

        if (dateText != null) dateText.text = "Date: " + timeProvider.GetFormattedDate();
        if (timeText != null) timeText.text = "Time: " + timeProvider.GetFormattedTime();
    }
}
