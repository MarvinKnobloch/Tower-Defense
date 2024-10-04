using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BuildingObj buildingObj;

    public BuildingSelector buildingSelector;
    private Image buttonImage;

    private bool canAffordBuilding;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }
    private void OnEnable()
    {
        RessourceManager.UIUpdate += ButtonUIUpdate;
        ButtonUIUpdate();
    }
    private void OnDisable()
    {
        RessourceManager.UIUpdate -= ButtonUIUpdate;
    }
    public void CreateBuildingButton()
    {
        if (canAffordBuilding == false) return;

        buildingSelector.CreateBuildingPrefab(buildingObj);
    }
    public void ButtonUIUpdate()
    {
        if (PlayerUI.Instance.ressourceManager.CheckForMoney(buildingObj.Costs))
        {
            canAffordBuilding = true;
            buttonImage.color = Color.green;
        }
        else
        {
            canAffordBuilding = false;
            buttonImage.color = Color.red;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerUI.Instance.hoverOverBuildingSelector = true;
        PlayerUI.Instance.ActivateBuildingTab(buildingObj);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerUI.Instance.CloseBuildingTab();
        PlayerUI.Instance.hoverOverBuildingSelector = false;
    }
}
