using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingValues : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingStats;

    public void ValuesTextUpdate(BuildingObj buildingObj)
    {
        buildingName.text = buildingObj.name;
        buildingStats.text = "Attack: " + buildingObj.damage +
                             "\nAttack Speed: " + buildingObj.attackSpeed +
                             "\nRange: " + buildingObj.range;

    }
}
