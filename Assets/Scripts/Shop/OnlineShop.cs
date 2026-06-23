using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OnlineShop : Interactable
{
    [Header("Shop Balancing")]
    public int seedCost = 5;
    public int cropValue = 10;

    [Header("Delivery Settings")]
    [SerializeField] private float deliveryTimeSeconds = 3f;
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private Transform deliverySpawnPoint;

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
        if (shopCanvas != null) shopCanvas.SetActive(false);

        if (buySeedButton != null) buySeedButton.onClick.AddListener(BuySeed);
        if (sellCropButton != null) sellCropButton.onClick.AddListener(SellCrop);
        if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        
        playerHUD = FindFirstObjectByType<PlayerHUD>();
        activePlayerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public override void Interact(GameObject player)
    {
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; 
        }
    }

    public void BuySeed()
    {
        if (activePlayerInventory == null) return;

        if (activePlayerInventory.goldCount >= seedCost)
        {
            activePlayerInventory.goldCount -= seedCost;
            if (playerHUD != null) playerHUD.UpdateHUDVisuals();

            StartCoroutine(ProcessDeliveryRoutine(1));
            Debug.Log("Ordered 1 seed via Online Shop!");
        }
        else
        {
            Debug.Log("Not enough gold to buy a seed!");
        }
    }

    private IEnumerator ProcessDeliveryRoutine(int amountOrdered)
    {
        yield return new WaitForSecondsRealtime(deliveryTimeSeconds);

        if (packagePrefab != null && deliverySpawnPoint != null)
        {
            GameObject spawnedPackage = Instantiate(packagePrefab, deliverySpawnPoint.position, deliverySpawnPoint.rotation);
            Debug.Log("A package has arrived at the delivery point.");

            Delivery deliveryScript = spawnedPackage.GetComponent<Delivery>();
            if (deliveryScript != null)
            {
                deliveryScript.seedCountInside = amountOrdered;
            }

            // Tell the spawn point blocker to check for packages and update physics state
            DeliveryZoneBlocker blocker = deliverySpawnPoint.GetComponent<DeliveryZoneBlocker>();
            if (blocker != null)
            {
                blocker.EvaluateBlockerState();
            }
        }
    }

    public void SellCrop()
    {
        if (activePlayerInventory == null) return;

        if (activePlayerInventory.cropCount > 0)
        {
            activePlayerInventory.cropCount--;
            activePlayerInventory.goldCount += cropValue;
            if (playerHUD != null) playerHUD.UpdateHUDVisuals();
            Debug.Log("Sold 1 crop via Online Shop!");
        }
    }

    public void CloseShop()
    {
        if (shopCanvas != null) shopCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}