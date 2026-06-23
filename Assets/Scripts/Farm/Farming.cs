using UnityEngine;

public class Farming : Interactable
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public float timeToGrow = 1.0f;
    public int cropYieldAmount = 1;
    public bool isWatered = false;

    [Header("Current Seed Data")]
    [Tooltip("Tracks what seed type is currently planted here.")]
    public string activeSeedName = "";

    [Header("Visual Component Links")]
    public SpriteRenderer plantSpriteRenderer;
    public Sprite plantSampleSprite;
    public Animator plantAnimator;

    [Header("3D Soil Visuals")]
    public MeshRenderer soilMeshRenderer;
    public Material drySoilMaterial;

    private Material wetSoilMaterial;
    private float growthTimer = 0f;
    private ITimeProvider timeProvider;
    private float lastTimeOfDay;

    void Start()
    {
        if (soilMeshRenderer != null)
            wetSoilMaterial = soilMeshRenderer.material;

        timeProvider = ServiceLocator.Get<ITimeProvider>();
        if (timeProvider != null)
            lastTimeOfDay = timeProvider.CurrentTimeOfDay;

        RefreshAllVisualsAndDialogue();
    }

    void Update()
    {
        HandleGrowthSimulation();
    }

    public override void Interact(GameObject playerObject)
    {
        IInventory inventory   = playerObject.GetComponent<PlayerInventory>();
        PlayerAction playerAction = playerObject.GetComponent<PlayerAction>();

        if (inventory == null || playerAction == null) return;

        if (currentStage == GrowthStage.ReadyToHarvest)
        {
            inventory.AddCrop(cropYieldAmount);
            Debug.Log($"Harvested {cropYieldAmount} {activeSeedName} crop(s)!");
            activeSeedName = "";
            TransitionToStage(GrowthStage.Empty);
            return;
        }

        if (currentStage == GrowthStage.Growing)
        {
            if (!isWatered)
            {
                WaterCrop();
                playerAction.DisplayDialogue($"You watered the thirsty {activeSeedName} seed!");
            }
            else
            {
                float progressPercent = (growthTimer / timeToGrow) * 100f;
                float daysRemaining   = Mathf.Max(0f, timeToGrow - growthTimer);
                string statusReport   = $"This {activeSeedName} plant is growing happily! " +
                                        $"\nProgress: {progressPercent:F0}% filled. " +
                                        $"\nEstimated time remaining: {daysRemaining:F1} in-game days.";
                playerAction.DisplayDialogue(statusReport);
            }
            return;
        }

        if (currentStage == GrowthStage.Empty)
        {
            if (inventory.SeedCount > 0)
            {
                inventory.UseSeed();
                activeSeedName = "Standard Seed";
                PlantSeed();
                playerAction.DisplayDialogue($"You planted a {activeSeedName} into the soil!");
            }
            else
            {
                playerAction.DisplayDialogue("You don't have any seeds in your inventory to plant here!");
            }
        }
    }

    private void HandleGrowthSimulation()
    {
        if (currentStage == GrowthStage.Growing && isWatered)
        {
            if (timeProvider != null)
            {
                float currentTime = timeProvider.CurrentTimeOfDay;
                float timeDelta   = currentTime - lastTimeOfDay;
                if (timeDelta < 0) timeDelta += 1f;
                growthTimer   += timeDelta;
                lastTimeOfDay  = currentTime;
            }
            else
            {
                growthTimer += Time.deltaTime / 60f;
            }

            if (growthTimer >= timeToGrow)
                TransitionToStage(GrowthStage.ReadyToHarvest);
        }
    }

    private void TransitionToStage(GrowthStage nextStage)
    {
        currentStage = nextStage;
        if (nextStage == GrowthStage.Empty) isWatered = false;
        RefreshAllVisualsAndDialogue();
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
        RefreshAllVisualsAndDialogue();
    }

    private void RefreshAllVisualsAndDialogue()
    {
        if (plantSpriteRenderer != null)
        {
            bool visible = (currentStage != GrowthStage.Empty);
            plantSpriteRenderer.gameObject.SetActive(visible);
            if (visible) plantSpriteRenderer.sprite = plantSampleSprite;
        }

        if (soilMeshRenderer != null)
            soilMeshRenderer.material = isWatered ? wetSoilMaterial : drySoilMaterial;

        if (plantAnimator != null)
            plantAnimator.SetInteger("GrowthStage", (int)currentStage);

        switch (currentStage)
        {
            case GrowthStage.Empty:          dialogueText = "This pot is ready for seeds! Press Enter to plant."; break;
            case GrowthStage.Growing:        dialogueText = isWatered ? "The plant is growing happily... Check back soon." : "The seed needs water! Press Enter to water it."; break;
            case GrowthStage.ReadyToHarvest: dialogueText = "The crop is fully grown! Press Enter to harvest."; break;
        }
    }
}
