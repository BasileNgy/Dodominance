using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopController : MonoBehaviour
{
    public List<ShopItem> shopItems;
    public CanvasManager canvasManager;
    public List<string> itemsBought;
    public PlayerController playerController;
    public GameManager gameManager;

    public void InitializeShop(PlayerController _playerController, CanvasManager _canvasManager, GameManager _gameManager)
    {
        playerController = _playerController;
        canvasManager = _canvasManager;
        gameManager = _gameManager;
        ResetShop(4);
    }
    public void ResetShop(int nbrOfItem)
    {

        for (int i = 0; i < nbrOfItem; i++)
        {
            GameObject itemUI = Instantiate(Resources.Load("Prefabs/ShopItem")) as GameObject;
            itemUI.transform.SetParent(transform);

            GearType randomGearType = (GearType)Random.Range(0, System.Enum.GetValues(typeof(GearType)).Length);
            string gearName = "";
            int cost = 0;
            if (randomGearType == GearType.Weapon)
            {
                WeaponType randomWeaponType = (WeaponType)Random.Range(0, System.Enum.GetValues(typeof(WeaponType)).Length);
                switch (randomWeaponType)
                {
                    case WeaponType.NormalGun:
                        gearName = "Normal Gun";
                        cost = 20;
                        break;
                    case WeaponType.UziGun:
                        gearName = "UZI Gun";
                        cost = 30;
                        break;
                    case WeaponType.ShotGun:
                        gearName = "ShotGun";
                        cost = 40;
                        break;
                    default:
                        break;
                }

            }
            else if (randomGearType == GearType.Consumable)
            {
                ConsumableType randomConsumableType = (ConsumableType)Random.Range(0, System.Enum.GetValues(typeof(ConsumableType)).Length);
                switch (randomConsumableType)
                {
                    case ConsumableType.HealingPotion:
                        gearName = "HealingPotion";
                        break;
                    default:
                        break;
                }
            }

            if (gearName == "")
            {
                Debug.LogError("Error in random gear generation");
                return;
            }
            ShopItem item = itemUI.GetComponent<ShopItem>();
            item.InitializeShopItem(itemUI, cost, gearName, this, playerController);
            shopItems.Add(item);
            item.gameObject.SetActive(false);
        }
    }
    public void PlayerBoughtAnItem(ShopItem item)
    {
        itemsBought.Add(item.gearName);
        shopItems.Remove(item);
        Destroy(item.gameObject);
    }

    public void SpawnBoughtGears()
    {
        for(int i = 0 ; i < itemsBought.Count ; i++)
        {
            string item = itemsBought[i];
            string path = "Prefabs/Gears/" + item;
            Vector3 itemPosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    itemPosition = gameManager.gearPosition1;
                    break;
                case 1:
                    itemPosition = gameManager.gearPosition2;
                    break;
                case 2:
                    itemPosition = gameManager.gearPosition3;
                    break;
                case 3:
                    itemPosition = gameManager.gearPosition4;
                    break;
                default:
                    break;
            }
            gameManager.SpawnGear(path, itemPosition);
        }

        itemsBought.Clear();
    }

}
