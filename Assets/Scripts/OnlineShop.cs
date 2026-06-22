using UnityEngine;
using UnityEngine.UI;

public class OnlineShop : MonoBehaviour
{
    [Header("Shop Balancing")]
    public int seedCost = 5;
    public int cropValue = 10;

    [Header("UI Panel Reference")]
    public GameObject shopCanvas;

    [Header("Button References")]
    public Button buySeedButton;
    public Button sellCropButton;
    public Button closeButton;

    private PlayerInventory activePlayerInventory;
    private PlayerHUD playerHUD;

    void Start()
    {
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(false);
        }

        if (buySeedButton != null) buySeedButton.onClick.AddListener(BuySeed);
        if (sellCropButton != null) sellCropButton.onClick.AddListener(SellCrop);
        if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        
        // Find the HUD system in the scene
        playerHUD = FindFirstObjectByType<PlayerHUD>();
    }

    public void OpenShop(GameObject player)
    {
        activePlayerInventory = player.GetComponent<PlayerInventory>();
        
        if (activePlayerInventory != null && shopCanvas != null)
        {
            shopCanvas.SetActive(true);
            
            // Unlock mouse cursor so the player can click UI elements
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f; // Freeze game actions while menu is open
        }
    }

    public void BuySeed()
    {
        if (activePlayerInventory == null) return;

        if (activePlayerInventory.goldCount >= seedCost)
        {
            activePlayerInventory.goldCount -= seedCost;
            activePlayerInventory.AddSeeds(1);
            Debug.Log("Bought 1 seed via Online Shop!");

            // Force HUD to update its numbers immediately while paused
            if (playerHUD != null) playerHUD.UpdateHUDVisuals();
        }
    }

    public void SellCrop()
    {
        if (activePlayerInventory == null) return;

        if (activePlayerInventory.cropCount > 0)
        {
            activePlayerInventory.cropCount--;
            activePlayerInventory.goldCount += cropValue;
            Debug.Log("Sold 1 crop via Online Shop!");

            // Force HUD to update its numbers immediately while paused
            if (playerHUD != null) playerHUD.UpdateHUDVisuals();
        }
    }

    public void CloseShop()
    {
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(false);
        }

        // Relock mouse cursor for standard gameplay movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f; // Resume gameplay execution
    }
}