using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    public HexCell prefab_meadow, prefab_forest, prefab_mountain;
    [Header("Map height")]
    public float layerHeight = 0.3f;
    [Range(0.0f, 1.0f)]
    public float mountainNoiseThreshold;
    [Range(0.01f, 1.0f)]
    public float mountainNoiseScale;
    [Header("Vegetation")]
    [Range(0.0f, 1.0f)]
    public float forestNoiseThreshold;
    [Range(0.01f, 1.0f)]
    public float forestNoiseScale;

    private int width;
    private int height;
    private HexCell[,] cells;
    private int ringCount;
    private float randomNoiseOffsetX, randomNoiseOffsetZ, randomForestOffsetX, randomForestOffsetZ;
    private int heightLevels;


    private void Awake() {
        randomNoiseOffsetX = UnityEngine.Random.Range(0f, 100000f);
        randomNoiseOffsetZ = UnityEngine.Random.Range(0f, 100000f);
        randomForestOffsetX = UnityEngine.Random.Range(0f, 100000f);
        randomForestOffsetZ = UnityEngine.Random.Range(0f, 100000f);
        heightLevels = 4;
    }

    public void GenerateCenteredGrid(int ringCount) {
        this.ringCount = ringCount;
        cells = new HexCell[ringCount * 2 + 1, ringCount * 2 + 1];
        if (ringCount < 0) ringCount = 0;

        CreateCell(0, 0, ringCount, 0);
        for (int ring = 1; ring <= ringCount; ring++) {
            for (int i = 0; i < ring; i++) {
                // middle right to down right
                CreateCell(ring, -i, ringCount, ring);
                // down right to down left
                CreateCell(ring - i, -ring, ringCount, ring);
                // down left to middle left
                CreateCell(-i, -ring + i, ringCount, ring);
                // middle left to top left
                CreateCell(-ring, i, ringCount, ring);
                // top left to top right
                CreateCell(-ring + i, ring, ringCount, ring);
                // top right to middle right
                CreateCell(0 + i, ring - i, ringCount, ring);
            }
        }
        //GetCellAt(1, 0).SetCastle();
        //GetCellAt(-1, 0).SetCastle();

        //GetCellAt(-1, 1).SetCastle();
        //GetCellAt(0, 1).SetCastle();
        //GetCellAt(0, -1).SetCastle();
        //GetCellAt(1, -1).SetCastle();
    }

    void CreateCell(int x, int z, int ringCount, int distanceToCastle) {
        // Map generation
        HexCell.Type type = HexCell.Type.MEADOW;

        float heightValue = 0f;
        // Use perlin noise to pseudo-randomly decide cell type and height
        heightValue = Mathf.PerlinNoise((x + 0.5f * z + randomNoiseOffsetX) * mountainNoiseScale, (z + randomNoiseOffsetZ) * mountainNoiseScale);
        if (heightValue > mountainNoiseThreshold) {
            type = HexCell.Type.MOUNTAINS;
        } else {
            // If it's not a mountain it can become a forest
            float forestValue = Mathf.PerlinNoise((x + 0.5f * z + randomForestOffsetX) * forestNoiseScale, (z + randomForestOffsetZ) * forestNoiseScale);
            if (forestValue > forestNoiseThreshold) {
                type = HexCell.Type.FOREST;
            }
        }
        int heightLevel = (int)(heightValue * heightLevels);


        // Cells close to the castle should always be of type meadow
        if (distanceToCastle < 2) {
            type = HexCell.Type.MEADOW;
        }

        Vector3 position;
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        position.y = heightLevel * layerHeight;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        // Instantiating prefab
        HexCell cell;
        switch (type) {
            case HexCell.Type.FOREST:
                cell = cells[x + ringCount, z + ringCount] = Instantiate<HexCell>(prefab_forest);
                break;
            case HexCell.Type.MOUNTAINS:
                cell = cells[x + ringCount, z + ringCount] = Instantiate<HexCell>(prefab_mountain);
                break;
            default:
                cell = cells[x + ringCount, z + ringCount] = Instantiate<HexCell>(prefab_meadow);
                // Meadows on a certain height level are rocky (This is just a visual effect
                break;
        }
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = new HexCoordinates(x, z);
        cell.SetHeightLevel(heightLevel);

        if (distanceToCastle == 0) {
            cell.SetCastle();
        }

        cell.Decorate();
    }

    /// <summary>
    /// Returns the HexCell at the given coordinate or null if there is no HexCell at that position.
    /// </summary>
    /// <param name="x">The x coordinate of the cell</param>
    /// <param name="z">The z coordinate of the cell</param>
    /// <returns>HexCell at (x, z) if present, else null</returns>
    public HexCell GetCellAt(int x, int z) {
        x += ringCount;
        z += ringCount;
        if (x < 0 || z < 0 || x >= cells.GetLength(0) || z >= cells.GetLength(1))
            return null;
        return cells[x, z];
    }

    public List<HexCell> GetNeighbours(int x, int z) {
        List<HexCell> neighbours = new List<HexCell>();
        HexCell c = GetCellAt(x + 1, z);
        if (c != null) neighbours.Add(c);
        c = GetCellAt(x - 1, z);
        if (c != null) neighbours.Add(c);
        c = GetCellAt(x - 1, z + 1);
        if (c != null) neighbours.Add(c);
        c = GetCellAt(x, z + 1);
        if (c != null) neighbours.Add(c);
        c = GetCellAt(x, z - 1);
        if (c != null) neighbours.Add(c);
        c = GetCellAt(x + 1, z - 1);
        if (c != null) neighbours.Add(c);
        return neighbours;
    }

    /// <summary>
    /// This method calculates the shortest path from one cell to another cell, given that there is a possible path.
    /// The A* Algorithm is used for that.
    /// </summary>
    /// <param name="end">The destination cell</param>
    /// <param name="start">Optional: The start cell. Leave empty to use map origin as start.</param>
    /// <returns></returns>
    public List<HexCell> FindPath(HexCell end, HexCell start = null) {
        List<PathCell> openList = new List<PathCell>();
        List<PathCell> closedList = new List<PathCell>();
        if (start == null) {
            openList.Add(new PathCell(0, 0));
        } else {
            openList.Add(new PathCell(start.coordinates.X, start.coordinates.Z));
        }

        while (openList.Count > 0) {
            // Find cell with least f
            PathCell q = openList[0];
            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].GetF() < q.GetF()) {
                    q = openList[i];
                }
            }

            openList.Remove(q);

            foreach (HexCell c in GetNeighbours(q.X, q.Z)) {
                // Set the parent so that the best path can be recreated in the end
                PathCell successor = new PathCell(c.coordinates.X, c.coordinates.Z);
                successor.SetParent(q);

                // Check if the end was reached
                if (successor.X == end.coordinates.X && successor.Z == end.coordinates.Z) {
                    List<HexCell> path = new List<HexCell>();
                    PathCell current = successor;
                    while (current != null) {
                        path.Add(GetCellAt(current.X, current.Z));
                        current = current.GetParent();
                    }
                    path.Reverse();
                    return path;
                }

                // Check if cell is traversable. Destination cells are never traversable but still have to be considered,
                // so this condition is only checked AFTER checking if the destination was reached.
                if (!c.IsTraversable()) {
                    continue;
                }

                successor.SetG(q.GetG() + 1 /* TODO: replace by distance factors (like roads)*/);
                successor.SetH((GetCellAt(successor.X, successor.Z).transform.position - end.transform.position).magnitude);
                successor.SetF(successor.GetG() + successor.GetH());

                // Only continue with this cell (aka successor) if it wasn't visited with a lower f value already
                bool skipSuccessor = false;
                foreach (PathCell other in openList) {
                    if (other.X == successor.X && other.Z == successor.Z) {
                        if (other.GetF() < successor.GetF()) {
                            skipSuccessor = true;
                            break;
                        }
                    }
                }
                if (skipSuccessor) {
                    continue;
                }
                foreach (PathCell other in closedList) {
                    if (other.X == successor.X && other.Z == successor.Z) {
                        if (other.GetF() < successor.GetF()) {
                            skipSuccessor = true;
                            break;
                        }
                    }
                }
                if (skipSuccessor) {
                    continue;
                }

                openList.Add(successor); // TODO: if algorithm not working try adding the other node instead??
            }

            closedList.Add(q);
        }

        // There is no path to the destination
        return null;
    }

    public void EndRound() {
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int z = 0; z < cells.GetLength(1); z++) {
                if (cells[x, z] == null) continue;

                cells[x, z].EndRound();
            }
        }
    }

    /// <summary>
    /// Calculates the amount of resources that a building generates based on the radius that the building covers
    /// </summary>
    /// <param name="building">The type of building that generates the r</param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public int CalculateBuildingOutput(int x, int z, HexCell.Building building, int radius) {
        int output = 0;
        for (int ring = 1; ring <= radius; ring++) {
            for (int i = 0; i < ring; i++) {
                // middle right to down right
                output += BuildingOutputHelper(building, GetCellAt(x +ring, z - i));
                // down right to down left
                output += BuildingOutputHelper(building, GetCellAt(x + ring - i, z - ring));
                // down left to middle left
                output += BuildingOutputHelper(building, GetCellAt(x - i, z - ring + i));
                // middle left to top left
                output += BuildingOutputHelper(building, GetCellAt(x - ring, z + i));
                // top left to top right
                output += BuildingOutputHelper(building, GetCellAt(x - ring + i, z + ring));
                // top right to middle right
                output += BuildingOutputHelper(building, GetCellAt(x + i, z + ring - i));
            }
        }
        return Mathf.Max(output, 0);
    }

    private int BuildingOutputHelper(HexCell.Building building, HexCell cell) {
        // No cell means no resource modifier
        if (cell == null) return 0;

        // Buildings on top of cells block the gain of resources.
        // That's why building is prioritized above cell type.
        HexCell.Building cellBuilding = cell.GetBuilding();
        if (cellBuilding != HexCell.Building.NONE) {
            HexCell.RES_BUILDING_MODIFIERS.TryGetValue((building, cellBuilding), out int value1);
            return value1;
        }

        // If the cell has not been built on, the cell type decides the resource gain
        HexCell.RES_TYPE_MODIFIERS.TryGetValue((building, cell.GetCellType()), out int value2);
        return value2;
    }

    //public void AnalyseNeighbouringCells(int x, int z) {
    //    // Look at all neighbouring cells to decide the type of this cell
    //    int forests = 0;
    //    int mountains = 0;
    //    HexCell neighbour = GetCellAt(x + 1, z);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    neighbour = GetCellAt(x - 1, z);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    neighbour = GetCellAt(x - 1, z + 1);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    neighbour = GetCellAt(x, z + 1);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    neighbour = GetCellAt(x, z - 1);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    neighbour = GetCellAt(x + 1, z - 1);
    //    if (neighbour != null) {
    //        if (neighbour.GetCellType() == HexCell.Type.FOREST)
    //            forests++;
    //        if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
    //            mountains++;
    //    }
    //    print(forests + " " + mountains);
    //}
}
