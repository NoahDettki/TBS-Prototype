using UnityEngine;

public class HexCell : MonoBehaviour {
    public enum Type { MEADOW, FOREST, MOUNTAINS }
    public GameObject prefab_forest, prefab_mountains;
    public HexCoordinates coordinates;

    private Type type;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetRandomType() {
        Quaternion rotation;
        switch (Random.Range(0, 3)) {
            case 0:
                type = Type.MEADOW;
                break;
            case 1:
                type = Type.FOREST;
                rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
                Instantiate(prefab_forest, transform.GetChild(0).position, rotation, transform.GetChild(0));
                break;
            case 2:
                type = Type.MOUNTAINS;
                rotation = Quaternion.AngleAxis(60f * Random.Range(0, 6), Vector3.up);
                Instantiate(prefab_mountains, transform.GetChild(0).position, rotation, transform.GetChild(0));
                break;
        }
    }

    public void Click() {
        animator.Play("CellClicked");
    }

    public void LooseFocus() {
        animator.Play("CellLooseFocus");
    }
}
