using UnityEngine;
using UnityEngine.InputSystem;
public class ClickHandler : MonoBehaviour
{
    private Mouse mouse;
    private Camera cam;
    private bool leftMouseDown;
    private bool rightMouseDown;
    private Vector3 thisMousePos;
    private Vector3 lastDragPoint;
    private Plane dragPlane;

    void Awake() {
        cam = Camera.main;
        leftMouseDown = false;
        rightMouseDown = false;
        dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update() {
        mouse = Mouse.current;
        thisMousePos = mouse.position.ReadValue();
        // Left mouse
        if (mouse.leftButton.wasPressedThisFrame) {
            leftMouseDown = true;
            LeftClick(thisMousePos);
        }
        if (mouse.leftButton.wasReleasedThisFrame) {
            leftMouseDown = false;
        }
        if (leftMouseDown) {

        }
        // Right mouse
        if (mouse.rightButton.wasPressedThisFrame) {
            rightMouseDown = true;
            // Collide ray with the y plane
            Ray ray = cam.ScreenPointToRay(thisMousePos);
            float enter = 0f;
            if (dragPlane.Raycast(ray, out enter)) {
                lastDragPoint = ray.GetPoint(enter);
            }
        }
        if (mouse.rightButton.wasReleasedThisFrame) {
            rightMouseDown = false;
        }
    }

    private void LateUpdate() {
        // The camera dragging function is called in late update to avoid graphic bugs
        if (rightMouseDown) {
            // Collide ray with the y plane
            Ray ray = cam.ScreenPointToRay(thisMousePos);
            float enter = 0f;
            if (dragPlane.Raycast(ray, out enter)) {
                Vector3 thisDragPoint = ray.GetPoint(enter);
                RightDrag(thisDragPoint - lastDragPoint);
            }
        }
    }

    private void LeftClick(Vector3 mousePosition) {
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            print(hit.collider.tag);
            switch (hit.collider.tag) {
                case "Terrain":
                    HexCell cell = hit.collider.GetComponentInParent<HexCell>();
                    // Cannot click a cell that is already focused
                    if (cell == GameHandler.game.focusedCell)
                        return;
                    // Unfocus last clicked Cell
                    if (GameHandler.game.focusedCell != null) {
                        GameHandler.game.focusedCell.LooseFocus();
                    }
                    // Focus clicked cell
                    cell.Click();
                    GameHandler.game.focusedCell = cell;
                    break;
            }
        }
    }

    private void RightDrag(Vector3 dragDirection) {
        // Drags the camera
        cam.transform.position = new Vector3(cam.transform.position.x - dragDirection.x, cam.transform.position.y, cam.transform.position.z - dragDirection.z);
    }
}
