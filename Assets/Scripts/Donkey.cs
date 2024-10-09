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
        transform.position = path[pathIndex].transform.position;
    }
}
