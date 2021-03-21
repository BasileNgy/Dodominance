using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]

public enum GearType
{
    Consumable,
    Weapon
}
public class GearController : MonoBehaviour
{
    //Scene references
    public GearController me;
    public GameManager gameManager;
    public Camera fpsCam;
    public AudioManager audioManager;
    public SlotsController slotsController;


    //Gear Type

    public GearType gearType { get; protected set; }

    public void ResetGear()
    {
        if (gearType == GearType.Weapon)
        {
            Weapon asWeapon = me as Weapon;
            asWeapon.munitions = asWeapon.capacity;
            asWeapon.currentRechargeTime = asWeapon.initialRechargeTime;
            asWeapon.currentBulletPerSec = asWeapon.initialBulletPerSec;
            asWeapon.currentDamage = asWeapon.initialDamage;
        }
        else if (gearType == GearType.Consumable)
        {
            Debug.Log("Reset consumable " + gameObject.name);
        }
    }
}
