using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag your InputReader ScriptableObject asset here.")]
    public InputReader inputReader;

    [Header("YesNo Panel")]
    public GameObject yesNoPanel;
    public Button     yesButton;
    public Button     noButton;

    [Header("Inventory List Panel")]
    public GameObject inventoryListPanel;
    public Transform  inventoryListContainer;
    public GameObject seedListItemPrefab;

    [Header("Navigation Highlight")]
    public Color highlightColor = Color.yellow;
    public Color normalColor    = Color.white;

    private Action _onYes;
    private Action _onNo;
    private int    _yesNoIndex  = 0;
    private bool   _yesNoActive = false;

    private List<SeedListItem> _listItems = new List<SeedListItem>();
    private int  _listIndex  = 0;
    private bool _listActive = false;

    public bool IsMenuOpen => _yesNoActive || _listActive;

    void Start()
    {
        if (yesNoPanel       != null) yesNoPanel.SetActive(false);
        if (inventoryListPanel != null) inventoryListPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(ConfirmYes);
        if (noButton  != null) noButton.onClick.AddListener(ConfirmNo);

        if (inputReader != null)
        {
            inputReader.OnNavigateEvent += HandleNavigate;
            inputReader.OnConfirmEvent  += HandleConfirm;
            inputReader.OnCancelEvent   += HandleCancel;
        }
    }

    void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.OnNavigateEvent -= HandleNavigate;
            inputReader.OnConfirmEvent  -= HandleConfirm;
            inputReader.OnCancelEvent   -= HandleCancel;
        }
    }

    public void ShowYesNo(Action onYes, Action onNo = null)
    {
        _onYes       = onYes;
        _onNo        = onNo;
        _yesNoIndex  = 0;
        _yesNoActive = true;
        _listActive  = false;

        if (yesNoPanel        != null) yesNoPanel.SetActive(true);
        if (inventoryListPanel != null) inventoryListPanel.SetActive(false);

        RefreshYesNoHighlight();
    }

    public void ShowInventoryList(List<SeedInventoryEntry> seeds, Action<SeedData> onSelected)
    {
        _yesNoActive = false;
        _listActive  = true;

        if (yesNoPanel        != null) yesNoPanel.SetActive(false);
        if (inventoryListPanel != null) inventoryListPanel.SetActive(true);

        foreach (var item in _listItems)
            if (item != null) Destroy(item.gameObject);
        _listItems.Clear();

        if (seeds == null || seeds.Count == 0)
        {
            HidePanels();
            return;
        }

        foreach (var entry in seeds)
        {
            if (seedListItemPrefab == null || inventoryListContainer == null) continue;

            GameObject   obj  = Instantiate(seedListItemPrefab, inventoryListContainer);
            SeedListItem item = obj.GetComponent<SeedListItem>();

            if (item != null)
            {
                item.Setup(entry, () =>
                {
                    HidePanels();
                    onSelected?.Invoke(entry.seedData);
                });
                _listItems.Add(item);
            }
        }

        _listIndex = 0;
        MoveToFirstAvailable();
    }

    public void HidePanels()
    {
        _yesNoActive = false;
        _listActive  = false;

        if (yesNoPanel        != null) yesNoPanel.SetActive(false);
        if (inventoryListPanel != null) inventoryListPanel.SetActive(false);
    }

    private void HandleNavigate(Vector2 dir)
    {
        if (_yesNoActive) NavigateYesNo(dir);
        if (_listActive)  NavigateList(dir);
    }

    private void HandleConfirm()
    {
        if (_yesNoActive)
        {
            if (_yesNoIndex == 0) ConfirmYes();
            else                  ConfirmNo();
        }
        else if (_listActive)
        {
            if (_listIndex >= 0 && _listIndex < _listItems.Count)
            {
                SeedListItem sel = _listItems[_listIndex];
                if (sel.HasStock && sel.selectButton != null)
                    sel.selectButton.onClick.Invoke();
            }
        }
    }

    private void HandleCancel()
    {
        if (_yesNoActive) ConfirmNo();
        else if (_listActive) HidePanels();
    }

    private void NavigateYesNo(Vector2 dir)
    {
        if (dir.x < -0.1f || dir.y > 0.1f)       _yesNoIndex = 0;
        else if (dir.x > 0.1f || dir.y < -0.1f)  _yesNoIndex = 1;
        RefreshYesNoHighlight();
    }

    private void ConfirmYes()
    {
        _yesNoActive = false;
        if (yesNoPanel != null) yesNoPanel.SetActive(false);
        _onYes?.Invoke();
    }

    private void ConfirmNo()
    {
        HidePanels();
        _onNo?.Invoke();
    }

    private void NavigateList(Vector2 dir)
    {
        if (_listItems.Count == 0) return;

        if (dir.y < -0.1f)      _listIndex = Mathf.Min(_listIndex + 1, _listItems.Count - 1);
        else if (dir.y > 0.1f)  _listIndex = Mathf.Max(_listIndex - 1, 0);

        RefreshListHighlight();
    }

    private void RefreshYesNoHighlight()
    {
        SetButtonColor(yesButton, _yesNoIndex == 0 ? highlightColor : normalColor);
        SetButtonColor(noButton,  _yesNoIndex == 1 ? highlightColor : normalColor);
    }

    private void RefreshListHighlight()
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
            if (_listItems[i].HasStock) { _listIndex = i; break; }
        }
        RefreshListHighlight();
    }

    private void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        ColorBlock cb    = button.colors;
        cb.normalColor   = color;
        cb.selectedColor = color;
        button.colors    = cb;
    }
}
