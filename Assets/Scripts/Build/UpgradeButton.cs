using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized] public GameObject currentSelectedBuilding;

    [NonSerialized] public BuildingObj buildingObj;

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
    public void CreateBuilding()
    {
        if (canAffordBuilding)
        {
            Vector3 position = currentSelectedBuilding.transform.position;
            Destroy(currentSelectedBuilding);
            PlayerUI.Instance.CloseBuildingTab();

            GameObject newBuilding = Instantiate(buildingObj.prefab, position, Quaternion.identity);
            Building building = newBuilding.GetComponent<Building>();
            building.isBuild = true;

            PlayerUI.Instance.ressourceManager.ChangeMoneyCount(-building.buildingObj.Costs);
            PlayerUI.Instance.SetBuildingSelectorValueBuilding(newBuilding);

            PlayerUI.Instance.ActivateBuildingValues(newBuilding);
        }
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
        PlayerUI.Instance.ActivateBuildingTab(buildingObj);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerUI.Instance.CloseBuildingTab();
    }
}
