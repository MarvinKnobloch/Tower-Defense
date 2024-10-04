using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingValues : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingStats;

    [Space]
    [SerializeField] private Transform upgradeGrid;
    [SerializeField] private GameObject upgradeButton;

    public void SetCurrentBuildingUI(BuildingObj buildingObj, GameObject building)
    {
        PlayerUI.Instance.SetTowerValueText(buildingName, buildingStats, buildingObj);

        if (upgradeGrid.transform.childCount != 0)
        {
            for (int i = upgradeGrid.childCount - 1; i >= 0; i--)
            {
                Destroy(upgradeGrid.GetChild(i).gameObject);
            }
        }

        if (buildingObj.upgrades.Length != 0)
        {
            for (int i = 0; i < buildingObj.upgrades.Length; i++)
            {
                GameObject buttonPrefab = Instantiate(upgradeButton, Vector3.zero, Quaternion.identity, upgradeGrid.transform);
                buttonPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildingObj.upgrades[i].BuildingName;

                UpgradeButton buildingButton = buttonPrefab.GetComponent<UpgradeButton>();
                buildingButton.buildingObj = buildingObj.upgrades[i];
                buildingButton.currentSelectedBuilding = building;

                Button button = buttonPrefab.GetComponent<Button>();
                button.onClick.AddListener(() => buildingButton.CreateBuilding());

                buttonPrefab.SetActive(true);
            }
        }
    }
}
