using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OnlineShop : MonoBehaviour
{
    [Header("Shop Balancing")]
    public int seedCost = 5;
    public int cropValue = 10;

    [Header("Delivery Settings")]
    [Tooltip("How many seconds it takes for the package to arrive.")]
    [SerializeField] private float deliveryTimeSeconds = 3f;
    
    [Tooltip("Drop your 3D furniture_package prefab here.")]
    [SerializeField] private GameObject packagePrefab;
    
    [Tooltip("Where the packages will spawn and clip together.")]
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
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(false);
        }

        if (buySeedButton != null) buySeedButton.onClick.AddListener(BuySeed);
        if (sellCropButton != null) sellCropButton.onClick.AddListener(SellCrop);
        if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        
        playerHUD = FindFirstObjectByType<PlayerHUD>();
    }

    public void OpenShop(GameObject player)
    {
        activePlayerInventory = player.GetComponent<PlayerInventory>();
        
        if (activePlayerInventory != null && shopCanvas != null)
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
            Debug.Log("Order placed! Shipping 1 seed...");

            if (playerHUD != null) playerHUD.UpdateHUDVisuals();

            StartCoroutine(ProcessDeliveryRoutine(1));
        }
    }

    private IEnumerator ProcessDeliveryRoutine(int amountOrdered)
    {
        yield return new WaitForSecondsRealtime(deliveryTimeSeconds);

        if (packagePrefab != null && deliverySpawnPoint != null)
        {
            // Spawn the package exactly at the spawn point position so they clip together
            GameObject spawnedPackage = Instantiate(packagePrefab, deliverySpawnPoint.position, deliverySpawnPoint.rotation);
            Debug.Log("A package has arrived and clipped into the delivery point.");

            // Inject the order details
            DeliveryPackage packageScript = spawnedPackage.GetComponent<DeliveryPackage>();
            if (packageScript != null)
            {
                packageScript.seedCountInside = amountOrdered;
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
            Debug.Log("Sold 1 crop via Online Shop!");

            if (playerHUD != null) playerHUD.UpdateHUDVisuals();
        }
    }

    public void CloseShop()
    {
        if (shopCanvas != null)
        {
            shopCanvas.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f; 
    }
}