using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : MonoBehaviour
{
    private List<HexCell> path;
    private int pathIndex;
    private int cellsPerTurn;
    private float debugTimer;

    private void Awake() {
        cellsPerTurn = 1;
        pathIndex = 0;
    }

    private void Update() {
        debugTimer += Time.deltaTime;
        if ( debugTimer > 1)
        {
            debugTimer -= 1.0f;
            Move();
        }
    }

    public void SetPath(List<HexCell> path) {
        this.path = path;
    }

    public void Move() {
        pathIndex += cellsPerTurn;
        if (pathIndex >= path.Count - 1) {
            return;
            // TODO: the donkey reached the destination
        }
        // Set position to new cell and adjust rotation
        transform.position = path[pathIndex].transform.position;
        Vector3 direction = path[pathIndex + 1].transform.position - path[pathIndex].transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
