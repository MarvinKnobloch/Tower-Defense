using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action enemyHasDied;
    private Vector3 targetPosition;
    private int currentWayPoint;
    private int maxWayPoints;

    [SerializeField] private float movementSpeed;

    private float WayPointCheckInterval = 0.1f;
    private float checkTimer;

    [Space]
    [SerializeField] private int currenthealth;
    [SerializeField] private int maxHealth;

    [SerializeField] private ArmorType armorType;

    public int Health
    {
        get { return currenthealth; }
        set { currenthealth = Math.Min(Math.Max(0, value), maxHealth); }
    }
    public enum ArmorType
    {
        Cloth,
        Basic,
        Armored,
        Balance,
        Magic,
    }
    private void Awake()
    {
        currentWayPoint = 1;
        Health = maxHealth;
        maxWayPoints = LevelManager.Instance.enemyWayPoints.Length;
        WayPointUpdate();
    }
    private void Update()
    {
        checkTimer += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if(checkTimer >= WayPointCheckInterval)
        {
            checkTimer = 0;

            if(Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                WayPointUpdate();
            }
        }
    }

    private void WayPointUpdate()
    {
        if (currentWayPoint < maxWayPoints)
        {
            targetPosition = LevelManager.Instance.GetWayPoint(currentWayPoint);
            currentWayPoint++;
        }
        else
        {
            OnDeath();
        }
    }
    private void OnDeath()
    {
        enemyHasDied?.Invoke();
        Destroy(gameObject);
    }
    public void HealthChange(int amount, BuildingObj.Type attackType)
    {
        Health += amount;

        if(currenthealth <= 0)
        {
            OnDeath();
        }
    }
}
