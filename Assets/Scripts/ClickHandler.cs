using UnityEngine;
using UnityEngine.InputSystem;
public class ClickHandler : MonoBehaviour
{
    private Camera cam;

    void Awake () {
        cam = Camera.main;
    }

    void Update () {
        Mouse mouse = Mouse.current;
        if(mouse.leftButton.wasPressedThisFrame) {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit)) {
                print(hit.collider.tag);
            }
        }
    }
}
