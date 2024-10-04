using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName = "Building")]
public class BuildingObj : ScriptableObject
{
    [Header("General")]
    public string BuildingName;
    public int ID;
    public int Costs;
    public GameObject prefab;
    public Vector3Int buildingSize;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Sprite projectileSprite;

    [Header("Values")]
    public int attackDamage;
    public float attackSpeed;
    public float attackRange;

    [Header("Attack")]
    public Attack attackTargetType;
    public int shotCount;

    [Space]
    public bool dealAoeDamage;
    public float ontargetAoeSize;
    public float groundAoeSize;
    public float aoeDamageReduction;

    public Type attackType;

    public enum Attack
    {
        SingleTarget,
        MultiShot,
        BounceShot,
        Ground,
        Cast,
    }
    public enum Type
    {
        Pierce,
        Normal,
        Heavy,
        Balance,
        Cast,   
    }

    [Header("Upgrades")]
    public BuildingObj[] upgrades;
}
