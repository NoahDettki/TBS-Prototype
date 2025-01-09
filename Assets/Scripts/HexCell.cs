using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public enum Type { MEADOW, FOREST, MOUNTAINS }

    public GameObject prefab_rockyMeadow, prefab_rockyMeadow2,
        prefab_forest, prefab_rockyForest, prefab_lumberForest,
        prefab_mountains, prefab_mountains2, prefab_highMountains, prefab_highMountains2;
    public GameObject prefab_construction, prefab_quarryPosition, prefab_acre, prefab_castle, prefab_sawmill, prefab_quarry, prefab_windmill, prefab_grain, prefab_tower;
    public HexCoordinates coordinates;
    public GameObject go_terrain, go_building, go_estimation;
    public TMP_Text resourceText;

    public Donkey prefab_donkey;

    public Type type = Type.MEADOW;
    private Building.Type building, buildingPreview;
    private bool isConstructionFinished;
    private int estimatedResourceGain;
    private int resourceGain;
    private int resourceRadius;

    private Animator animator;
    private GameObject go_decoration;
    private int heightLevel;
    private Donkey donkey;
    private Quaternion possibleQuarryRotation;

    private void Awake() {
        animator = GetComponent<Animator>();
        building = Building.Type.NONE;
        buildingPreview = Building.Type.NONE;
        isConstructionFinished = false;
        estimatedResourceGain = -1;
        resourceGain = 0;
        resourceRadius = 2;
    }

    public void Decorate() {
        Quaternion rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
        float rng = UnityEngine.Random.value;
        switch (type) {
            case Type.MEADOW:
                // Meadows with buildings need no decoration
                if (building != Building.Type.NONE)
                    return;
                // Meadows at a certain height level can have decorative rocks on them
                if (heightLevel >= 2) {
                    if (rng > 0.8f) {
                        go_decoration = Instantiate(prefab_rockyMeadow, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    }
                    if (rng > 0.5f) {
                        go_decoration = Instantiate(prefab_rockyMeadow2, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    }
                }
                break;
            case Type.FOREST:
                // Forests at a certain height level can have decorative rocks on them
                if (heightLevel >= 2) {
                    if (rng > 0.5f) {
                        go_decoration = Instantiate(prefab_rockyForest, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    }
                }
                go_decoration = Instantiate(prefab_forest, go_terrain.transform.position, rotation, go_terrain.transform);
                break;
            case Type.MOUNTAINS:
                // Mountains at a certain height level don't have vegetation
                if (heightLevel >= 3) {
                    if (rng > 0.5f) {
                        go_decoration = Instantiate(prefab_highMountains, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    } else {
                        go_decoration = Instantiate(prefab_highMountains2, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    }
                } else {
                    if (rng > 0.5f) {
                        go_decoration = Instantiate(prefab_mountains, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    } else {
                        go_decoration = Instantiate(prefab_mountains2, go_terrain.transform.position, rotation, go_terrain.transform);
                        break;
                    }
                }
        }
    }

    public bool CanBuild(Building.Type b) {
        // Can only build on empty cells
        if (building != Building.Type.NONE) return false;

        // Can only build if enough ressources
        if (Building.Costs[b].lumber > GameHandler.game.GetLumber() ||
            Building.Costs[b].stone > GameHandler.game.GetStone() ||
            Building.Costs[b].wheat > GameHandler.game.GetWheat()) {
            return false;
        }

        switch (b) {
            case Building.Type.SAWMILL:
                if (type == Type.MOUNTAINS) return false;
                break;
            case Building.Type.QUARRY:
                if (type != Type.MOUNTAINS) return false;
                // If the mountain is completely surrounded by other mountains it is not possible to build a quarry there
                bool accessPossible = false;
                foreach (HexCell c in GameHandler.game.grid.GetNeighbours(coordinates.X, coordinates.Z)) {
                    if (c.GetCellType() != Type.MOUNTAINS) {
                        accessPossible = true;
                        break;
                    }
                }
                if (!accessPossible) return false;
                break;
            case Building.Type.WINDMILL:
                if (type != Type.MEADOW) return false;
                break;
            case Building.Type.GRAIN:
                if (type != Type.MEADOW) return false;
                break;
            case Building.Type.TOWER:
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
    public bool Build(Building.Type b) {
        if (CanBuild(b)) {
            // Check if this cell is reachable from the castle
            List<HexCell> path = GameHandler.game.grid.FindPath(this);
            if (path == null) return false;

            switch(b) {
                case Building.Type.SAWMILL:
                    Destroy(go_decoration);
                    // Lumbermills in the forest spawn with a few trees around them
                    if (type == Type.FOREST) {
                        go_decoration = Instantiate(prefab_lumberForest, go_terrain.transform.position, Quaternion.identity, go_terrain.transform);
                    }
                    Instantiate<GameObject>(prefab_construction,go_building.transform.position, Quaternion.Euler(0, 0, 0), go_building.transform);
                    break;
                case Building.Type.QUARRY:
                    // The quarry should be rotated towards the path that the donkeys will take
                    possibleQuarryRotation = Quaternion.LookRotation(path[path.Count - 2].transform.position - transform.position, Vector3.up);
                    Instantiate<GameObject>(prefab_quarryPosition, go_building.transform.position, Quaternion.Euler(0, 0, 0), go_building.transform);
                    break;
                case Building.Type.WINDMILL:
                    Destroy(go_decoration);
                    Instantiate<GameObject>(prefab_construction, go_building.transform.position, Quaternion.Euler(0, 0, 0), go_building.transform);
                    break;
                case Building.Type.GRAIN:
                    Destroy(go_decoration);
                    Instantiate<GameObject>(prefab_acre, go_building.transform.position, Quaternion.Euler(0, 0, 0), go_building.transform);
                    break;
                case Building.Type.TOWER:
                    Destroy(go_decoration);
                    Instantiate<GameObject>(prefab_construction, go_building.transform.position, Quaternion.Euler(0, 0, 0), go_building.transform);
                    break;
            }
            animator.SetTrigger("place");
            building = b;
            isConstructionFinished = false;

            // Placing a building triggers a worker travelling to the spot
            Vector3 donkeyDirection = path[1].transform.position - path[0].transform.position;
            donkeyDirection.y = 0;
            donkey = Instantiate<Donkey>(prefab_donkey, path[0].transform.position, Quaternion.LookRotation(donkeyDirection, Vector3.up));
            donkey.SetPath(path);

            foreach (HexCell c in GameHandler.game.estimatedCells) {
                c.ApplyEstimatedGain();
            }

            // Pay ressources
            GameHandler.game.AdjustLumber(-Building.Costs[b].lumber);
            GameHandler.game.AdjustStone(-Building.Costs[b].stone);
            GameHandler.game.AdjustWheat(-Building.Costs[b].wheat);
            GameHandler.game.UpdateRessourceDisplay();

            return true;
        } else return false;
    }

    public void SetCastle() {
        Instantiate(prefab_castle, go_building.transform.position, Quaternion.identity, go_building.transform);
        building = Building.Type.CASTLE;
        isConstructionFinished = true;
    }

    public bool IsTraversable() {
        if (type == Type.MOUNTAINS) return false;
        if (building != Building.Type.NONE && building != Building.Type.GRAIN) return false;
        return true;
    }

    public int GetResourceRadius() {
        return resourceRadius;
    }

    public int GetResourceGain() {
        return resourceGain;
    }

    public void ApplyEstimatedGain() {
        resourceGain = estimatedResourceGain;
    }

    public void Focus() {
        animator.SetBool("focus", true);
    }

    public void LooseFocus() {
        animator.SetBool("focus", false);
        HideResourceGain();
    }

    public void ToggleFocus() {
        if (GameHandler.game.focusedCell != this) {
            if (GameHandler.game.focusedCell != null) {
                GameHandler.game.focusedCell.LooseFocus();
            }
            GameHandler.game.focusedCell = this;
            Focus();
            ShowResourceGain();
        } else {
            GameHandler.game.focusedCell = null;
            LooseFocus();
        }
    }

    public void PlayBuildSound() {
        go_building.GetComponent<PlaySound>().Play();
    }

    public void PlaySmokeParticles() {
        go_terrain.GetComponent<ParticleSystem>().Play();
    }

    public Type GetCellType() {
        return type;
    }

    public Building.Type GetBuilding() {
        return building;
    }

    public void SetHeightLevel(int level) {
        heightLevel = level;
    }

    public void EndRound() {
        if (donkey != null) {
            donkey.Move();
        }
    }

    public void DonkeyArrived(GameHandler.Resources resourceType, int amount) {
        if (building == Building.Type.CASTLE) {
            // Destination: Castle
            // The castle will always be the drop off point for new resources
            switch (resourceType) {
                case GameHandler.Resources.LUMBER:
                    GameHandler.game.AdjustLumber(amount);
                    break;
                case GameHandler.Resources.STONE:
                    GameHandler.game.AdjustStone(amount);
                    break;
                case GameHandler.Resources.WHEAT:
                    GameHandler.game.AdjustWheat(amount);
                    break;
            }
        } else {
            // Destination: Building
            // Buildings produce resources
            if (!isConstructionFinished) {
                // The first arriving donkey finishes construction work
                isConstructionFinished = true;
                Destroy(go_building.transform.GetChild(0).gameObject);
                switch (building) {
                    case Building.Type.SAWMILL:
                        Instantiate<GameObject>(prefab_sawmill, go_building.transform.position, Quaternion.Euler(0, 180, 0), go_building.transform);
                        break;
                    case Building.Type.QUARRY:
                        Destroy(go_decoration);
                        Instantiate<GameObject>(prefab_quarry, go_building.transform.position, possibleQuarryRotation, go_building.transform);
                        break;
                    case Building.Type.WINDMILL:
                        Instantiate<GameObject>(prefab_windmill, go_building.transform.position, Quaternion.Euler(0, 180, 0), go_building.transform);
                        break;
                    case Building.Type.GRAIN:
                        Instantiate<GameObject>(prefab_grain, go_building.transform.position, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), go_building.transform);
                        // Grain fields do not need their own donkey once grown
                        Destroy(donkey.gameObject);
                        donkey = null;
                        break;
                    case Building.Type.TOWER:
                        Instantiate<GameObject>(prefab_tower, go_building.transform.position, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), go_building.transform);
                        // Towers do not need their own donkey once built
                        Destroy(donkey.gameObject);
                        donkey = null;
                        break;
                }
            }
            // The donkey picks up another load of resources
            if (donkey != null) {
                donkey.SetLoad(Building.Products[building], resourceGain);
            }
        }
    }

    // Previews and estimations --------------------------------------------

    public Building.Type GetPreviewBuilding() {
        return buildingPreview;
    }

    /// <summary>
    /// Estimate the resource gain if the given building is placed. This also triggers new estimations
    /// for all buildings inside this cell's building radius.
    /// </summary>
    /// <param name="b">The building that is eventually to be build</param>
    /// <param name="origin">If true, this cell is the origin of the estimation and will trigger nearby buildings to also estimate</param>
    public void EstimateResourceGain(Building.Type b, bool origin) {
        // Castles, Towers and Grain will never be estimated
        if (building == Building.Type.CASTLE || building == Building.Type.GRAIN || building == Building.Type.TOWER) return;

        // Caution: This must be called before repeating the CalculateBuildingOutput() function
        GameHandler.game.estimatedCells.Add(this);

        go_estimation.SetActive(true);
        buildingPreview = b;
        estimatedResourceGain = GameHandler.game.grid.CalculateBuildingOutput(coordinates.X, coordinates.Z, b, resourceRadius, origin);
        resourceText.SetText(estimatedResourceGain.ToString());
    }

    public void ResetBuildingPreview() {
        buildingPreview = Building.Type.NONE;
        go_estimation.SetActive(false);
        // Revert text back to the actual resource gain
        estimatedResourceGain = -1;
        resourceText.SetText(resourceGain.ToString());
    }

    public void ShowResourceGain() {
        go_estimation.SetActive(true);
    }

    public void HideResourceGain() {
        go_estimation.SetActive(false);
    }
}
