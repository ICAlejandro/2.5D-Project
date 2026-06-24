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
        IInventory   inventory    = playerObject.GetComponent<PlayerInventory>();
        PlayerAction playerAction = playerObject.GetComponent<PlayerAction>();

        if (inventory == null || playerAction == null) return;

        // ── Harvest ────────────────────────────────────────────────────────
        if (currentStage == GrowthStage.ReadyToHarvest)
        {
            inventory.AddCrop(activeSeed.cropYieldAmount);
            Debug.Log($"Harvested {activeSeed.cropYieldAmount} {activeSeed.seedName} crop(s)!");
            activeSeed = null;
            TransitionToStage(GrowthStage.Empty);
            return;
        }

        // ── Growing + watered → show status only ──────────────────────────
        if (currentStage == GrowthStage.Growing && isWatered)
        {
            float progressPercent = (growthTimer / activeSeed.daysToGrow) * 100f;
            float daysRemaining   = Mathf.Max(0f, activeSeed.daysToGrow - growthTimer);
            string status = $"Your {activeSeed.seedName} is growing happily!\n" +
                            $"Progress: {progressPercent:F0}%\n" +
                            $"Est. time remaining: {daysRemaining:F1} day(s).";
            playerAction.DisplayDialogue(status);
            return;
        }

        // ── Growing + not watered → dialogue, then yes/no ─────────────────
        if (currentStage == GrowthStage.Growing && !isWatered)
        {
            playerAction.DisplayDialogue(
                $"Your {activeSeed.seedName} is thirsty! Water it?",
                onClose: () =>
                {
                    optionUI.ShowYesNo(
                        onYes: () =>
                        {
                            WaterCrop();
                            playerAction.DisplayDialogue($"You watered the {activeSeed.seedName}!");
                        },
                        onNo: () => { }
                    );
                }
            );
            return;
        }

        // ── Empty → dialogue, then yes/no, then seed list ─────────────────
        if (currentStage == GrowthStage.Empty)
        {
            playerAction.DisplayDialogue(
                "This pot is empty. Would you like to plant a seed?",
                onClose: () =>
                {
                    optionUI.ShowYesNo(
                        onYes: () =>
                        {
                            optionUI.ShowInventoryList(inventory.SeedInventory, selectedSeed =>
                            {
                                if (inventory.UseSeed(selectedSeed))
                                {
                                    activeSeed = selectedSeed;
                                    PlantSeed();
                                    playerAction.DisplayDialogue($"You planted a {selectedSeed.seedName}!");
                                }
                            });
                        },
                        onNo: () => { }
                    );
                }
            );
        }
    }

    private void HandleGrowthSimulation()
    {
        if (currentStage == GrowthStage.Growing && isWatered && activeSeed != null)
        {
            if (timeProvider != null)
            {
                float currentTime = timeProvider.CurrentTimeOfDay;
                float timeDelta   = currentTime - lastTimeOfDay;
                if (timeDelta < 0) timeDelta += 1f;
                growthTimer  += timeDelta;
                lastTimeOfDay = currentTime;
            }
            else
            {
                growthTimer += Time.deltaTime / 60f;
            }

            if (growthTimer >= activeSeed.daysToGrow)
                TransitionToStage(GrowthStage.ReadyToHarvest);
        }
    }

    private void TransitionToStage(GrowthStage nextStage)
    {
        currentStage = nextStage;
        if (nextStage == GrowthStage.Empty) isWatered = false;
        RefreshVisuals();
    }

    void PlantSeed()
    {
        growthTimer = 0f;
        isWatered   = false;
        if (timeProvider != null) lastTimeOfDay = timeProvider.CurrentTimeOfDay;
        TransitionToStage(GrowthStage.Growing);
    }

    void WaterCrop()
    {
        isWatered = true;
        if (timeProvider != null) lastTimeOfDay = timeProvider.CurrentTimeOfDay;
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
