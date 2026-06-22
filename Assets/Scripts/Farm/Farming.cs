using UnityEngine;

public class Farming : MonoBehaviour
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public float timeToGrow = 5.0f;
    public int cropYieldAmount = 1; // Gives 1 physical crop item when harvested
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

    void Start()
    {
        interactableComponent = GetComponent<Interactable>();
        
        if (soilMeshRenderer != null)
        {
            wetSoilMaterial = soilMeshRenderer.material;
        }

        UpdateStatusAndDialogue();
        UpdateAnimatorState();
        UpdateSoilVisual();
        
        if (plantSpriteRenderer != null && currentStage == GrowthStage.Empty)
        {
            plantSpriteRenderer.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentStage == GrowthStage.Growing && isWatered)
        {
            growthTimer += Time.deltaTime;

            if (growthTimer >= timeToGrow)
            {
                currentStage = GrowthStage.ReadyToHarvest;
                UpdateSoilVisual();
                UpdateStatusAndDialogue();
                UpdateAnimatorState();
            }
        }
    }

    public void Interact(GameObject playerObject)
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
                else
                {
                    if (interactableComponent != null)
                    {
                        interactableComponent.dialogueText = "You don't have any seeds! Go find some.";
                    }
                }
                break;

            case GrowthStage.Growing:
                if (!isWatered)
                {
                    WaterCrop();
                }
                break;

            case GrowthStage.ReadyToHarvest:
                HarvestPlant(playerObject);
                break;
        }
    }

    void PlantSeed()
    {
        currentStage = GrowthStage.Growing;
        growthTimer = 0f;
        isWatered = false; 

        if (plantSpriteRenderer != null)
        {
            plantSpriteRenderer.gameObject.SetActive(true);
            plantSpriteRenderer.sprite = plantSampleSprite;
        }

        UpdateSoilVisual();
        UpdateStatusAndDialogue();
        UpdateAnimatorState();
    }

    void WaterCrop()
    {
        isWatered = true;
        UpdateSoilVisual();
        UpdateStatusAndDialogue();
        Debug.Log("Crop watered! It is now growing.");
    }

    void HarvestPlant(GameObject playerObject)
    {
        currentStage = GrowthStage.Empty;
        isWatered = false; 

        if (plantSpriteRenderer != null)
        {
            plantSpriteRenderer.gameObject.SetActive(false);
        }

        if (playerObject != null)
        {
            PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // Give the player physical crops instead of gold!
                inventory.AddCrop(cropYieldAmount);
            }
        }

        UpdateSoilVisual(); 
        UpdateStatusAndDialogue();
        UpdateAnimatorState();
        
        if (interactableComponent != null && playerObject != null)
        {
            PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                interactableComponent.dialogueText = "Harvested! Crops in Bag: " + inventory.cropCount;
            }
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
                interactableComponent.dialogueText = "This pot is ready for seeds! Press Enter to plant.";
                break;
            case GrowthStage.Growing:
                if (!isWatered)
                {
                    interactableComponent.dialogueText = "The seed needs water! Press Enter to water it.";
                }
                else
                {
                    interactableComponent.dialogueText = "The plant is growing happily... Check back soon.";
                }
                break;
            case GrowthStage.ReadyToHarvest:
                interactableComponent.dialogueText = "The crop is fully grown! Press Enter to harvest.";
                break;
        }
    }

    void UpdateAnimatorState()
    {
        if (plantAnimator == null) return;

        plantAnimator.SetInteger("GrowthStage", (int)currentStage);
    }
}