using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBuildingPreview : MonoBehaviour
{
    public GameObject go_sawmill, go_quarry, go_windmill, go_wheat, go_tower;

    public void Switch(Building.Type type) {
        go_sawmill.SetActive(false);
        go_quarry.SetActive(false);
        go_windmill.SetActive(false);
        go_wheat.SetActive(false);
        go_tower.SetActive(false);

        switch (type) {
            case Building.Type.SAWMILL:
                go_sawmill.SetActive(true);
                break;
            case Building.Type.QUARRY:
                go_quarry.SetActive(true);
                break;
            case Building.Type.WINDMILL:
                go_windmill.SetActive(true);
                break;
            case Building.Type.GRAIN:
                go_wheat.SetActive(true);
                break;
            case Building.Type.TOWER:
                go_tower.SetActive(true);
                break;
        }
    }
}
