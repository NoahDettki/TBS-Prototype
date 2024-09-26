using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void Click() {
        animator.Play("CellClicked");
    }

    public void LooseFocus() {
        animator.Play("CellLooseFocus");
    }
}
