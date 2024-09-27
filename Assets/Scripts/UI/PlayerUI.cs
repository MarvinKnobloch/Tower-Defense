using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    public RessourceManager ressourceManager;

    [Space]
    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private GameObject buildingMenuButton;

    [Space]
    [SerializeField] private GameObject buildingValuesObj;
    private BuildingValues buildingValues;
    [SerializeField] private GameObject selectCircle;
    [SerializeField] private GameObject rangeCircle;


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
        buildingValuesObj.SetActive(true);
        BuildingObj buildingObj = building.GetComponent<Building>().buildingObj;

        selectCircle.transform.localScale = buildingObj.buildingSize;
        selectCircle.transform.position =  building.transform.position;
        selectCircle.SetActive(true);

        rangeCircle.SetActive(true);
        rangeCircle.transform.position = building.transform.position;

        rangeCircle.GetComponent<RangeCircle>().DrawCircle(buildingObj.range * 0.01f);

        buildingValues.ValuesTextUpdate(buildingObj);
    }
    public void CloseBuildingValues()
    {
        buildingValuesObj.SetActive(false);
        selectCircle.SetActive(false);
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
}
