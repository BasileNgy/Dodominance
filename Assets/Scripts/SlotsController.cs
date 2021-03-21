using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotsController : MonoBehaviour
{
    public GameManager gameManager;
    public int slotIndex;
    public Slot activeSlot;
    public Consumable activeConsumable;
    public Weapon activeWeapon;
    public List<Slot> slots;
    public GameObject gearsInLevel;
    public List<Arsenal> arsenals;
    public List<string> arsenalsLootable;

    private void Update()
    {
        if (!gameManager.gameIsPaused)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f )
            {
                if (activeWeapon != null && activeWeapon.reloading)
                {
                    return;
                }
                DecreaseIndex();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (activeWeapon != null && activeWeapon.reloading)
                {
                    return;
                }
                IncreaseIndex();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                lootArsenal();
            }
        }

    }

    public void IncreaseIndex()
    {
        if(slotIndex >= slots.Count - 1)
        {
            slotIndex = 0;
            SetSlotActive(slotIndex);
        } else
        {
            slotIndex++;
            SetSlotActive(slotIndex);
        }
    }
    public void DecreaseIndex()
    {
        if (slotIndex <= 0)
        {
            slotIndex = slots.Count -1;
            SetSlotActive(slotIndex);
        }
        else
        {
            slotIndex--;
            SetSlotActive(slotIndex);
        }

    }

    void CheckActiveGear()
    {
        if (activeSlot.gear != null)
        {
            if (activeWeapon != null)
            {
                ResetGearStats();
                activeWeapon = null;
            }
            if (activeConsumable != null)
            {
                activeConsumable = null;
            }
            activeSlot.gear.gameObject.SetActive(false);
        }

    }

    void SetSlotActive(int index)
    {
        CheckActiveGear();
        activeSlot = slots[index];
        if (activeSlot.gear != null)
        {

            activeSlot.gear.gameObject.SetActive(true);
            activeSlot.gear.gameObject.GetComponent<GearController>().enabled = true;
            if(activeSlot.gear.gearType == GearType.Weapon)
            {
                activeWeapon = activeSlot.gear as Weapon;
                activeConsumable = null;
                ApplyArsenalToGear();
                gameManager.ActivateHudGear(activeWeapon);

            } else if (activeSlot.gear.gearType == GearType.Consumable)
            {
                activeConsumable = activeSlot.gear as Consumable;
                activeWeapon = null;
                gameManager.ActivateHudGear(activeConsumable);
            }

        }
        else
        {
            gameManager.HideHUDGear();
        }
    }

    public void DropWeapon()
    {
        if(activeSlot.slotEmpty || activeSlot.gear == null)
        {
            Debug.Log("No gear to drop");
            return ;
        }
        if (activeWeapon != null)
        {
            ResetGearStats();
            activeWeapon = null;

        }
        if(activeConsumable != null)
        {
            activeConsumable = null;
        }
        activeSlot.gear.gameObject.transform.parent = gearsInLevel.transform;
        activeSlot.gear.gameObject.GetComponent<GearController>().enabled = false;
        activeSlot.gear = null;
        activeSlot.slotEmpty = true;
        activeSlot.SetName("Empty Slot");
        gameManager.HideHUDGear();
    }

    public void LootGear(GearController gearToLoot, GearType gearType)
    {
        if(gearToLoot.gearType == GearType.Consumable)
        {
            Consumable consumableToLoot = gearToLoot as Consumable; 
            foreach (Slot s in slots)
            {
                if (s.gear != null && s.gear.gearType == GearType.Consumable && s.gear.name == consumableToLoot.name)
                {
                    Consumable updatedConsumable = s.gear as Consumable;
                    updatedConsumable.consumablesRemaining += consumableToLoot.consumablesRemaining;
                    consumableToLoot.SelfDestroyC();
                    if (s == activeSlot)
                    {
                        gameManager.ActivateHudGear(activeSlot.gear);
                    }
                    
                    return;
                }
            }
        }
        Slot lootingSlot = null;

        if (activeSlot.slotEmpty)
        {
            lootingSlot = activeSlot;
        }

        foreach (Slot s in slots)
        {
            if(lootingSlot != null || !s.slotEmpty)
            {
                continue;
            }
            lootingSlot = s;
        }
        if (lootingSlot == null && !activeSlot.slotEmpty)
        {
            Debug.Log("Droping " + activeSlot.gear.name);
            DropWeapon();
            lootingSlot = activeSlot;
        }


        gearToLoot.transform.parent = lootingSlot.slot.transform;
        lootingSlot.gear = gearToLoot;
        lootingSlot.gear.transform.position = lootingSlot.slot.transform.position;
        if(gearToLoot.name == "HealingPotion")
        {
            lootingSlot.gear.transform.rotation = Quaternion.Euler(Vector3.zero);

        } else
        {
            lootingSlot.gear.transform.rotation = lootingSlot.slot.transform.rotation;
        }
        lootingSlot.SetName(gearToLoot.name);
        lootingSlot.slotEmpty = false;
        if(lootingSlot == activeSlot)
        {
            lootingSlot.gear.gameObject.SetActive(true);
            lootingSlot.gear.gameObject.GetComponent<GearController>().enabled = true;

            if (gearToLoot.gearType == GearType.Weapon)
            {
                activeWeapon = gearToLoot as Weapon;
                activeConsumable = null;
                ApplyArsenalToGear();
            }
            else if (gearToLoot.gearType == GearType.Consumable)
            {
                activeConsumable = gearToLoot as Consumable;
                activeWeapon = null;
            }
            gameManager.ActivateHudGear(lootingSlot.gear);
        } else
        {
            lootingSlot.gear.gameObject.SetActive(false);
        }
    }

    public void lootArsenal()
    {
        if(arsenalsLootable.Count > 0)
        {
            string boost = arsenalsLootable[UnityEngine.Random.Range(0, arsenalsLootable.Count)];

            Arsenal a = arsenals.Find(arsenal => arsenal.myName == boost);

            if (a == null)
            {
                Debug.LogWarning("Could not find arsenal : " + boost);
                return;
            }

            switch (a.myName)
            {
                case "Damage":
                    a.totalEffect += a.effect;
                    if (a.totalEffect > 2)
                    {
                        a.totalEffect = 2;
                        arsenalsLootable.Remove(boost);
                    }
                    ApplyArsenalToGear();
                    break;
                case "RechargeTime":
                    a.totalEffect -= a.effect;
                    if (a.totalEffect < 0.5f)
                    {
                        a.totalEffect = 0.5f;
                        arsenalsLootable.Remove(boost);
                    }
                    ApplyArsenalToGear();
                    break;
                case "FireRate":
                    a.totalEffect += a.effect;
                    if (a.totalEffect > 2)
                    {
                        a.totalEffect = 2;
                        arsenalsLootable.Remove(boost);
                    }
                    ApplyArsenalToGear();
                    break;
                default:
                    Debug.LogWarning("Error for arsenal : " + boost); ;
                    break;
            }

        }
    }

    private void ResetGearStats()
    {
        activeWeapon.currentDamage = activeWeapon.initialDamage;
        activeWeapon.currentBulletPerSec = activeWeapon.initialBulletPerSec;
        activeWeapon.currentRechargeTime = activeWeapon.initialRechargeTime;

    }
    public void ApplyArsenalToGear()
    {
        activeWeapon.currentDamage = activeWeapon.initialDamage * arsenals.Find(arsenal => arsenal.myName == "Damage").totalEffect;
        activeWeapon.currentBulletPerSec = activeWeapon.initialBulletPerSec * arsenals.Find( arsenal => arsenal.myName == "FireRate").totalEffect;
        activeWeapon.currentRechargeTime = activeWeapon.initialRechargeTime * arsenals.Find(arsenal => arsenal.myName == "RechargeTime").totalEffect;

    }

}
