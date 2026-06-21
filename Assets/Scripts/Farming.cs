using UnityEngine;

public class Farming : MonoBehaviour
{
    public enum GrowthStage { Empty, Growing, ReadyToHarvest }

    [Header("Growth Settings")]
    public GrowthStage currentStage = GrowthStage.Empty;
    public float timeToGrow = 5.0f;
    public int cropValue = 10;

    [Header("2D Visual References")]
    public SpriteRenderer plant;
    public Sprite plantSampleSprite;
    public Animator plantAnimator;

    private Interactable interactableComponent;
    private float growthTimer = 0f;

    void Start()
    {
        interactableComponent = GetComponent<Interactable>();
        UpdateStatusAndDialogue();
        UpdateAnimatorState();
        
        if (plant != null && currentStage == GrowthStage.Empty)
        {
            plant.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentStage == GrowthStage.Growing)
        {
            growthTimer += Time.deltaTime;

            if (growthTimer >= timeToGrow)
            {
                currentStage = GrowthStage.ReadyToHarvest;
                UpdateStatusAndDialogue();
                UpdateAnimatorState();
            }
        }
    }

    public void Interact(GameObject playerObject)
    {
        switch (currentStage)
        {
            case GrowthStage.Empty:
                PlantSeed();
                break;

            case GrowthStage.ReadyToHarvest:
                HarvestPlant(playerObject);
                break;

            case GrowthStage.Growing:
                break;
        }
    }

    void PlantSeed()
    {
        currentStage = GrowthStage.Growing;
        growthTimer = 0f;

        if (plant != null)
        {
            plant.gameObject.SetActive(true);
            plant.sprite = plantSampleSprite;
        }

        UpdateStatusAndDialogue();
        UpdateAnimatorState();
    }

    void HarvestPlant(GameObject playerObject)
    {
        currentStage = GrowthStage.Empty;

        if (plant != null)
        {
            plant.gameObject.SetActive(false);
        }

        PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddCrop(cropValue);
        }

        UpdateStatusAndDialogue();
        UpdateAnimatorState();
        
        if (interactableComponent != null && inventory != null)
        {
            interactableComponent.dialogueText = "Harvested! Total Gold: " + inventory.goldCount;
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
                interactableComponent.dialogueText = "A seed is growing here! Check back in a few seconds.";
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