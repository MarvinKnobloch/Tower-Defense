using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Levels", menuName = "Level")]
public class LevelObj : ScriptableObject
{
    public int levelNumber;
    public LevelEnemyValues[] levelEnemyValues;
    public float levelScaling;
}

[Serializable]
public struct LevelEnemyValues
{
    public GameObject enemy;
    public int amount;
    public float spawnRate;
}
