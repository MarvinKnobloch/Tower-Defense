using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName = "Building")]
public class BuildingObj : ScriptableObject
{
    public string BuildingName;
    public int ID;
    public int Costs;

    public GameObject prefab;
    public Vector3Int buildingSize;

    public float damage;
    public float attackSpeed;
    public float range;
}
