using UnityEngine;

public class HexCell : MonoBehaviour {
    public enum Type { MEADOW, FOREST, MOUNTAINS }
    public enum Building { NONE, CASTLE, SAWMILL, QUARRY, WINDMILL, GRAIN }
    public GameObject prefab_forest, prefab_mountains;
    public GameObject prefab_castle, prefab_sawmill, prefab_quarry, prefab_windmill, prefab_grain;
    public HexCoordinates coordinates;

    private Type type;
    private Building building;
    private GameObject go_building;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        type = Type.MEADOW;
        building = Building.NONE;
        go_building = null;
    }

    public void SetRandomType() {
        Quaternion rotation;
        switch (Random.Range(0, 3)) {
            case 0:
                type = Type.MEADOW;
                break;
            case 1:
                type = Type.FOREST;
                rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
                Instantiate(prefab_forest, transform.GetChild(0).position, rotation, transform.GetChild(0));
                break;
            case 2:
                type = Type.MOUNTAINS;
                rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
                Instantiate(prefab_mountains, transform.GetChild(0).position, rotation, transform.GetChild(0));
                break;
        }
    }

    public bool CanBuild(Building b) {
        // Can only build on empty cells
        if (building != Building.NONE) return false;

        switch (b) {
            case Building.SAWMILL:
                if (type == Type.MOUNTAINS) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    /// <summary>
    /// Includes function CanBuild() to test if the building can be placed. If true, places it
    /// </summary>
    /// <param name="b"></param>
    /// <returns>true, if the building was placed. Returns false otherwise</returns>
    public bool Build(Building b) {
        if (CanBuild(b)) {
            switch(b) {
                case Building.SAWMILL:
                    go_building = Instantiate<GameObject>(prefab_sawmill,transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
                    break;
                case Building.QUARRY:
                    go_building = Instantiate<GameObject>(prefab_sawmill, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
                    break;
            }
            // Start Animation
            building = b;
            return true;
        } else return false;
    }

    public void SetCastle() {
        Instantiate(prefab_castle, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
        building = Building.CASTLE;
    }

    public void Focus() {
        animator.SetBool("focus", true);
        //animator.Play("CellClicked");
    }

    public void LooseFocus() {
        animator.SetBool("focus", false);
        //animator.Play("CellLooseFocus");
    }

    public Type GetCellType() {
        return type;
    }
}
