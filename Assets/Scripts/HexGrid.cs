using System;
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

    public HexCell GetCellAt(int x, int z) {
        x += ringCount;
        z += ringCount;
        if (x < 0 || z < 0 || x >= cells.GetLength(0) || z >= cells.GetLength(1))
            return null;
        return cells[x, z];
    }

    public void AnalyseNeighbouringCells(int x, int z) {
        // Look at all neighbouring cells to decide the type of this cell
        int forests = 0;
        int mountains = 0;
        HexCell neighbour = GetCellAt(x + 1, z);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        neighbour = GetCellAt(x - 1, z);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        neighbour = GetCellAt(x - 1, z + 1);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        neighbour = GetCellAt(x, z + 1);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        neighbour = GetCellAt(x, z - 1);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        neighbour = GetCellAt(x + 1, z - 1);
        if (neighbour != null) {
            if (neighbour.GetCellType() == HexCell.Type.FOREST)
                forests++;
            if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS)
                mountains++;
        }
        print(forests + " " + mountains);
    }
}
