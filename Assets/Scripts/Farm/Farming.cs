using UnityEngine;

public class Farming : Interactable
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public float timeToGrow = 1.0f; // In fractional days (e.g., 1.0 = 1 full in-game day)
    public int cropYieldAmount = 1; 
    public bool isWatered = false;

    [Header("Current Seed Data")]
    [Tooltip("Tracks what seed type is currently planted here.")]
    public string activeSeedName = "";

    [Header("Visual Component Links")]
    public SpriteRenderer plantSpriteRenderer;
    public Sprite plantSampleSprite; // Replace or expand for multi-seed visuals later
    public Animator plantAnimator;
    
    [Header("3D Soil Visuals")]
    public MeshRenderer soilMeshRenderer;
    public Material drySoilMaterial;
    
    private Material wetSoilMaterial; 
    private float growthTimer = 0f;
    private TimeManager timeManager;
    private float lastTimeOfDay;

    void Start()
    {
        if (soilMeshRenderer != null)
        {
            wetSoilMaterial = soilMeshRenderer.material;
        }

        timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager != null)
        {
            lastTimeOfDay = timeManager.currentTimeOfDay;
        }

        RefreshAllVisualsAndDialogue();
    }

    void Update()
    {
        HandleGrowthSimulation();
    }

    // Overridden bridge execution method triggered by PlayerAction.cs
    public override void Interact(GameObject playerObject)
    {
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        PlayerAction playerAction = playerObject.GetComponent<PlayerAction>();
        
        if (inventory == null || playerAction == null) return;

        // STEP 1: If the plant is fully grown, harvest it immediately
        if (currentStage == GrowthStage.ReadyToHarvest)
        {
            inventory.AddCrop(cropYieldAmount);
            Debug.Log($"Harvested {cropYieldAmount} {activeSeedName} crop(s)!");
            
            // Reset pot back to an empty slate
            activeSeedName = "";
            TransitionToStage(GrowthStage.Empty);
            return;
        }

        // STEP 2: If a seed is currently growing
        if (currentStage == GrowthStage.Growing)
        {
            // Case A: If it's dry, water it
            if (!isWatered)
            {
                WaterCrop();
                playerAction.DisplayDialogue($"You watered the thirsty {activeSeedName} seed!");
            }
            // Case B: If it's already watered, display its current status report
            else
            {
                float progressPercent = (growthTimer / timeToGrow) * 100f;
                float daysRemaining = Mathf.Max(0f, timeToGrow - growthTimer);
                
                // Formats status to show current progress cleanly to the player
                string statusReport = $"This {activeSeedName} plant is growing happily! " +
                                       $"\nProgress: {progressPercent:F0}% filled. " +
                                       $"\nEstimated time remaining: {daysRemaining:F1} in-game days.";
                                       
                playerAction.DisplayDialogue(statusReport);
            }
            return;
        }

        // STEP 3: If the pot is completely empty, handle the planting logic sequence
        if (currentStage == GrowthStage.Empty)
        {
            // For now, checks if player has standard seeds. 
            // (You can expand this check later if your inventory system uses multiple item names/IDs!)
            if (inventory.seedCount > 0)
            {
                inventory.UseSeed();
                activeSeedName = "Standard Seed"; // Tag the seed type to this pot instance
                PlantSeed();
                
                playerAction.DisplayDialogue($"You planted a {activeSeedName} into the soil!");
            }
            else
            {
                // Fallback warning text if player interacts completely empty handed
                playerAction.DisplayDialogue("You don't have any seeds in your inventory to plant here!");
            }
        }
    }

    private void HandleGrowthSimulation()
    {
        if (currentStage == GrowthStage.Growing && isWatered)
        {
            if (timeManager != null)
            {
                float currentTime = timeManager.currentTimeOfDay;
                float timeDelta = currentTime - lastTimeOfDay;

                // Handle clock loop midnight rollover wrapping boundary seamlessly
                if (timeDelta < 0) timeDelta += 1f;

                growthTimer += timeDelta;
                lastTimeOfDay = currentTime;
            }
            else
            {
                growthTimer += Time.deltaTime / 60f; 
            }

            if (growthTimer >= timeToGrow)
            {
                TransitionToStage(GrowthStage.ReadyToHarvest);
            }
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
        isWatered = false; 
        if (timeManager != null) lastTimeOfDay = timeManager.currentTimeOfDay;
        TransitionToStage(GrowthStage.Growing);
    }

    void WaterCrop()
    {
        isWatered = true;
        if (timeManager != null) lastTimeOfDay = timeManager.currentTimeOfDay;
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
        {
            soilMeshRenderer.material = isWatered ? wetSoilMaterial : drySoilMaterial;
        }

        if (plantAnimator != null) plantAnimator.SetInteger("GrowthStage", (int)currentStage);

        // Sets up basic dynamic dialogue values as fallback backup variables
        switch (currentStage)
        {
            case GrowthStage.Empty: dialogueText = "This pot is ready for seeds! Press Enter to plant."; break;
            case GrowthStage.Growing: dialogueText = isWatered ? "The plant is growing happily... Check back soon." : "The seed needs water! Press Enter to water it."; break;
            case GrowthStage.ReadyToHarvest: dialogueText = "The crop is fully grown! Press Enter to harvest."; break;
        }
    }
}