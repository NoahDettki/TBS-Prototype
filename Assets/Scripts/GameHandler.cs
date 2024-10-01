using UnityEngine;

public class GameHandler : MonoBehaviour {
    // Singleton pattern
    public static GameHandler game;

    public enum GameState {
        MENU,
        PREPARING,
        INGAME
    }
    public static GameState State { get; private set; }

    public HexCell.Building building;

    public HexGrid grid;
    public int gridRings;
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
        building = HexCell.Building.NONE;
        focusedCell = null;
    }

    public void StartNewGame() {
        State = GameState.PREPARING;
        grid.GenerateCenteredGrid(gridRings);
        State = GameState.INGAME;
    }
}
