using UnityEngine;

public class Farming : Interactable
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public bool isWatered = false;

    [Header("Visual Component Links")]
    public SpriteRenderer plantSpriteRenderer;
    public Animator       plantAnimator;

    [Header("3D Soil Visuals")]
    public MeshRenderer soilMeshRenderer;
    public Material     drySoilMaterial;

    [Header("UI Reference")]
    [Tooltip("Assign the OptionUI component from your OptionCanvas.")]
    public OptionUI optionUI;

    [Header("Farming Dialogue Settings (Editable in Inspector)")]
    [TextArea(2, 5)]
    [Tooltip("Text shown when the soil plot is completely empty. Prompting to plant a seed.")]
    public string emptyPlotPrompt = "It's empty, should I plant something?";

    [TextArea(2, 5)]
    [Tooltip("Text shown when a seed is planted but needs water. Use {0} as a placeholder for the seed name.")]
    public string waterPrompt = "Water the {0}?";

    [TextArea(2, 5)]
    [Tooltip("Text shown when interacting with a watered growing plant. Use {0} for name, {1} for progress %, and {2} for days left.")]
    public string growthStatusDialogue = "{0} is growing!\nProgress: {1:F0}%\nDays remaining: {2:F1}";

    [TextArea(2, 5)]
    [Tooltip("Dialogue displayed when a crop is successfully watered.")]
    public string waterSuccessDialogue = "You watered the soil patch.";

    [TextArea(2, 5)]
    [Tooltip("Dialogue displayed upon harvesting a mature crop. Use {0} for yield amount and {1} for the crop name.")]
    public string harvestSuccessDialogue = "Harvested {0} {1}(s)!";

    private SeedData      activeSeed;
    private Material      wetSoilMaterial;
    private float         growthTimer = 0f;
    private ITimeProvider timeProvider;
    private float         lastTimeOfDay;

    void Start()
    {
        if (soilMeshRenderer != null)
            wetSoilMaterial = soilMeshRenderer.material;

        timeProvider = ServiceLocator.Get<ITimeProvider>();
        if (timeProvider != null)
            lastTimeOfDay = timeProvider.CurrentTimeOfDay;

        if (optionUI == null)
            optionUI = FindFirstObjectByType<OptionUI>();

        RefreshVisuals();
    }

    void Update()
    {
        HandleGrowthSimulation();
    }

    public override void Interact(GameObject playerObject)
    {
        IInventory inventory = playerObject.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = ServiceLocator.Get<IInventory>();
        }
        if (inventory == null) return;

        PlayerAction playerAction = playerObject.GetComponent<PlayerAction>();

        // LAYER 1: Plot is completely empty
        if (currentStage == GrowthStage.Empty)
        {
            if (optionUI == null) 
                optionUI = FindFirstObjectByType<OptionUI>();

            if (playerAction != null)
            {
                playerAction.DisplayDialogue(emptyPlotPrompt);
            }

            if (optionUI != null)
            {
                PlayerStateManager.SetState(PlayerState.Dialogue);

                optionUI.ShowYesNo(
                    onYes: () =>
                    {
                        optionUI.ShowInventoryList(inventory.SeedInventory, (selectedSeed) =>
                        {
                            if (inventory.UseSeed(selectedSeed))
                            {
                                PlantSeed(selectedSeed);
                                if (playerAction != null) playerAction.CloseDialogue();
                            }
                            else
                            {
                                Debug.LogWarning("Failed to plant: Selected seed is out of stock!");
                                if (playerAction != null) playerAction.CloseDialogue();
                            }
                        });
                    },
                    onNo: () =>
                    {
                        if (playerAction != null) playerAction.CloseDialogue();
                    }
                );
            }
        }
        // LAYER 2: Plot is occupied by a growing plant
        else if (currentStage == GrowthStage.Growing)
        {
            if (!isWatered)
            {
                if (playerAction != null && activeSeed != null)
                {
                    string formattedPrompt = string.Format(waterPrompt, activeSeed.seedName);
                    playerAction.DisplayDialogue(formattedPrompt);
                }

                if (optionUI != null)
                {
                    PlayerStateManager.SetState(PlayerState.Dialogue);

                    optionUI.ShowYesNo(
                        onYes: () =>
                        {
                            WaterCrop();
                            // Close Option panels before sending text so the state settles perfectly
                            optionUI.HideAll(); 
                            if (playerAction != null) playerAction.DisplayDialogue(waterSuccessDialogue);
                        },
                        onNo: () =>
                        {
                            optionUI.HideAll();
                            if (playerAction != null) playerAction.CloseDialogue();
                        }
                    );
                }
            }
            else
            {
                if (playerAction != null && activeSeed != null) 
                {
                    float progress = (growthTimer / activeSeed.daysToGrow) * 100f;
                    float remaining = Mathf.Max(0f, activeSeed.daysToGrow - growthTimer);
                    
                    string formattedStatus = string.Format(growthStatusDialogue, activeSeed.seedName, progress, remaining);
                    playerAction.DisplayDialogue(formattedStatus);
                }
            }
        }
        // LAYER 3: Plot is ready to harvest
        else if (currentStage == GrowthStage.ReadyToHarvest)
        {
            if (activeSeed != null)
            {
                inventory.AddCrop(activeSeed.cropYieldAmount);
                if (playerAction != null) 
                {
                    string formattedHarvest = string.Format(harvestSuccessDialogue, activeSeed.cropYieldAmount, activeSeed.seedName);
                    playerAction.DisplayDialogue(formattedHarvest);
                }
            }
            
            activeSeed = null;
            TransitionToStage(GrowthStage.Empty);
        }
    }

    private void HandleGrowthSimulation()
    {
        if (currentStage != GrowthStage.Growing || !isWatered || activeSeed == null) return;

        if (timeProvider != null)
        {
            float now = timeProvider.CurrentTimeOfDay;
            float delta = now - lastTimeOfDay;
            if (delta < 0) delta += 1f;
            growthTimer  += delta;
            lastTimeOfDay = now;
        }
        else
        {
            growthTimer += Time.deltaTime / 60f;
        }

        if (growthTimer >= activeSeed.daysToGrow)
            TransitionToStage(GrowthStage.ReadyToHarvest);
    }

    private void TransitionToStage(GrowthStage nextStage)
    {
        currentStage = nextStage;
        if (nextStage == GrowthStage.Empty) isWatered = false;
        RefreshVisuals();
    }

    private void PlantSeed(SeedData seed)
    {
        activeSeed  = seed;
        growthTimer = 0f;
        isWatered   = false;
        
        if (timeProvider != null) 
            lastTimeOfDay = timeProvider.CurrentTimeOfDay;
            
        TransitionToStage(GrowthStage.Growing);
    }

    private void WaterCrop()
    {
        isWatered = true;
        if (timeProvider != null) 
            lastTimeOfDay = timeProvider.CurrentTimeOfDay;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (plantSpriteRenderer != null)
        {
            bool visible = currentStage != GrowthStage.Empty;
            plantSpriteRenderer.gameObject.SetActive(visible);
            if (visible && activeSeed != null)
                plantSpriteRenderer.sprite = activeSeed.plantSprite;
        }

        if (soilMeshRenderer != null)
            soilMeshRenderer.material = isWatered ? wetSoilMaterial : drySoilMaterial;

        if (plantAnimator != null)
            plantAnimator.SetInteger("GrowthStage", (int)currentStage);
    }
}