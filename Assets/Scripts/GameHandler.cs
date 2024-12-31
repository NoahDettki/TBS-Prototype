using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameHandler : MonoBehaviour {
    // Singleton pattern
    public static GameHandler game;

    public enum GameState {
        MENU,
        PREPARING,
        INGAME
    }
    public enum Resources {
        LUMBER,
        STONE,
        WHEAT
    }
    public static GameState State { get; private set; }


    public HexGrid grid;
    public int gridRings;

    [HideInInspector]
    public HexCell focusedCell;
    [HideInInspector]
    public Building.Type building;
    // A list of cells that preview their resource gain in building mode
    [HideInInspector]
    public List<HexCell> estimatedCells;

    [Header("Ressources")]
    public TMP_Text text_lumberCount;
    public TMP_Text text_stoneCount;
    public TMP_Text text_wheatCount;

    [SerializeField]
    private int startLumber, startStone, startWheat;
    private int lumber, stone, wheat;
    private int lumberGain, stoneGain, wheatGain;

    [Header("Pathfinding")]
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
        building = Building.Type.NONE;
        estimatedCells = new List<HexCell>();
        focusedCell = null;
    }

    public void Start() {
        lumber = startLumber;
        stone = startStone;
        wheat = startWheat;
        UpdateRessourceDisplay();
    }

    public void Update() {
        if (Keyboard.current[Key.Numpad1].wasPressedThisFrame) AdjustLumber(10);
        if (Keyboard.current[Key.Numpad2].wasPressedThisFrame) AdjustStone(10);
        if (Keyboard.current[Key.Numpad3].wasPressedThisFrame) AdjustWheat(10);
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

    public void UpdateRessourceDisplay() {
        text_lumberCount.text = GameHandler.game.lumber.ToString();
        text_stoneCount.text = GameHandler.game.stone.ToString();
        text_wheatCount.text = GameHandler.game.wheat.ToString();
    }

    public void EndRound() {
        grid.EndRound();
    }

    public int GetLumber() {
        return lumber;
    }

    public int GetStone() {
        return stone;
    }

    public int GetWheat() {
        return wheat;
    }

    public bool AdjustLumber(int increase) {
        if (lumber + increase < 0)
            return false;
        lumber += increase;
        UpdateRessourceDisplay();
        return true;
    }

    public bool AdjustStone(int increase) {
        if (stone + increase < 0)
            return false;
        stone += increase;
        UpdateRessourceDisplay();
        return true;
    }

    public bool AdjustWheat(int increase) {
        if (wheat + increase < 0)
            return false;
        wheat += increase;
        UpdateRessourceDisplay();
        return true;
    }
}
