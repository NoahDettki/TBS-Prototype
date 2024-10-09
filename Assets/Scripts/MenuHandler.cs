using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject pauseMenu;

    public void Awake() {
        mainMenu.SetActive(true);
        hud.SetActive(false);
    }

    public void NewGame() {
        // Prepare Map
        GameHandler.game.StartNewGame();

        // Hide menu
        hud.SetActive(true);
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

    public void SelectInBuildMenu(int buildingID) {
        HexCell.Building building = (HexCell.Building)buildingID;
        if (building == GameHandler.game.building) return;

        GameHandler.game.building = building;
        print("Player selected " + building);
    }

    public void EndRound() {
        GameHandler.game.EndRound();
    }
}
