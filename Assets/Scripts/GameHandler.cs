using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Singleton pattern
    public static GameHandler game;

    public enum GameState {
        MENU,
        PREPARING,
        INGAME
    }
    public static GameState State { get; private set; }

    public HexGrid grid;
    [HideInInspector]
    public HexCell focusedCell;

    public void Awake() {
        // Singleton pattern
        if (game == null) {
            game = this;
        } else {
            print("Warning: Doublicate GameHandler detected!");
            Destroy(this.gameObject);
        }
        // Initialize
        State = GameState.MENU;
        focusedCell = null;
    }

    public void StartNewGame() {
        State = GameState.PREPARING;
        grid.GenerateGrid(6, 6);
    }
}
