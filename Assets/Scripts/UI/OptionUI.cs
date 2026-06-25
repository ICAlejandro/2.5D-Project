using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public Color highlightColor = Color.yellow;
    public Color normalColor    = Color.white;

    private Action _onYes;
    private Action _onNo;

    private int  _yesNoIndex  = 0;
    private bool _yesNoActive = false;

    private List<SeedListItem> _listItems = new List<SeedListItem>();
    private int  _listIndex  = 0;
    private bool _listActive = false;

    public bool IsMenuOpen => _yesNoActive || _listActive;

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

    public void ShowYesNo(Action onYes, Action onNo = null)
    {
        _onYes       = onYes;
        _onNo        = onNo;
        _yesNoIndex  = 0;
        _yesNoActive = true;
        _listActive  = false;

        if (yesNoDialogue != null) yesNoDialogue.SetActive(true);
        if (inventoryList != null) inventoryList.SetActive(false);

        HighlightYesNo();
    }

    private void HandleYesNoNavigation()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame ||
            Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            _yesNoIndex = 0;
            HighlightYesNo();
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame ||
                 Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            _yesNoIndex = 1;
            HighlightYesNo();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (_yesNoIndex == 0) OnYesClicked();
            else OnNoClicked();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnNoClicked();
    }

    private void HighlightYesNo()
    {
        SetButtonColor(yesButton, _yesNoIndex == 0 ? highlightColor : normalColor);
        SetButtonColor(noButton, _yesNoIndex == 1 ? highlightColor : normalColor);
    }

    private void OnYesClicked()
    {
        _yesNoActive = false;
        if (yesNoDialogue != null) yesNoDialogue.SetActive(false);

        _onYes?.Invoke();
    }

    private void OnNoClicked()
    {
        HideAll();
        _onNo?.Invoke();
    }

    public void ShowInventoryList(List<SeedInventoryEntry> seeds, Action<SeedData> onSeedSelected)
    {
        _yesNoActive = false;
        if (yesNoDialogue != null) yesNoDialogue.SetActive(false);

        _listActive = true;
        if (inventoryList != null) inventoryList.SetActive(true);

        foreach (var item in _listItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _listItems.Clear();

        if (seeds == null || seeds.Count == 0)
        {
            Debug.Log("Inventory collection holds zero entries.");
            HideAll();
            return;
        }

        foreach (var entry in seeds)
        {
            if (seedListItemPrefab != null && inventoryListContainer != null)
            {
                GameObject obj = Instantiate(seedListItemPrefab, inventoryListContainer);
                SeedListItem listItem = obj.GetComponent<SeedListItem>();
                
                if (listItem != null)
                {
                    listItem.Setup(entry, () =>
                    {
                        HideAll();
                        onSeedSelected?.Invoke(entry.seedData);
                    });
                    _listItems.Add(listItem);
                }
            }
        }

        _listIndex = 0;
        MoveToFirstAvailable();
    }

    private void HandleListNavigation()
    {
        if (Keyboard.current == null || _listItems.Count == 0) return;

        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            _listIndex = Mathf.Min(_listIndex + 1, _listItems.Count - 1);
            HighlightList();
        }
        else if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
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
                {
                    if (selected.selectButton != null)
                        selected.selectButton.onClick.Invoke();
                }
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            HideAll();
    }

    private void HighlightList()
    {
        for (int i = 0; i < _listItems.Count; i++)
        {
            if (_listItems[i] != null)
                SetButtonColor(_listItems[i].selectButton, i == _listIndex ? highlightColor : normalColor);
        }
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

    private void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        ColorBlock cb   = button.colors;
        cb.normalColor  = color;
        cb.selectedColor = color;
        button.colors   = cb;
    }

    public void HideAll()
    {
        _yesNoActive = false;
        _listActive  = false;
        
        if (yesNoDialogue != null) yesNoDialogue.SetActive(false);
        if (inventoryList != null) inventoryList.SetActive(false);

        PlayerStateManager.SetState(PlayerState.Free);

        PlayerAction playerAction = FindFirstObjectByType<PlayerAction>();
        if (playerAction != null)
        {
            playerAction.CloseDialogue();
        }
    }
}