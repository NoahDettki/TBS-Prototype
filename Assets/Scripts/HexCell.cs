using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public enum Type { MEADOW, FOREST, MOUNTAINS }
    public enum Building { NONE, CASTLE, SAWMILL, QUARRY, WINDMILL, GRAIN }
    public GameObject prefab_rockyMeadow, prefab_rockyMeadow2,
        prefab_forest, prefab_rockyForest, prefab_lumberForest,
        prefab_mountains, prefab_mountains2, prefab_highMountains, prefab_highMountains2;
    public GameObject prefab_castle, prefab_sawmill, prefab_quarry, prefab_windmill, prefab_grain;
    public HexCoordinates coordinates;
    public GameObject go_terrain, go_building;

    public Donkey prefab_donkey;

    public Type type = Type.MEADOW;
    private Building building;
    private Animator animator;
    private GameObject go_decoration;
    private int heightLevel;
    private Donkey donkey;

    private void Awake() {
        animator = GetComponent<Animator>();
        building = Building.NONE;
    }

    public void Decorate() {
        Quaternion rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
        float rng = UnityEngine.Random.value;
        switch (type) {
            case Type.MEADOW:
                // Meadows with buildings need no decoration
                if (building != Building.NONE)
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
            // Check if this cell is reachable from the castle
            List<HexCell> path = GameHandler.game.grid.FindPath(this);
            if (path == null) return false;

            switch(b) {
                case Building.SAWMILL:
                    Destroy(go_decoration);
                    // Lumbermills in the forest spawn with a few trees around them
                    if (type == Type.FOREST) {
                        go_decoration = Instantiate(prefab_lumberForest, go_terrain.transform.position, Quaternion.identity, go_terrain.transform);
                    }
                    Instantiate<GameObject>(prefab_sawmill,go_building.transform.position, Quaternion.Euler(0, 180, 0), go_building.transform);
                    break;
                case Building.QUARRY:
                    //go_building = Instantiate<GameObject>(prefab_sawmill, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
                    break;
            }
            animator.SetTrigger("place");
            building = b;

            // Placing a building triggers a worker travelling to the spot
            Vector3 donkeyDirection = path[1].transform.position - path[0].transform.position;
            donkeyDirection.y = 0;
            donkey = Instantiate<Donkey>(prefab_donkey, path[0].transform.position, Quaternion.LookRotation(donkeyDirection, Vector3.up));
            donkey.SetPath(path);

            return true;
        } else return false;
    }

    public void SetCastle() {
        Instantiate(prefab_castle, go_building.transform.position, Quaternion.identity, go_building.transform);
        building = Building.CASTLE;
    }

    public bool IsTraversable() {
        if (type == Type.MOUNTAINS) return false;
        if (building != Building.NONE) return false;
        return true;
    }

    public void Focus() {
        animator.SetBool("focus", true);
    }

    public void LooseFocus() {
        animator.SetBool("focus", false);
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

    public void SetHeightLevel(int level) {
        heightLevel = level;
    }
}
