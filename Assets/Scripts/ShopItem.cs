using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class ShopItem : MonoBehaviour
{
    public GameObject item;
    public int cost;
    public GearController gear;
    public Button buyItemBtn;
    public string gearName;
    public ShopController shopController;
    public PlayerController playerController;
    public bool playerBoughtMe;

    public void InitializeShopItem(GameObject _item, int _cost, string _gearName, ShopController _shopController, PlayerController _playerController)
    {
        item = _item;
        cost = _cost;
        gearName = _gearName;
        shopController = _shopController;
        playerBoughtMe = false;
        playerController = _playerController;

        List<Text> itemTexts = item.GetComponentsInChildren<Text>().ToList();
        if(itemTexts.Count > 0)
        {
            Text nameText = itemTexts.Find(text => text.name == "ItemName");
            if(nameText == null)
            {
                Debug.LogError("No nameText found");
                return;
            }
            nameText.text = gearName;
            Text costText = itemTexts.Find(text => text.name == "CostText");
            if (costText == null)
            {
                Debug.LogError("No costText found");
                return;
            }
            costText.text = cost.ToString();

            buyItemBtn = item.GetComponentInChildren<Button>();
            buyItemBtn.onClick.AddListener(() => {
                PlayerWantsToBuyMe();

            });
        } else
        {
            Debug.LogError("No itemTexts found");
        }

    }

    private void PlayerWantsToBuyMe()
    {
        if (playerController.currentMoney >= cost)
        {
            playerBoughtMe = true;
            playerController.PlayerLooseMoney(cost);
            shopController.PlayerBoughtAnItem(this);

        }
        else
        {
            Debug.Log("Not enough money !");
        }
    }


}
