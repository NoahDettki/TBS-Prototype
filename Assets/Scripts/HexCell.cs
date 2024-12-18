using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public enum Type { MEADOW, FOREST, MOUNTAINS }

    public GameObject prefab_rockyMeadow, prefab_rockyMeadow2,
        prefab_forest, prefab_rockyForest, prefab_lumberForest,
        prefab_mountains, prefab_mountains2, prefab_highMountains, prefab_highMountains2;
    public GameObject prefab_construction, prefab_castle, prefab_sawmill, prefab_quarry, prefab_windmill, prefab_grain;
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
                    //go_building = Instantiate<GameObject>(prefab_sawmill, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
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

            resourceGain = estimatedResourceGain;

            // Pay ressources
            GameHandler.game.AjustLumber(-Building.Costs[b].lumber);
            GameHandler.game.AjustStone(-Building.Costs[b].stone);
            GameHandler.game.AjustWheat(-Building.Costs[b].wheat);
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
        if (building != Building.Type.NONE) return false;
        return true;
    }

    //public void UpdateResourceGain() {
    //    resourceGain = GameHandler.game.grid.CalculateBuildingOutput(coordinates.X, coordinates.Z, building, resourceRadius);
    //    resourceText.SetText(resourceGain.ToString());
    //}

    public int GetResourceRadius() {
        return resourceRadius;
    }

    public int GetResourceGain() {
        return resourceGain;
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

    public void DonkeyArrived() {
        if (!isConstructionFinished) {
            // The first arriving donkey finishes construction work
            isConstructionFinished = true;
            Destroy(go_building.transform.GetChild(0).gameObject);
            switch (building) {
                case Building.Type.SAWMILL:
                    Instantiate<GameObject>(prefab_sawmill, go_building.transform.position, Quaternion.Euler(0, 180, 0), go_building.transform);
                    break;
            }
        }

        // DEBUG
        Destroy(donkey.gameObject);
        donkey = null;
    }

    // Previews and estimations --------------------------------------------

    //public void SetBuildingPreview(Building preview) {
    //    buildingPreview = preview;
    //}

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
        // Castle will never be estimated
        if (building == Building.Type.CASTLE) return;

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
