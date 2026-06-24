using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// General-purpose option UI manager.
/// Handles reusable UI panels that any system can call into:
///   - YesNoDialogue: a simple yes/no popup with keyboard navigation
///   - InventoryList: a scrollable, clickable list with keyboard navigation
///
/// Controls:
///   W / S or Up / Down  — navigate between options
///   Enter               — confirm selected option
///   Escape              — cancel / close
/// </summary>
public class OptionUI : MonoBehaviour
{
    [Header("YesNo Dialogue Panel")]
    public GameObject yesNoDialogue;
    public Button     yesButton;
    public Button     noButton;

    [Header("Inventory List Panel")]
    public GameObject inventoryList;
    public Transform  inventoryListContainer;
    public GameObject seedListItemPrefab;

    [Header("Navigation Highlight")]
    [Tooltip("Color applied to the currently selected button/item.")]
    public Color highlightColor = Color.yellow;
    public Color normalColor    = Color.white;

    private Action _onYes;
    private Action _onNo;

    // ── YesNo navigation ───────────────────────────────────────────────────
    private int  _yesNoIndex  = 0;
    private bool _yesNoActive = false;

    // ── Inventory list navigation ──────────────────────────────────────────
    private List<SeedListItem> _listItems = new List<SeedListItem>();
    private int  _listIndex  = 0;
    private bool _listActive = false;

    void Start()
    {
        if (yesNoDialogue != null) yesNoDialogue.SetActive(false);
        if (inventoryList != null) inventoryList.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton  != null) noButton.onClick.AddListener(OnNoClicked);
    }

    void Update()
    {
        if (_yesNoActive) HandleYesNoNavigation();
        if (_listActive)  HandleListNavigation();
    }

    // ── YesNo Dialogue ─────────────────────────────────────────────────────

    public void ShowYesNo(Action onYes, Action onNo = null)
    {
        _onYes       = onYes;
        _onNo        = onNo;
        _yesNoIndex  = 0;
        _yesNoActive = true;

        if (yesNoDialogue != null) yesNoDialogue.SetActive(true);

        HighlightYesNo();
        PlayerStateManager.SetState(PlayerState.Dialogue);
    }

    public void HideYesNo()
    {
        _yesNoActive = false;
        if (yesNoDialogue != null) yesNoDialogue.SetActive(false);
        // Don't set state here — let the callback decide what comes next
    }

    private void OnYesClicked()
    {
        HideYesNo();
        _onYes?.Invoke();
        // Only free the player if nothing else opened after Yes
        TryResumeGame();
    }

    private void OnNoClicked()
    {
        HideYesNo();
        _onNo?.Invoke();
        // Only free the player if nothing else opened after No
        TryResumeGame();
    }

    private void HandleYesNoNavigation()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            _yesNoIndex = 0;
            HighlightYesNo();
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame ||
                 Keyboard.current.dKey.wasPressedThisFrame ||
                 Keyboard.current.downArrowKey.wasPressedThisFrame ||
                 Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            _yesNoIndex = 1;
            HighlightYesNo();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (_yesNoIndex == 0) OnYesClicked();
            else                  OnNoClicked();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnNoClicked();
    }

    private void HighlightYesNo()
    {
        SetButtonColor(yesButton, _yesNoIndex == 0 ? highlightColor : normalColor);
        SetButtonColor(noButton,  _yesNoIndex == 1 ? highlightColor : normalColor);
    }

    // ── Inventory List ─────────────────────────────────────────────────────

    public void ShowInventoryList(List<SeedInventoryEntry> seeds, Action<SeedData> onSeedSelected)
    {
        if (inventoryList == null || inventoryListContainer == null || seedListItemPrefab == null) return;

        foreach (Transform child in inventoryListContainer)
            Destroy(child.gameObject);
        _listItems.Clear();

        foreach (SeedInventoryEntry entry in seeds)
        {
            GameObject   item     = Instantiate(seedListItemPrefab, inventoryListContainer);
            SeedListItem listItem = item.GetComponent<SeedListItem>();

            if (listItem != null)
            {
                SeedData capturedSeed = entry.seedData;
                listItem.Setup(entry, () =>
                {
                    HideInventoryList();
                    onSeedSelected?.Invoke(capturedSeed);
                });
                _listItems.Add(listItem);
            }
        }

        _listIndex  = 0;
        _listActive = true;

        MoveToFirstAvailable();
        inventoryList.SetActive(true);
        PlayerStateManager.SetState(PlayerState.Dialogue);
    }

    public void HideInventoryList()
    {
        _listActive = false;
        _listItems.Clear();
        if (inventoryList != null) inventoryList.SetActive(false);
        TryResumeGame();
    }

    private void HandleListNavigation()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            _listIndex = Mathf.Min(_listIndex + 1, _listItems.Count - 1);
            HighlightList();
        }
        else if (Keyboard.current.wKey.wasPressedThisFrame ||
                 Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            _listIndex = Mathf.Max(_listIndex - 1, 0);
            HighlightList();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (_listIndex >= 0 && _listIndex < _listItems.Count)
            {
                SeedListItem selected = _listItems[_listIndex];
                if (selected.HasStock)
                    selected.SelectItem();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            HideInventoryList();
    }

    private void HighlightList()
    {
        for (int i = 0; i < _listItems.Count; i++)
            SetButtonColor(_listItems[i].selectButton, i == _listIndex ? highlightColor : normalColor);
    }

    private void MoveToFirstAvailable()
    {
        for (int i = 0; i < _listItems.Count; i++)
        {
            if (_listItems[i].HasStock)
            {
                _listIndex = i;
                break;
            }
        }
        HighlightList();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    /// <summary>
    /// Only resumes the game if NO panels are currently open.
    /// This prevents freeing the player mid-flow when one panel
    /// closes and another is about to open.
    /// </summary>
    private void TryResumeGame()
    {
        bool yesNoOpen = yesNoDialogue != null && yesNoDialogue.activeSelf;
        bool listOpen  = inventoryList != null && inventoryList.activeSelf;

        if (!yesNoOpen && !listOpen)
            PlayerStateManager.SetState(PlayerState.Free);
    }

    private void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        ColorBlock cb  = button.colors;
        cb.normalColor = color;
        button.colors  = cb;
    }
}
