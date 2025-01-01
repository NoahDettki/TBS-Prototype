using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject nextRound;
    public GameObject pauseMenu;
    private float timetillNextRound = 1.1f;

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

    public void EndRound() {
        nextRound.GetComponent<Button>().interactable = false;
        StartCoroutine(BlockNextRoundButton());
        GameHandler.game.EndRound();
    }

    IEnumerator BlockNextRoundButton() {
        yield return new WaitForSeconds(timetillNextRound);
        nextRound.GetComponent<Button>().interactable = true;
    }
}
