using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class ClickHandler : MonoBehaviour
{
    public GameObject buildingAid;
    public TMP_Text buildingAidText;
    private Mouse mouse;
    private Camera cam;
    private bool leftMouseDown;
    private bool rightMouseDown;
    private float mouseDownTimer;
    private Vector3 thisMousePos;
    private Vector3 lastDragPoint;
    private Plane dragPlane;

    void Awake() {
        cam = Camera.main;
        leftMouseDown = false;
        rightMouseDown = false;
        mouseDownTimer = 0f;
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
            // Right mouse CLICK
            if (mouseDownTimer < 0.2f) {
                EndBuildingMode();
            }
            rightMouseDown = false;
            mouseDownTimer = 0f;
        }
        if (rightMouseDown) {
            mouseDownTimer += Time.deltaTime;
        }
        // Build menu
        if (GameHandler.game.building != HexCell.Building.NONE) {
            SetBuildingAid(thisMousePos);
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
        // Prevent the player from clicking cells through UI elements
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        // Check if and what cell was clicked
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            //print(hit.collider.tag);
            switch (hit.collider.tag) {
                case "Terrain":
                    HexCell cell = hit.collider.GetComponentInParent<HexCell>();

                    // If in building mode, try to build
                    if (GameHandler.game.building != HexCell.Building.NONE) {
                        cell.Build(GameHandler.game.building);
                        EndBuildingMode();
                    }
                    break;
            }
        }
    }

    private void RightDrag(Vector3 dragDirection) {
        // Drags the camera
        cam.transform.position = new Vector3(cam.transform.position.x - dragDirection.x, cam.transform.position.y, cam.transform.position.z - dragDirection.z);
    }

    private void SetBuildingAid(Vector3 mousePosition) {
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            // Only show building aid on cells
            if (!hit.collider.CompareTag("Terrain")) {
                if (buildingAid.activeInHierarchy) RemoveBuildingAid();
                return;
            }
            
            HexCell cell = hit.collider.GetComponentInParent<HexCell>();
            // Only show building aid on valid cells
            if (!cell.CanBuild(GameHandler.game.building)) {
                if (buildingAid.activeInHierarchy) RemoveBuildingAid();
                return;
            }

            // Show building aid
            if (!buildingAid.activeInHierarchy) buildingAid.SetActive(true);
            // Update building aid position and resource estimation
            if (GameHandler.game.focusedCell != cell) {
                HideAllBuildingPreviews();
                buildingAid.transform.parent = cell.go_terrain.transform;
                buildingAid.transform.position = cell.go_terrain.transform.position;
                UpdateBuildingAidEstimation(cell);
                if (GameHandler.game.focusedCell != null) {
                    GameHandler.game.focusedCell.LooseFocus();
                }
                GameHandler.game.focusedCell = cell;
                cell.Focus();
            }
            
        } else {
            // Nothing was hit with the raycast
            if (buildingAid.activeInHierarchy) RemoveBuildingAid();
            return;
        }
    }

    private void UpdateBuildingAidEstimation(HexCell cell) {
        cell.PreviewResourceGain(GameHandler.game.building);
        //buildingAidText.SetText(cell.GetResourceGain() + "");
    }

    private void EndBuildingMode() {
        GameHandler.game.building = HexCell.Building.NONE;
        EventSystem.current.SetSelectedGameObject(null);
        RemoveBuildingAid();
    }

    private void RemoveBuildingAid() {
        buildingAid.SetActive(false);
        if (GameHandler.game.focusedCell != null) {
            GameHandler.game.focusedCell.LooseFocus();
            GameHandler.game.focusedCell = null;
            HideAllBuildingPreviews();
        }
    }

    private void HideAllBuildingPreviews() {
        for (int i = GameHandler.game.previewedCells.Count - 1; i >= 0; i--) {
            GameHandler.game.previewedCells[i].HideBuildingPreview();
            GameHandler.game.previewedCells.RemoveAt(i);
        }
    }
}
