using System;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    public HexCell prefab_meadow, prefab_forest, prefab_mountain;

    private int width;
    private int height;
    private HexCell[,] cells;
    private int ringCount;

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
        if (distanceToCastle > 1) {
            // Look at all neighbouring cells to decide the type of this cell
            int forests = 0;
            int mountains = 0;
            HexCell neighbour = GetCellAt(x + 1, z);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            neighbour = GetCellAt(x - 1, z);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            neighbour = GetCellAt(x - 1, z + 1);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            neighbour = GetCellAt(x, z + 1);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            neighbour = GetCellAt(x, z - 1);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            neighbour = GetCellAt(x + 1, z - 1);
            if (neighbour != null) {
                if (neighbour.GetCellType() == HexCell.Type.FOREST) forests++;
                if (neighbour.GetCellType() == HexCell.Type.MOUNTAINS) mountains++;
            }
            print(forests + " " + mountains);
            if (UnityEngine.Random.Range(0, 6) <= forests) {
                type = HexCell.Type.FOREST;
            } else if (UnityEngine.Random.Range(0, 6) <= mountains) {
                type = HexCell.Type.MOUNTAINS;
            }
        }

        Vector3 position;
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        position.y = type == HexCell.Type.MOUNTAINS ? 0.3f : 0f;
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
                break;
        }
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = new HexCoordinates(x, z);

        cell.SetType(type);

        if (distanceToCastle == 0) {
            cell.SetCastle();
        }
    }

    public HexCell GetCellAt(int x, int z) {
        x += ringCount;
        z += ringCount;
        if (x < 0 || z < 0 || x >= cells.GetLength(0) || z >= cells.GetLength(1))
            return null;
        return cells[x, z];
    }
}
