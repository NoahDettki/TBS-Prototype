using System;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    public HexCell cellPrefab;

    private int width;
    private int height;
    private HexCell[,] cells;

    //public void GenerateGrid(int width, int height) {
    //    this.width = width;
    //    this.height = height;

    //    cells = new HexCell[height * width];
    //    int cellCount = 0;
    //    for (int z = 0; z < height; z++) {
    //        for (int x = 0; x < width; x++) {
    //            CreateCell(x, z, cellCount);
    //            cellCount++;
    //        }
    //    }
    //}

    public void GenerateCenteredGrid(int ringCount) {
        cells = new HexCell[ringCount * 2 + 1, ringCount * 2 + 1];
        if (ringCount < 0) ringCount = 0;

        CreateCell(0, 0, ringCount, true);
        for (int ring = 1; ring <= ringCount; ring++) {
            for (int i = 0; i < ring; i++) {
                // middle right to down right
                CreateCell(ring, -i, ringCount, false);
                // down right to down left
                CreateCell(ring - i, -ring, ringCount, false);
                // down left to middle left
                CreateCell(-i, -ring + i, ringCount, false);
                // middle left to top left
                CreateCell(-ring, i, ringCount, false);
                // top left to top right
                CreateCell(-ring + i, ring, ringCount, false);
                // top right to middle right
                CreateCell(0 + i, ring - i, ringCount, false);
            }
        }
    }

    void CreateCell(int x, int z, int ringCount, Boolean castle) {
        Vector3 position;
        position.x = (x + z * 0.5f) * (HexMetrics.innerRadius * 2f);
        //position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        // Instantiating prefab
        //HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        print(x + " " + z + " => " + (x + ringCount) + " " + (z + ringCount));
        HexCell cell = cells[x + ringCount, z + ringCount] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = new HexCoordinates(x, z);
        //cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        // As long as I don't have a better algorithm, cells will just get a random type
        if (castle) {
            cell.SetCastle();
        } else {
            cell.SetRandomType();
        }
    }
}
