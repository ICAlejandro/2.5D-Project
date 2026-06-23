using UnityEngine;

public class Farming : MonoBehaviour
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
    private Interactable interactableComponent;
    private float growthTimer = 0f;

    private TimeManager timeManager;
    private float lastTimeOfDay;

    #region Unity Lifecycle
    void Start()
    {
        InitializeComponents();
        RefreshAllVisualsAndDialogue();
    }

    void Update()
    {
        HandleGrowthSimulation();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        interactableComponent = GetComponent<Interactable>();
        
        if (soilMeshRenderer != null)
        {
            wetSoilMaterial = soilMeshRenderer.material;
        }

        timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager != null)
        {
            lastTimeOfDay = timeManager.currentTimeOfDay;
        }
    }
    #endregion

    #region Core Simulation Logic
    private void HandleGrowthSimulation()
    {
        if (currentStage == GrowthStage.Growing && isWatered)
        {
            if (timeManager != null)
            {
                float currentTime = timeManager.currentTimeOfDay;
                float timeDelta = currentTime - lastTimeOfDay;

                if (timeDelta < 0)
                {
                    timeDelta += 1f;
                }

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
        RefreshAllVisualsAndDialogue();
    }
    #endregion

    #region Interaction Handling
    public void Interact(GameObject playerObject)
    {
        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        switch (currentStage)
        {
            case GrowthStage.Empty:
                HandlePlantingAttempt(inventory);
                break;

            case GrowthStage.Growing:
                HandleWateringAttempt();
                break;

            case GrowthStage.ReadyToHarvest:
                HandleHarvestAttempt(playerObject, inventory);
                break;
        }
    }

    private void HandlePlantingAttempt(PlayerInventory inventory)
    {
        if (inventory.seedCount > 0)
        {
            inventory.UseSeed();
            PlantSeed();
        }
        else
        {
            UpdateDialogueText("You don't have any seeds! Go find some.");
        }
    }

    private void HandleWateringAttempt()
    {
        if (!isWatered)
        {
            WaterCrop();
        }
    }

    private void HandleHarvestAttempt(GameObject playerObject, PlayerInventory inventory)
    {
        HarvestPlant(playerObject);
        UpdateDialogueText("Harvested! Crops in Bag: " + inventory.cropCount);
    }
    #endregion

    #region Action Implementations
    void PlantSeed()
    {
        growthTimer = 0f;
        isWatered = false; 

        if (timeManager != null)
        {
            lastTimeOfDay = timeManager.currentTimeOfDay;
        }

        TransitionToStage(GrowthStage.Growing);
    }

    void WaterCrop()
    {
        isWatered = true;
        
        if (timeManager != null)
        {
            lastTimeOfDay = timeManager.currentTimeOfDay;
        }

        UpdateSoilVisual();
        UpdateStatusAndDialogue();
        Debug.Log("Crop watered! It is now growing.");
    }

    void HarvestPlant(GameObject playerObject)
    {
        isWatered = false; 

        if (playerObject != null)
        {
            PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddCrop(cropYieldAmount);
            }
        }

        TransitionToStage(GrowthStage.Empty);
    }
    #endregion

    #region Visual & UI Updates
    private void RefreshAllVisualsAndDialogue()
    {
        UpdatePlantVisibility();
        UpdateSoilVisual();
        UpdateStatusAndDialogue();
        UpdateAnimatorState();
    }

    private void UpdatePlantVisibility()
    {
        if (plantSpriteRenderer == null) return;

        bool shouldBeVisible = (currentStage != GrowthStage.Empty);
        plantSpriteRenderer.gameObject.SetActive(shouldBeVisible);

        if (shouldBeVisible)
        {
            plantSpriteRenderer.sprite = plantSampleSprite;
        }
    }

    void UpdateSoilVisual()
    {
        if (soilMeshRenderer == null) return;

        if (isWatered)
        {
            if (wetSoilMaterial != null) soilMeshRenderer.material = wetSoilMaterial;
        }
        else
        {
            if (drySoilMaterial != null) soilMeshRenderer.material = drySoilMaterial;
        }
    }

    void UpdateStatusAndDialogue()
    {
        if (interactableComponent == null) return;

        switch (currentStage)
        {
            case GrowthStage.Empty:
                UpdateDialogueText("This pot is ready for seeds! Press Enter to plant.");
                break;
            case GrowthStage.Growing:
                UpdateDialogueText(isWatered 
                    ? "The plant is growing happily... Check back soon." 
                    : "The seed needs water! Press Enter to water it.");
                break;
            case GrowthStage.ReadyToHarvest:
                UpdateDialogueText("The crop is fully grown! Press Enter to harvest.");
                break;
        }
    }

    void UpdateAnimatorState()
    {
        if (plantAnimator == null) return;
        plantAnimator.SetInteger("GrowthStage", (int)currentStage);
    }

    private void UpdateDialogueText(string text)
    {
        if (interactableComponent != null)
        {
            interactableComponent.dialogueText = text;
        }
    }
    #endregion
}