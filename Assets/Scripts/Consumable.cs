using UnityEngine;

public enum ConsumableType
{
    HealingPotion
}
public class Consumable : GearController
{

    public ConsumableType mType;
    public PlayerController playerController;

    //Consumable Stats
    public int consumablesRemaining;
    void Awake()
    {
        gearType = GearType.Consumable;
    }

    private void Update()
    {
        if (!gameManager.gameIsPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (consumablesRemaining > 0)
                {
                    ApplyEffect();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if(consumablesRemaining <= 0 && !gameManager.gameIsPaused)
        {
            SelfDestroyC();
        }
    }

    private void ApplyEffect()
    {
        if(mType == ConsumableType.HealingPotion && playerController.currentHealth < playerController.maxHealth)
        {
            consumablesRemaining--;
            playerController.playerGainHealth(20);
            gameManager.textConsumablesRemaining.SetMyText(consumablesRemaining.ToString());
        }
    }

    public void SelfDestroyC()
    {
        if(slotsController.activeConsumable == this)
        {
            gameManager.HideHUDGear();
            slotsController.activeConsumable = null;
            slotsController.activeSlot.gear = null;
            slotsController.activeSlot.slotEmpty = true;
            slotsController.activeSlot.SetName("Empty Slot");
        }
        Debug.Log("Consumable done" + name);
        gameManager.gearsInGame.Remove(this);
        Destroy(gameObject);

    }
}
