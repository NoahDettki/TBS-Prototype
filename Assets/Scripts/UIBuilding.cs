using System;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Building.Type type;
    public RectTransform infoSlide;
    public TMP_Text displayName;
    public TMP_Text lumberCost, stoneCost, wheatCost;
    public Image image;
    [Serializable]
    public struct TypedImage
    {
        public Building.Type type;
        public Sprite sprite;
    }
    public TypedImage[] images;
    public float slideSpeed;
    private bool isHovered;
    private float infoSlideStartY;
    private float infoSlideTargetY;

    void Awake() {
        foreach (TypedImage i in images) {
            if (i.type == type) {
                image.sprite = i.sprite;
            }
        }
        displayName.text = Building.DisplayNames[type];
        lumberCost.text = Building.Costs[type].lumber.ToString();
        stoneCost.text = Building.Costs[type].stone.ToString();
        wheatCost.text = Building.Costs[type].wheat.ToString();
        isHovered = false;
        infoSlideStartY = infoSlide.anchoredPosition.y;
        infoSlideTargetY = infoSlide.rect.height;
        //print($"Start-Y: {infoSlideStartY}");
        //print($"Target-Y: {infoSlideStartY + infoSlide.rect.height}");
    }

    void Start() {
        GameHandler.game.uiBuildingOptions.Add(this);
    }

    void Update() {
        if (isHovered) {
            infoSlide.transform.Translate(new Vector3(0, Time.deltaTime * slideSpeed, 0));
            if (infoSlide.transform.position.y > infoSlideTargetY) {
                infoSlide.transform.position = new Vector3(infoSlide.transform.position.x, infoSlideTargetY, infoSlide.transform.position.z);
            }
        } else {
            infoSlide.transform.Translate(new Vector3(0, -Time.deltaTime * slideSpeed, 0));
            if (infoSlide.transform.position.y < infoSlideStartY) {
                infoSlide.transform.position = new Vector3(infoSlide.transform.position.x, infoSlideStartY, infoSlide.transform.position.z);
            }
        }
    }

    public void CheckResourceSufficiency() {
        if (GameHandler.game.GetLumber() < Building.Costs[type].lumber) {
            lumberCost.color = Color.red;
        } else {
            lumberCost.color = Color.white;
        }
        if (GameHandler.game.GetStone() < Building.Costs[type].stone) {
            stoneCost.color = Color.red;
        } else {
            stoneCost.color = Color.white;
        }
        if (GameHandler.game.GetWheat() < Building.Costs[type].wheat) {
            wheatCost.color = Color.red;
        } else {
            wheatCost.color = Color.white;
        }
    }

    public void Click() {
        GameHandler.game.building = this.type;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }
}
