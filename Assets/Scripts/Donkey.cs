using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : MonoBehaviour
{
    private List<HexCell> path;
    private int pathDir; // either 1 or -1
    private int pathIndex;
    private int cellsPerTurn;
    private GameHandler.Resources loadType;
    private int loadAmount;
    private Queue<IEnumerator> moveQueue = new Queue<IEnumerator>();
    private bool leavingAnimation = false;

    private void Awake() {
        cellsPerTurn = 2;
        pathDir = 1;
        pathIndex = 0;
    }

    private void Start() {
        StartCoroutine(CoroutineCoordinator());
    }

    public void SetPath(List<HexCell> path) {
        this.path = path;
    }

    public void SetLoad(GameHandler.Resources loadType, int loadAmount) {
        this.loadType = loadType;
        this.loadAmount = loadAmount;
    }

    public void SwitchDirection() {
        pathDir *= -1;
        leavingAnimation = true;
    }

    public void Move() {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        // Even when the donkey can move futher than one field, moves are handled one after another
        for (int i = 0; i < cellsPerTurn; i++) {
            pathIndex += pathDir;

            if ((pathDir == 1 && pathIndex == path.Count - 1)
                || (pathDir == -1 && pathIndex == 0)) {
                // The donkey reached the destination, so it must rotate 180 degrees
                //Quaternion newRotation = rot * Quaternion.Euler(0, 180, 0);
                moveQueue.Enqueue(CellTransition(pos, rot, path[pathIndex].transform.position, rot, 1f/cellsPerTurn, 1));
                SwitchDirection();
                break;
            } else {
                // The donkey has not yet reached the destionation. New Rotation is dependent on further path cells
                Vector3 newDirection = path[pathIndex + pathDir].transform.position - path[pathIndex].transform.position;
                newDirection.y = 0;
                Quaternion newRotation = Quaternion.LookRotation(newDirection, Vector3.up);
                int transitionOptions = 0;
                // If the donkey reached the destination last move it has to rotate 180 degrees BEFORE making its next move
                if (leavingAnimation) {
                    rot *= Quaternion.Euler(0, 180, 0);
                    transitionOptions = -1;
                    leavingAnimation = false;
                }

                moveQueue.Enqueue(CellTransition(pos, rot, path[pathIndex].transform.position, newRotation, 1f / cellsPerTurn, transitionOptions));

                // Update helper position and rotation
                pos = path[pathIndex].transform.position;
                rot = newRotation;
            }
        }
    }

    IEnumerator CoroutineCoordinator() {
        while (true) {
            while (moveQueue.Count > 0)
                yield return StartCoroutine(moveQueue.Dequeue());
            yield return null;
        }
    }

    /// <summary>
    /// This transistion is played for every two hex cells the donkey moves between.
    /// </summary>
    /// <param name="p1">start position</param>
    /// <param name="r1">start rotation</param>
    /// <param name="p2">target position</param>
    /// <param name="r2">target rotation</param>
    /// <param name="timer">duration of the transition in seconds</param>
    /// <param name="isArrivingOrLeaving">wheter this is the transition that is performed from or to a building. -1: leaving, 0: traveling, 1: arriving</param>
    /// <returns></returns>
    IEnumerator CellTransition(Vector3 p1, Quaternion r1, Vector3 p2, Quaternion r2, float timer, int isArrivingOrLeaving) {
        float transistionTime = 0f;
        while (transistionTime < timer) {
            if (transistionTime > timer) transistionTime = timer;
            transform.position = Vector3.Lerp(p1, p2, transistionTime / timer);
            transform.rotation = Quaternion.Lerp(r1, r2, Mathf.Pow(transistionTime / timer, 4.0f));
            // When the donkey starts from a building it grows in size
            if (isArrivingOrLeaving == -1) {
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, transistionTime / timer);
            } else if (isArrivingOrLeaving == 1) {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, transistionTime / timer);
            }
            transistionTime += Time.deltaTime;
            yield return null;
        }
        if (isArrivingOrLeaving == 1) {
            path[pathIndex].DonkeyArrived(loadType, loadAmount);
        }
    }
}
