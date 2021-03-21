using UnityEngine;


public enum WeaponType
{
    NormalGun,
    UziGun,
    ShotGun
}
public class Weapon : GearController
{

    //Scene reference
    public LayerMask targetsMask;
    public ParticleSystem sparkle;
    public GameObject impactEffect;
    public RaycastHit rayHit;

    // Weapon Stats
    //public bool isMeleeWeapon;

    public WeaponType mType;
    public float initialDamage = 20;
    public float currentDamage;

    public float initialBulletPerSec = 2;
    public float currentBulletPerSec;
    public float nextTimeToFire = 0f;

    public float range = 100f;

    public float initialRechargeTime = 2.5f;
    public float currentRechargeTime;
    public bool reloading;

    public int munitions = 0;
    public int capacity = 15;

    public bool isAutomaticWeapon;
    public bool autoModeTriggered;
    void Awake()
    {
        gearType = GearType.Weapon;
    }

   private void Update()
    {
        if (!gameManager.gameIsPaused)
        {
            if (Input.GetMouseButtonDown(2) && isAutomaticWeapon)
            {
                if (!autoModeTriggered)
                {
                    Debug.Log("AutoMode Triggered");
                    autoModeTriggered = true;
                }
                else
                {
                    Debug.Log("AutoMode Disabled");
                    autoModeTriggered = false;

                }
            }

            if (autoModeTriggered && Input.GetMouseButton(0) && !reloading && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / currentBulletPerSec;
                Shoot();

            }
            else if (!autoModeTriggered && Input.GetMouseButtonDown(0) && !reloading && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / currentBulletPerSec;
                Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R) && !reloading && munitions < capacity)
            {
                Reload();
            }

        }
    }

    private void LateUpdate()
    {

        if(munitions == 0 && !reloading && !gameManager.gameIsPaused)
        {
            Reload();
        }
        
    }

    public void Shoot()
    {
        sparkle.Play();
        audioManager.Play("Gunshot");
        munitions--;
        gameManager.textMunitions.SetMyText(munitions.ToString());

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out rayHit, range, targetsMask))
        {
            GameObject targetObject = rayHit.transform.gameObject;
            if (targetObject == null)
            {
                return;
            }
            GameObject impact = Instantiate(impactEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            Destroy(impact, 1f);
            if (targetObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return;
            }
            else if(targetObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                EnemyStats target = targetObject.GetComponent<EnemyStats>();

                if (target != null && !target.isDead)
                {
                    target.ApplyDamage(currentDamage);
                    gameManager.CreateFloatingText(((int)currentDamage).ToString(), rayHit.point);
                }
                return;

            }
            else
            {
                NPC target = targetObject.GetComponent<NPC>();
                if (target != null && target.mType == NPCType.ShopKeeper)
                {
                    target.GetComponent<ShopKeeperAI>().enabled = true;
                    target.gameObject.layer = LayerMask.NameToLayer("Enemies");
                    target.enabled = false;
                }
                return;
            }
        }
    }
    void Reload()
    {
        reloading = true;
        audioManager.Play("Reloading");
        Debug.Log("Reloading "+gameObject.name+" ...");
        Invoke("ReloadFinished", currentRechargeTime);

    }

    void ReloadFinished()
    {

        audioManager.Play("ReloadFinished");
        Debug.Log("Reload finished");
        munitions = capacity;
        gameManager.textMunitions.SetMyText(munitions.ToString());
        reloading = false;
    }
    public void SelfDestroyW()
    {
        if (slotsController.activeWeapon == this)
        {
            gameManager.HideHUDGear();
            slotsController.activeWeapon = null;
            slotsController.activeSlot.gear = null;
            slotsController.activeSlot.slotEmpty = true;
            slotsController.activeSlot.SetName("Empty Slot");
        }
        Debug.Log("Weapon done" + name);
        gameManager.gearsInGame.Remove(this);
        Destroy(gameObject);
    }

}