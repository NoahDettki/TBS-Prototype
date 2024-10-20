using System.Collections.Generic;
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


    public HexGrid grid;
    public int gridRings;

    [HideInInspector]
    public HexCell focusedCell;
    [HideInInspector]
    public HexCell.Building building;
    // A list of cells that preview their resource gain in building mode
    [HideInInspector]
    public List<HexCell> previewedCells;

    [HideInInspector]
    public int wood, stone, food;
    private int woodGain, stoneGain, foodGain;

    public bool testPathfinding = false;
    public Vector2 testPathfindingStart;
    public Vector2 testPathfindingEnd;
    public GameObject testPathMarker;

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
        previewedCells = new List<HexCell>();
        focusedCell = null;
    }

    private void TestPathfinding() {
        List<HexCell> path = grid.FindPath(grid.GetCellAt((int)testPathfindingStart.x, (int)testPathfindingStart.y), grid.GetCellAt((int)testPathfindingEnd.x, (int)testPathfindingEnd.y));
        foreach (HexCell cell in path) {
            Instantiate(testPathMarker, cell.transform.position, Quaternion.identity);
        }
        if (path.Count > 0) print("No possible path");
    }

    public void StartNewGame() {
        State = GameState.PREPARING;
        grid.GenerateCenteredGrid(gridRings);
        State = GameState.INGAME;
        // TODO: remove eventually. This is only for debugging
        if (testPathfinding)TestPathfinding();
    }

    public void EndRound() {
        grid.EndRound();
    }
}
