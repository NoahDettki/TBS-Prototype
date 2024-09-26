using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;

    public void NewGame() {
        // Prepare Map
        GameHandler.game.StartNewGame();

        // Hide menu
        mainMenu.SetActive(false);
    }

    public void QuitGame() {
        // TODO: Save game data

        // Quit depending on wether running in editor or standalone
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
