using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public HexCell cellPrefab;

    private int width;
    private int height;
    private HexCell[] cells;

    public void GenerateGrid(int width, int height) {
        this.width = width;
        this.height = height;

        cells = new HexCell[height * width];
        int cellCount = 0;
        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, cellCount);
                cellCount++;
            }
        }
    }

    void CreateCell(int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        // Instantiating prefab
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        // As long as I don't have a better algorithm, cells will just get a random type
        cell.SetRandomType();
    }
}
