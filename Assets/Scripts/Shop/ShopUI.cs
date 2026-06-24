using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles shop UI only — opening/closing the panel and cursor state.
/// Transaction logic lives separately in OnlineShop.cs on the same GameObject.
/// Uses PlayerStateManager to lock/unlock player movement.
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("UI Panel Reference")]
    public GameObject shopCanvas;

    [Header("Button References")]
    public Button buySeedButton;
    public Button sellCropButton;
    public Button closeButton;

    private OnlineShop onlineShop;

    void Start()
    {
        onlineShop = GetComponent<OnlineShop>();

        if (shopCanvas != null) shopCanvas.SetActive(false);

        if (buySeedButton  != null) buySeedButton.onClick.AddListener(onlineShop.BuySeed);
        if (sellCropButton != null) sellCropButton.onClick.AddListener(onlineShop.SellCrop);
        if (closeButton    != null) closeButton.onClick.AddListener(CloseShop);
    }

    public void OpenShop()
    {
        if (shopCanvas == null) return;

        shopCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Lock player via state manager — distinct from dialogue
        PlayerStateManager.SetState(PlayerState.Dialogue);
    }

    public void CloseShop()
    {
        if (shopCanvas == null) return;

        shopCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        PlayerStateManager.SetState(PlayerState.Free);
    }
}
