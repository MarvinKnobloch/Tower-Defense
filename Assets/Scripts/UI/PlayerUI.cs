using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    public RessourceManager ressourceManager;

    [Space]
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject buildingMenuButton;

    [Space]
    public GameObject buildingValuesObj;
    private GameObject currentSelectedBuilding;
    private BuildingValues buildingValues;
    [SerializeField] private GameObject selectCircle;
    [SerializeField] private GameObject rangeCircle;

    [Space]
    [SerializeField] private GameObject buildingTab;
    [SerializeField] private TextMeshProUGUI buildingTabName;
    [SerializeField] private TextMeshProUGUI buildingTabStats;
    [SerializeField] private TextMeshProUGUI buildingTabCosts;
    public bool hoverOverBuildingSelector;

    [Space]
    public GameObject levelTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        buildingValues = buildingValuesObj.GetComponent<BuildingValues>();
    }
    public void ActivateBuildingValues(GameObject building)
    {
        currentSelectedBuilding = building;
        buildingValuesObj.SetActive(true);
        BuildingObj buildingObj = building.GetComponent<Building>().buildingObj;

        selectCircle.transform.localScale = buildingObj.buildingSize;
        selectCircle.transform.position =  building.transform.position;
        selectCircle.SetActive(true);

        DrawRangeCicrle(building.transform.position, buildingObj);

        buildingValues.SetCurrentBuildingUI(buildingObj, building);
    }
    public void DrawRangeCicrle(Vector3 position, BuildingObj buildingObj)
    {
        rangeCircle.SetActive(true);
        rangeCircle.transform.position = position;

        rangeCircle.GetComponent<RangeCircle>().DrawCircle(buildingObj.attackRange * 0.01f);
    }
    public void SetBuildingSelectorValueBuilding(GameObject building)
    {
        GetComponent<BuildingSelector>().currentValuesUIBuilding = building;
    }
    public void CloseBuildingValues()
    {
        buildingValuesObj.SetActive(false);
        selectCircle.SetActive(false);
        DisableRangeCircle();
        if(hoverOverBuildingSelector == false) CloseBuildingTab();
        currentSelectedBuilding = null;
    }
    public void ResetRangeCircle()
    {
        if(currentSelectedBuilding != null)
        {
            DrawRangeCicrle(currentSelectedBuilding.transform.position, currentSelectedBuilding.GetComponent<Building>().buildingObj);
        }
        else
        {
            DisableRangeCircle();
        }
    }
    public void DisableRangeCircle()
    {
        rangeCircle.SetActive(false);
    }
    public void ActivateBuildingMenu()
    {
        buildingMenu.SetActive(true);
        buildingMenuButton.SetActive(false);
    }
    public void CloseBuildingMenu()
    {
        buildingMenu.SetActive(false);
        buildingMenuButton.SetActive(true);
    }
    public void ActivateBuildingTab(BuildingObj buildingObj)
    {
        SetTowerValueText(buildingTabName, buildingTabStats, buildingObj);

        if (ressourceManager.CheckForMoney(buildingObj.Costs))
        {
            buildingTabCosts.text = "<color=green>" + buildingObj.Costs + " Money";
        }
        else
        {
            buildingTabCosts.text = "<color=red>" + buildingObj.Costs + "</color> Money";
        }

        buildingTab.SetActive(true);
    }
    public void SetTowerValueText(TextMeshProUGUI nameText, TextMeshProUGUI statsText, BuildingObj buildingObj)
    {
        nameText.text = buildingObj.name;
        statsText.text = "Attack: " + buildingObj.attackDamage +
                             "\nAttack Speed: " + buildingObj.attackSpeed +
                             "\nRange: " + buildingObj.attackRange +
                             "\nAttack: " + buildingObj.attackTargetType;

        switch (buildingObj.attackTargetType)
        {
            case BuildingObj.Attack.MultiShot:
                statsText.text += "(" + buildingObj.shotCount + ")";
                break;
            case BuildingObj.Attack.BounceShot:
                statsText.text += "(" + buildingObj.shotCount + ")";
                break;
            case BuildingObj.Attack.Ground:
                statsText.text += "(" + buildingObj.groundAoeSize + ")";
                break;
        }
        statsText.text += "\nType: " + buildingObj.attackType;
    }
    public void CloseBuildingTab()
    {
        buildingTab.SetActive(false);
    }
}
