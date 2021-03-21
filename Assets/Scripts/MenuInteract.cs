using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuInteract : MonoBehaviour
{
    public CanvasManager canvasManager;

    public void StartGameButton()
    {
        canvasManager.ToggleCanvas(CanvasType.MainMenuCanvas);
        canvasManager.ToggleCanvas(CanvasType.HUDCanvas);
        canvasManager.gameManager.StartANewGame();
    }
    public void Unpause()
    {
        canvasManager.ToggleCanvas(CanvasType.PauseCanvas);
    }
    public void PauseToMenu()
    {
        canvasManager.ToggleCanvas(CanvasType.PauseCanvas);
        canvasManager.ToggleCanvas(CanvasType.HUDCanvas);
        canvasManager.ToggleCanvas(CanvasType.MainMenuCanvas);

    }
    public void ToggleOptions()
    {
        canvasManager.ToggleCanvas(CanvasType.OptionsCanvas);
    }
    public void CreditsToMenu()
    {
        canvasManager.ToggleCanvas(CanvasType.CreditsCanvas);
        canvasManager.ToggleCanvas(CanvasType.HUDCanvas);
        canvasManager.ToggleCanvas(CanvasType.MainMenuCanvas);
    }
    public void MenuToCredits()
    {
        canvasManager.ToggleCanvas(CanvasType.MainMenuCanvas);
        canvasManager.ToggleCanvas(CanvasType.CreditsCanvas);
    }
    public void GameOverRetry()
    {
        canvasManager.ToggleCanvas(CanvasType.GameOverCanvas);
        canvasManager.gameManager.StartANewGame();
    }
    public void GameOverToMenu()
    {
        canvasManager.ToggleCanvas(CanvasType.GameOverCanvas);
        canvasManager.ToggleCanvas(CanvasType.HUDCanvas);
        canvasManager.ToggleCanvas(CanvasType.MainMenuCanvas);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
