using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls one row in the seed selection list.
/// Exposes HasStock and SelectItem() for keyboard navigation via OptionUI.
/// </summary>
public class SeedListItem : MonoBehaviour
{
    [Header("UI References")]
    public Image           seedIcon;
    public TextMeshProUGUI seedNameText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI descriptionText;
    public Button          selectButton;

    // ── Public for OptionUI keyboard navigation ────────────────────────────
    public bool HasStock { get; private set; }

    private Action _onSelected;

    public void Setup(SeedInventoryEntry entry, Action onSelected)
    {
        HasStock   = entry.amount > 0;
        _onSelected = onSelected;

        if (seedNameText    != null) seedNameText.text    = entry.seedData.seedName;
        if (amountText      != null) amountText.text      = $"x{entry.amount}";
        if (descriptionText != null) descriptionText.text = entry.seedData.description;
        if (seedIcon        != null && entry.seedData.seedIcon != null)
            seedIcon.sprite = entry.seedData.seedIcon;

        if (selectButton != null)
        {
            selectButton.interactable = HasStock;
            if (HasStock)
                selectButton.onClick.AddListener(() => SelectItem());
        }

        // Grey out the row if out of stock
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group != null)
            group.alpha = HasStock ? 1f : 0.4f;
    }

    /// <summary>Called by button click or keyboard Enter in OptionUI.</summary>
    public void SelectItem()
    {
        _onSelected?.Invoke();
    }
}
