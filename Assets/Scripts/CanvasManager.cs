using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public enum CanvasType
{
    HUDCanvas,
    ShopCanvas,
    PauseCanvas,
    MainMenuCanvas,
    OptionsCanvas,
    GameOverCanvas,
    CreditsCanvas,
    LoadingCanvas
}
public class CanvasManager : MonoBehaviour
{
    public List<CanvasController> canvasControllerList;
    public DialogManager dialogManager;
    public GameManager gameManager;
    public PlayerController playerController;

    CanvasController hud, shopCanvas, pause, mainMenu, options, gameOver, credits, loading;
    HorizontalLayoutGroup shopContent;
    public ShopController shopController;

    public MenuInteract menuInteract;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !shopCanvas.isShown && !gameManager.gameIsPaused && gameManager.donePlaying)
        {
            ToggleCanvas(CanvasType.PauseCanvas);
        }
    }

    public void InitiateCanvas(PlayerController _playerController)
    {
        playerController = _playerController;
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(canvas => canvas.gameObject.SetActive(false));

        shopCanvas = canvasControllerList.Find(canvas => canvas.canvasType == CanvasType.ShopCanvas);

        List<Button> shopButtons = shopCanvas.GetComponentsInChildren<Button>().ToList();
        shopButtons.Find(button => button.name == "ExitShopButton").onClick.AddListener(() =>
        {
            ToggleCanvas(CanvasType.ShopCanvas);
            if(shopController.itemsBought.Count > 0)
            {
                Debug.Log("You bought " + shopController.itemsBought.Count + " items !");
                shopController.SpawnBoughtGears();
            }
            else
            {
                Debug.Log("You did not bought anything !");
            }
            ReleaseShop();
            dialogManager.ResumeDialog(1, 0);
        });
        shopContent = shopCanvas.GetComponentInChildren<HorizontalLayoutGroup>();


        ToggleCanvas(CanvasType.MainMenuCanvas);
        Cursor.visible = true;

    }

    public void ToggleCanvas(CanvasType type)
    {
        Debug.Log("Toggle called on " + type.ToString());
        CanvasController toggled = canvasControllerList.Find(canvas => canvas.canvasType == type);

        if (toggled != null)
        {
            if (!toggled.isShown)
            {
                Debug.Log("Was hidden");
                if(type == CanvasType.HUDCanvas)
                {
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.visible = true;
                }
                toggled.gameObject.SetActive(true);
                toggled.isShown = true;

                if(type != CanvasType.HUDCanvas)
                {
                    if (type != CanvasType.LoadingCanvas)
                    {
                        Time.timeScale = 0f;
                        Debug.Log("Time scale 0 and CursorConfined");
                        Cursor.lockState = CursorLockMode.Confined;
                    }                  
                    gameManager.gameIsPaused = true;
                }
            }
            else
            {
                Debug.Log("Was shown");
                if (type == CanvasType.HUDCanvas)
                {
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                }
                toggled.gameObject.SetActive(false);
                toggled.isShown = false;

                if (type != CanvasType.HUDCanvas)
                {
                    if (type != CanvasType.LoadingCanvas)
                    {
                        Time.timeScale = 1f;
                        Cursor.lockState = CursorLockMode.Locked;
                        Debug.Log("Time scale 1 and Cursor Locked");
                    }
                    gameManager.gameIsPaused = false;
                }

            }
        }
    }

    public void ReleaseShop()
    {
        foreach (ShopItem item in shopController.shopItems)
        {
            item.gameObject.transform.SetParent(shopController.transform);
            item.gameObject.SetActive(false);
        }
        shopController = null;
    }

    public void SetShop(ShopController _shopController)
    {
        if(shopController != null)
        {
            ReleaseShop();
        }
        shopController = _shopController;
        foreach(ShopItem item in shopController.shopItems)
        {
            item.gameObject.transform.SetParent(shopContent.transform);
            item.transform.localScale = Vector3.one;
            item.gameObject.SetActive(true);
        }
    }


}
