using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Building : MonoBehaviour
{
    public BuildingObj buildingObj;
    private BoxCollider2D boxCollider;
    private float towerRange;
    [SerializeField] private LayerMask enemyLayer;

    [NonSerialized] public bool isBuild;
    private bool isAttacking;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        boxCollider.size = new Vector2(buildingObj.buildingSize.x, buildingObj.buildingSize.y);
    }
    private void Start()
    {
        towerRange = buildingObj.attackRange * 0.01f;
        GetComponent<CircleCollider2D>().radius = towerRange;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBuild == true)
        {
            if (isAttacking) return;

            if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
            {
                isAttacking = true;
                InvokeRepeating("SetTarget", 0, buildingObj.attackSpeed);
            }

        }
    }
    public void OnBuild()
    {
        //Sell und Build um CD zu überspringen ist möglich
        isAttacking = true;
        InvokeRepeating("SetTarget", buildingObj.attackSpeed, buildingObj.attackSpeed);

        //Wenn kein Gegner in der Nähe ist, wird der Invoke dirket wieder gecancelt
        SetTarget();

    }
    private void SetTarget()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, towerRange, enemyLayer);

        if (colls.Length != 0)
        {
            switch (buildingObj.attackTargetType)
            {
                case BuildingObj.Attack.SingleTarget:
                    SingleTarget(colls);
                    break;
                case BuildingObj.Attack.MultiShot:
                    MultiShot(colls);
                    break;
                case BuildingObj.Attack.BounceShot:
                    SingleTarget(colls);
                    break;
                case BuildingObj.Attack.Ground:
                    SingleTarget(colls);
                    break;
            }
        }
        else
        {
            CancelInvoke();
            isAttacking = false;
        }
    }
    private void SingleTarget(Collider2D[] colls)
    {
        GameObject target = null;
        float clostestTargetRange = 100;
        foreach (Collider2D coll in colls)
        {
            float distance = Vector2.Distance(transform.position, coll.transform.position);
            if (distance < clostestTargetRange)
            {
                clostestTargetRange = distance;
                target = coll.gameObject;
            }
        }
        Shoot(target);
    }
    private void MultiShot(Collider2D[] colls)
    {
        if(colls.Length > buildingObj.shotCount)
        {
            colls = colls.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToArray();
            for (int i = 0; i < buildingObj.shotCount; i++)
            {
                Debug.Log(i);
                Shoot(colls[i].gameObject);
            }
        }
        else
        {
            for (int i = 0; i < colls.Length; i++)
            {
                Shoot(colls[i].gameObject);
            }
        }
    }
    private void Shoot(GameObject target)
    {
        GameObject projectileObj = Instantiate(buildingObj.projectilePrefab, transform.position, Quaternion.identity);
        projectileObj.GetComponent<Projectile>().SetValues(buildingObj, target);
        //projectile.SetValues(buildingObj, target);
    }
}
