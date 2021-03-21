using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Scene objects
    private Camera playerCam;
    public GameObject gearsInLevel;

    public GameObject shopObject;
    public Vector3 initialGearPosition1, initialGearPosition2, initialGearPosition3, gearPosition1, gearPosition2, gearPosition3, gearPosition4, bossPandaPosition, bossMammothPosition;

    private LevelBuilder_2 levelBuilder_2;
    private LevelBuilder_3 levelBuilder_3;

    //HUD references
    private HUDBar barArmor;
    private HUDBar barHealth;

    private HUDText textArmor;
    private HUDText textHealth;
    private HUDText textMoney;


    private GameObject weaponHUD;
    public HUDText textMunitions;
    private HUDText textCapacity;
    private GameObject consumableHUD;
    public HUDText textConsumablesRemaining;

    private HUDText textSlot1;
    private HUDText textSlot2;
    private HUDText textSlot3;

    //Controllers
    private GameManager me;
    private PlayerController playerController;
    private CharacterController playerBody;
    private SlotsController slotsController;
    private CanvasManager canvasManager;
    private AudioManager audioManager;
    private DialogManager dialogManager;
    public List<ShopController> shopControllers;

    //Game entities
    private GameObject playerObject;
    private PlayerView playerView;
    public List<EnemyStats> enemies;
    public List<GearController> gearsInGame;
    public List<NPC> npcInGame;
    private GameObject gearSlot1;
    private GameObject gearSlot2;
    private GameObject gearSlot3;

    //Layers
    private LayerMask playerMask;
    private LayerMask tpMask;
    private LayerMask npcMask;
    private LayerMask gearMask;
    private LayerMask enemiesMask;
    private LayerMask groundMask;
    private RaycastHit gearRay;


    //Game stats
    private float gearLootRange = 2;
    private float dialogRange = 5;
    public bool gameIsPaused;
    private Vector3 playerSpawnPoint;

    //Teleporters
    private Transform tpToLevel1FromTuto;
    private Transform tpToTutoFromLevel1;

    public Transform tpToArena1FromLevel1;
    public Transform tpToLevel1FromArena1;

    private Transform tpToMarketFromArena1;
    private Transform tpToArena1FromMarket;

    public Transform tpToLevel2FromMarket;
    public Transform tpToMarketFromLevel2;

    public Transform tpToArena2FromLevel2;
    public Transform tpToLevel2FromArena2;


    private void Awake()
    {
        gameIsPaused = true;
        //Get controllers
        me = this;
        playerObject = GameObject.Find("FirstPersonPlayer");
        playerBody = playerObject.GetComponent<CharacterController>();
        playerController = playerObject.GetComponent<PlayerController>();
        playerView = playerObject.GetComponentInChildren<PlayerView>();
        audioManager = FindObjectOfType<AudioManager>();
        canvasManager = FindObjectOfType<CanvasManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        slotsController = FindObjectOfType<SlotsController>();

        //Get game objects
        gearsInLevel = GameObject.Find("GearsInLevel");
        shopObject = GameObject.Find("Shop");
        initialGearPosition1 = GameObject.Find("InitialGear1").transform.position;
        initialGearPosition2 = GameObject.Find("InitialGear2").transform.position;
        initialGearPosition3 = GameObject.Find("InitialGear3").transform.position;
        gearPosition1 = GameObject.Find("Gear1Position").transform.position;
        gearPosition2 = GameObject.Find("Gear2Position").transform.position;
        gearPosition3 = GameObject.Find("Gear3Position").transform.position;
        gearPosition4 = GameObject.Find("Gear4Position").transform.position;
        bossPandaPosition = GameObject.Find("BossPandaSpawnPoint").transform.position;
        bossMammothPosition = GameObject.Find("BossMammothSpawnPoint").transform.position;
        playerCam = FindObjectOfType<Camera>();

        levelBuilder_2 = FindObjectOfType<LevelBuilder_2>();
        levelBuilder_3 = FindObjectOfType<LevelBuilder_3>();

        gearSlot1 = GameObject.Find("GearSlot1");
        gearSlot2 = GameObject.Find("GearSlot2");
        gearSlot3 = GameObject.Find("GearSlot3");

        //Get HUD
        barArmor = GameObject.Find("Armor Bar").GetComponent<HUDBar>();
        barHealth = GameObject.Find("Health Bar").GetComponent<HUDBar>();

        textArmor = GameObject.Find("Current Armor").GetComponent<HUDText>();
        textHealth = GameObject.Find("Current Health").GetComponent<HUDText>();
        textMoney = GameObject.Find("Current Money").GetComponent<HUDText>();

        weaponHUD = GameObject.Find("HUDWeapon");
        textMunitions = GameObject.Find("Munitions").GetComponent<HUDText>();
        textCapacity = GameObject.Find("Capacity").GetComponent<HUDText>();
        weaponHUD.SetActive(false);
        consumableHUD = GameObject.Find("HUDConsumable");
        textConsumablesRemaining = consumableHUD.GetComponentInChildren<HUDText>();
        consumableHUD.SetActive(false);

        textSlot1 = GameObject.Find("TextSlot 1").GetComponent<HUDText>();
        textSlot2 = GameObject.Find("TextSlot 2").GetComponent<HUDText>();
        textSlot3 = GameObject.Find("TextSlot 3").GetComponent<HUDText>();

        //Get Teleporters

        tpToLevel1FromTuto = GameObject.Find("tpToLevel1FromTuto").transform; 
        tpToTutoFromLevel1 = GameObject.Find("tpToTutoFromLevel1").transform;

        tpToLevel1FromArena1 = GameObject.Find("tpToLevel1FromArena1").transform;

        tpToMarketFromArena1 = GameObject.Find("tpToMarketFromArena1").transform;
        tpToMarketFromArena1.gameObject.SetActive(false);
        tpToArena1FromMarket = GameObject.Find("tpToArena1FromMarket").transform;

        tpToLevel2FromMarket = GameObject.Find("tpToLevel2FromMarket").transform;
        tpToMarketFromLevel2 = GameObject.Find("tpToMarketFromLevel2").transform;

        tpToLevel2FromArena2 = GameObject.Find("tpToLevel2FromArena2").transform;
        //Set Layer Masks

        groundMask = 256;
        enemiesMask = 512;
        playerMask = 1024;
        gearMask = 2048;
        tpMask = 4096;
        npcMask = 8192;

        playerSpawnPoint = playerController.transform.position;

        canvasManager.gameManager = me;
        canvasManager.InitiateCanvas(playerController);

        InitializePlayer();
        dialogManager.gameManager = me;
        playerView.gameManager = me;
        slotsController.gameManager = me;
        gearsInGame = new List<GearController>();
        enemies = new List<EnemyStats>();
        npcInGame = new List<NPC>();



        EnemyStats[] initialEnemies = FindObjectsOfType<EnemyStats>();

        foreach (EnemyStats enemy in initialEnemies)
        {
            InitializeEnemy(enemy);
        }

        NPC[] initialNPCs = FindObjectsOfType<NPC>();

        foreach (NPC npc in initialNPCs)
        {
            InitializeNPC(npc);
        }
    }


    private void Update()
    {
        if (!gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.G) && !slotsController.activeSlot.slotEmpty)
            {
                slotsController.DropWeapon();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {

                if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out gearRay, gearLootRange, gearMask))
                {
                    Debug.Log("RayCast hit a gear : " + gearRay.transform.name);
                    GearController gearToLoot = gearRay.transform.GetComponent<GearController>();

                    if (gearToLoot != null)
                    {
                        if (gearToLoot.gearType == GearType.Weapon)
                        {
                            slotsController.LootGear(gearToLoot, gearToLoot.gearType);
                        }
                        else if (gearToLoot.gearType == GearType.Consumable)
                        {
                            Debug.Log("Looting Consumable " + gearToLoot.name + " !");
                            slotsController.LootGear(gearToLoot, gearToLoot.gearType);
                        }
                    }
                }
                else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out gearRay, gearLootRange, tpMask))
                {
                    Debug.Log("Found teleporter " + gearRay.transform.name);

                    Vector3 teleportTo;
                    switch (gearRay.transform.name)
                    {
                        case "tpToLevel1FromTuto":
                            teleportTo = new Vector3(tpToTutoFromLevel1.position.x, tpToTutoFromLevel1.position.y + 2, tpToTutoFromLevel1.position.z);
                            playerController.teleportPlayer(teleportTo);
                            tpToTutoFromLevel1.gameObject.SetActive(false);
                            break;

                        case "tpToArena1FromLevel1":
                            teleportTo = new Vector3(tpToLevel1FromArena1.position.x, tpToLevel1FromArena1.position.y + 2, tpToLevel1FromArena1.position.z);
                            playerController.teleportPlayer(teleportTo);
                            tpToLevel1FromArena1.gameObject.SetActive(false);
                            enemies.Find(enemy => enemy.name.Equals("BossPanda")).gameObject.SetActive(true);
                            break;

                        case "tpToMarketFromArena1":
                            teleportTo = new Vector3(tpToArena1FromMarket.position.x, tpToArena1FromMarket.position.y + 2, tpToArena1FromMarket.position.z);
                            playerController.teleportPlayer(teleportTo);
                            tpToArena1FromMarket.gameObject.SetActive(false);
                            break;

                        case "tpToLevel2FromMarket":
                            teleportTo = new Vector3(tpToMarketFromLevel2.position.x, tpToMarketFromLevel2.position.y + 2, tpToMarketFromLevel2.position.z);
                            playerController.teleportPlayer(teleportTo);
                            tpToMarketFromLevel2.gameObject.SetActive(false);
                            break;

                        case "tpToArena2FromLevel2":
                            teleportTo = new Vector3(tpToLevel2FromArena2.position.x, tpToLevel2FromArena2.position.y + 2, tpToLevel2FromArena2.position.z);
                            playerController.teleportPlayer(teleportTo);
                            tpToLevel2FromArena2.gameObject.SetActive(false);
                            enemies.Find(enemy => enemy.name.Equals("BossMammoth")).gameObject.SetActive(true);
                            break;


                        default:
                            Debug.Log("Error");
                            break;
                    }
                }
                else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out gearRay, gearLootRange * 2, npcMask))
                {
                    Debug.Log("NPC detected");
                    NPC npc = gearRay.transform.GetComponent<NPC>();
                    if (npc.mType == NPCType.ShopKeeper)
                    {
                        ShopController shopController = npc.gameObject.GetComponent<ShopController>();
                        canvasManager.SetShop(shopController);
                    }
                    npc.LaunchDialog();
                }
            }

        }
    }


    public void StartANewGame()
    {
        Debug.Log("Start a new game");
        playerController.playerReset(playerSpawnPoint);
        ClearGearsInGame();
        ClearEnemiesInGame();

        BuildLevel1();
    }

    public void BuildLevel1()
    {
        canvasManager.ToggleCanvas(CanvasType.LoadingCanvas);
        levelBuilder_2.gameManager = me;

        levelBuilder_2.NewMap();

    }
    public void BuildLevel2()
    {
        levelBuilder_3.gameManager = me;

        levelBuilder_3.NewMap();

    }

    public void RestartGameStats()
    {
        dialogManager.canvasManager = canvasManager;
        canvasManager.dialogManager = dialogManager;

        slotsController.gearsInLevel = gearsInLevel;

        slotsController.slots.Clear();
        Slot slot1 = new Slot(gearSlot1, textSlot1, playerCam);
        slotsController.slots.Add(slot1);
        Slot slot2 = new Slot(gearSlot2, textSlot2, playerCam);
        slotsController.slots.Add(slot2);
        Slot slot3 = new Slot(gearSlot3, textSlot3, playerCam);
        slotsController.slots.Add(slot3);

        InitializeArsenals();


        SpawnInitialGears();
        ResetGearsInGame();
        ResetGearSlots(slotsController);
        ResetArsenals(slotsController);
        if (!slotsController.activeSlot.slotEmpty && slotsController.activeWeapon != null)
        {
            slotsController.ApplyArsenalToGear();
        }
        SpawnBosses();
        SpawnShopKeeper(shopObject.transform.position);

        canvasManager.ToggleCanvas(CanvasType.LoadingCanvas);
        playerView.xRotation = 0f;
    }

    private void InitializeShopKeeper(ShopController shopKeeper)
    {
        shopKeeper.InitializeShop(playerController, canvasManager, me);
        shopControllers.Add(shopKeeper);
    }

    private void InitializePlayer()
    {
        playerController.gameManager = me;
        playerController.controller = playerBody;
        playerController.slotsController = slotsController;
        playerController.barArmor = barArmor;
        playerController.barHealth = barHealth;
        playerController.txtArmor = textArmor;
        playerController.txtHealth = textHealth;
        playerController.txtMoney = textMoney;
    }

    public void ResetGearsInGame()
    {
        gearsInGame.Clear();
        GearController[] gearsIG = FindObjectsOfType<GearController>();

        foreach (GearController gear in gearsIG)
        {
            GearController initializedGead = InitializeGear(gear);
            gearsInGame.Add(initializedGead);
        }


    }

    private void SpawnInitialGears()
    {
        SpawnGear("Prefabs/Gears/Normal Gun", initialGearPosition1);
        SpawnGear("Prefabs/Gears/UZI Gun", initialGearPosition2);
        SpawnGear("Prefabs/Gears/Shotgun", initialGearPosition3);

    }

    private void SpawnBosses()
    {
        SpawnEnemy("Prefabs/BossPanda", bossPandaPosition);
        SpawnEnemy("Prefabs/BossMammoth", bossMammothPosition);

    }
    public void SpawnGear(string gear, Vector3 position)
    {
        //Debug.Log("Spawning " + gear + " at position " + position);
        GameObject newGearObject = Instantiate(Resources.Load(gear), position, Quaternion.Euler(new Vector3(-90, 0, 90))) as GameObject;
        newGearObject.transform.localScale = Vector3.one * 10;
        newGearObject.transform.SetParent(gearsInLevel.transform);
        GearController newGear = newGearObject.GetComponent<GearController>();
        GearController intilializedGear = InitializeGear(newGear);
        gearsInGame.Add(intilializedGear);
    }

    public GearController InitializeGear(GearController gearToInitialize)
    {
        gearToInitialize.me = gearToInitialize;
        gearToInitialize.fpsCam = playerCam;
        gearToInitialize.gameManager = me;
        gearToInitialize.audioManager = audioManager;
        gearToInitialize.slotsController = slotsController;
        gearToInitialize.ResetGear();
        gearToInitialize.enabled = false;
        if(gearToInitialize.gearType == GearType.Weapon)
        {
            Weapon weaponToInitialize = gearToInitialize as Weapon;
            weaponToInitialize.name = weaponToInitialize.mType.ToString();
            weaponToInitialize.targetsMask = groundMask | enemiesMask | npcMask;

        }
        if(gearToInitialize.gearType == GearType.Consumable)
        {
            Consumable consumableToInitialize = gearToInitialize as Consumable;
            consumableToInitialize.name = consumableToInitialize.mType.ToString();
            consumableToInitialize.playerController = playerController;
        }
        return gearToInitialize;

    }

    public void ResetGearSlots(SlotsController slotsC)
    {
        foreach (Slot s in slotsC.slots)
        {
            if (s.slot.transform.childCount > 0)
            {
                GearController slotsGear = s.slot.transform.GetChild(0).GetComponent<GearController>();
                if (slotsGear != null)
                {
                    s.gear = slotsGear;
                    s.slotEmpty = false;
                    s.SetName(s.gear.name);
                    s.gear.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Child(0) of this slot is not a gear !");
                    s.slotEmpty = true;
                    s.SetName("Empty Slot");
                }
            }
            else
            {
                s.slotEmpty = true;
                s.SetName("Empty Slot");

            }
        }
        slotsC.slotIndex = 0;
        slotsC.activeSlot = slotsC.slots[slotsC.slotIndex];
        if (slotsC.activeSlot.slot.transform.childCount > 0 && !slotsC.activeSlot.slotEmpty)
        {
            slotsC.activeSlot.gear.gameObject.SetActive(true);
            slotsC.activeSlot.gear.enabled = true;
            if(slotsC.activeSlot.gear.gearType == GearType.Weapon)
            {
                slotsC.activeWeapon = slotsC.activeSlot.gear as Weapon;
                slotsC.activeConsumable = null;
                slotsC.ApplyArsenalToGear();
            }
            else if(slotsC.activeSlot.gear.gearType == GearType.Consumable)
            {
                slotsC.activeConsumable = slotsC.activeSlot.gear as Consumable;
                slotsC.activeWeapon = null;
            }
            ActivateHudGear(slotsC.activeSlot.gear);
        }

    }

    public void SetPlayerHUD()
    {
        barHealth.SetMyMaxValue(playerController.currentHealth);
        barHealth.SetMyCurrentValue(playerController.currentHealth);
        textHealth.SetMyText(playerController.currentHealth.ToString());
        barArmor.SetMyMaxValue(playerController.maxArmor);
        barArmor.SetMyCurrentValue(playerController.currentArmor);
        textArmor.SetMyText(playerController.currentArmor.ToString());
        textMoney.SetMyText(playerController.currentMoney.ToString());

    }

    public void ActivateHudGear(GearController gear)
    {
        if (gear.gearType == GearType.Weapon)
        {
            Weapon weapon = gear as Weapon;
            consumableHUD.SetActive(false);
            weaponHUD.SetActive(true);
            textMunitions.SetMyText(weapon.munitions.ToString());
            textCapacity.SetMyText(weapon.capacity.ToString());
        }
        else if (gear.gearType == GearType.Consumable)
        {
            Consumable consumable = gear as Consumable;
            weaponHUD.SetActive(false);
            consumableHUD.SetActive(true);
            textConsumablesRemaining.SetMyText(consumable.consumablesRemaining.ToString());
        }
    }

    public void HideHUDGear()
    {
        weaponHUD.SetActive(false);
        consumableHUD.SetActive(false);
    }


    public void EnemyDied(EnemyStats enemy, Vector3 position)
    {
        slotsController.lootArsenal();
        if (enemy.name.Equals("BossPanda"))
        {
            tpToMarketFromArena1.gameObject.SetActive(true);
        }if (enemy.name.Equals("ShopKeeper"))
        {
            npcInGame.Remove(enemy.gameObject.GetComponent<NPC>());
            shopControllers.Remove(enemy.gameObject.GetComponent<ShopController>());
        }
        if (enemy.name.Equals("BossMammoth"))
        {
            canvasManager.ToggleCanvas(CanvasType.CreditsCanvas);
        }
        enemies.Remove(enemy);
        enemy.gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(enemy.gameObject, 3);
    }

    public void SpawnEnemy(string enemy, Vector3 position)
    {
        GameObject newEnemy = Instantiate(Resources.Load(enemy), position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;
        newEnemy.name = enemy;
        EnemyStats newStats = newEnemy.GetComponent<EnemyStats>();
        InitializeEnemy(newStats);
    }
    public void SpawnShopKeeper(Vector3 position)
    {
        //Debug.Log("Spawning ShopKeeper at position " + position);
        GameObject newShopKeeper= Instantiate(Resources.Load("Prefabs/ShopKeeper"), position, Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;
        newShopKeeper.name = "ShopKeeper";
        newShopKeeper.transform.SetParent(shopObject.transform);
        newShopKeeper.transform.position = position;
        NPC newShopKeeperNPC = newShopKeeper.GetComponent<NPC>();
        npcInGame.Add(newShopKeeperNPC);
        InitializeNPC(newShopKeeperNPC);
        ShopController newShopKeeperController = newShopKeeper.GetComponent<ShopController>();
        InitializeShopKeeper(newShopKeeperController);
        ShopKeeperAI newShopKeeperEnemy = newShopKeeper.GetComponent<ShopKeeperAI>();
        newShopKeeperEnemy.enabled = false;
        InitializeEnemy(newShopKeeperEnemy);
    }

    public void PlayerDies()
    {
        Debug.Log("Uh oh ... You died !");

        canvasManager.ToggleCanvas(CanvasType.GameOverCanvas);
    }

    public void ClearGearsInGame()
    {
        for(int i = 0; i < gearsInGame.Count ; i++)
        {
            GearController gear = gearsInGame[i];
            Debug.Log("Gear : " + gear.name);
            if(gear.gearType == GearType.Weapon)
            {
                Weapon weaponToDestroy = gear as Weapon;
                gearsInGame.Remove(weaponToDestroy);
                Debug.Log("Destroying Weapon : " + weaponToDestroy.name);
                weaponToDestroy.SelfDestroyW();

            } else if (gear.gearType == GearType.Consumable)
            {
                Consumable consumableToDestroy = gear as Consumable;
                gearsInGame.Remove(consumableToDestroy);
                Debug.Log("Destroying Consumable : "+ consumableToDestroy.name);
                consumableToDestroy.SelfDestroyC();
            }
        }

    }
    public void ClearEnemiesInGame()
    {
        Debug.Log("Destroying All Enemies");
        for (int i = 0; i < enemies.Count ; i++)
        {
            EnemyStats enemy = enemies[i];
            if (enemy.name.Equals("ShopKeeper"))
            {
                npcInGame.Remove(enemy.gameObject.GetComponent<NPC>());
                shopControllers.Remove(enemy.gameObject.GetComponent<ShopController>());
            }
            //enemies.Remove(enemy);
            Debug.Log(enemy.name+" destroyed.");
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }

    private void InitializeEnemy(EnemyStats myEnemy)
    {
        myEnemy.playerObject = playerObject;
        myEnemy.playerController = playerController;
        myEnemy.gameManager = me;
        myEnemy.audioManager = audioManager;
        myEnemy.name = myEnemy.myType.ToString();
        if(myEnemy.name.Equals("BossPanda") || myEnemy.name.Equals("BossMammoth"))
        {
            myEnemy.gameObject.SetActive(false);
        }
        enemies.Add(myEnemy);

    }

    private void InitializeNPC(NPC myNpc)
    {
        myNpc.dialogManager = dialogManager;
        myNpc.gameManager = me;
        myNpc.npcName = myNpc.mType.ToString();
        myNpc.SetNpcConversation(myNpc.npcName);
        myNpc.whatIsPlayer = playerMask;
        myNpc.dialogRange = dialogRange;
    }
    public void InitializeArsenals()
    {
        Arsenal damage = new Arsenal("Damage", playerController, 1.1f, 1f);
        Arsenal rechargeTime = new Arsenal("RechargeTime", playerController, 0.9f, 1f);
        Arsenal fireRate = new Arsenal("FireRate", playerController, 1.1f, 1f);
        slotsController.arsenals.Add(damage);
        slotsController.arsenals.Add(rechargeTime);
        slotsController.arsenals.Add(fireRate);
        foreach (Arsenal a in slotsController.arsenals)
        {
            slotsController.arsenalsLootable.Add(a.myName);
        }

    }

    public void CreateFloatingText(string myText, Vector3 position)
    {
        Vector3 myPosition = position;
        myPosition.y += 2;
        Quaternion myRotation = new Quaternion();

        GameObject myFloatingText = Instantiate(Resources.Load("Prefabs/TextAnimator"), myPosition, myRotation) as GameObject;
        if (myFloatingText == null)
        {
            Debug.Log("Failed to load textObject");
            return;
        }
        TextMesh test = myFloatingText.GetComponentInChildren<TextMesh>();
        if (test == null)
        {

            Debug.Log("Failed to load textMesh");
            return;
        }
        test.transform.rotation = Quaternion.LookRotation(test.transform.position - playerCam.transform.position);
        test.text = myText;
        Destroy(myFloatingText, 1);
    }

    public void ResetArsenals(SlotsController slotsC)
    {
        slotsC.arsenalsLootable.Clear();

        foreach (Arsenal a in slotsC.arsenals)
        {
            a.totalEffect = a.initialEffect;
            slotsC.arsenalsLootable.Add(a.myName);
        }
    }

}
