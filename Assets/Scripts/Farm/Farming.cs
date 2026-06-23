using UnityEngine;

public class Farming : Interactable
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public float timeToGrow = 1.0f; 
    public int cropYieldAmount = 1; 
    public bool isWatered = false;

    [Header("2D Visual References")]
    public SpriteRenderer plantSpriteRenderer;
    public Sprite plantSampleSprite;
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

    public override void Interact(GameObject playerObject)
    {
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        switch (currentStage)
        {
            case GrowthStage.Empty:
                if (inventory.seedCount > 0)
                {
                    inventory.UseSeed();
                    PlantSeed();
                }
                break;

            case GrowthStage.Growing:
                if (!isWatered) WaterCrop();
                break;

            case GrowthStage.ReadyToHarvest:
                inventory.AddCrop(cropYieldAmount);
                TransitionToStage(GrowthStage.Empty);
                break;
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

        switch (currentStage)
        {
            case GrowthStage.Empty: dialogueText = "This pot is ready for seeds! Press Enter to plant."; break;
            case GrowthStage.Growing: dialogueText = isWatered ? "The plant is growing happily... Check back soon." : "The seed needs water! Press Enter to water it."; break;
            case GrowthStage.ReadyToHarvest: dialogueText = "The crop is fully grown! Press Enter to harvest."; break;
        }
    }
}