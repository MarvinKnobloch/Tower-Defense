using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private BuildingObj buildingObj;
    private GameObject currenttarget;
    private Vector3 direction;
    private bool targetHasDied;
    private float distanceCheckTimer;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask hitLayer;
    private int damage;

    //Area
    private Vector3 areaTarget;
    private float areaDistanceCheckTime = 0.1f;
    private float areaRange;

    //Bounce
    [SerializeField] private float bounceRange;
    private int currentBounce;
    private int maxBounce;

    public Type attackType;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }
    public enum Type
    {
        Single,
        Area,
        Bounce,
    }
    void Update()
    {
        switch (attackType)
        {
            case Type.Single:
                SingleTarget();
                break;
            case Type.Area:
                GroundTarget();
                break;
            case Type.Bounce:
                BounceTarget();
                break;
        }
    }
    public void SetValues(BuildingObj _buildingObj, GameObject target)
    {
        buildingObj = _buildingObj;
        spriteRenderer.sprite = buildingObj.projectileSprite;

        currenttarget = target;

        damage = buildingObj.attackDamage;

        areaTarget = target.transform.position;

        currentBounce = 0;
        maxBounce = buildingObj.shotCount;

        switch (buildingObj.attackTargetType)
        {
            case BuildingObj.Attack.SingleTarget:
                attackType = Type.Single;
                break;
            case BuildingObj.Attack.MultiShot:
                attackType = Type.Single;
                break;
            case BuildingObj.Attack.BounceShot:
                attackType = Type.Bounce;
                break;
            case BuildingObj.Attack.Ground:
                attackType = Type.Area;
                transform.right = areaTarget - transform.position;
                currenttarget = null;
                break;
        }
    }
    private void SingleTarget()
    {
        if (currenttarget != null)
        {
            direction = (currenttarget.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, currenttarget.transform.position, projectileSpeed * Time.deltaTime);
            transform.right = currenttarget.transform.position - transform.position;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, projectileSpeed * Time.deltaTime);
            if (targetHasDied == false)
            {
                targetHasDied = true;
                Destroy(gameObject, 0.5f);
            }
        }
    }
    private void BounceTarget()
    {
        if (currenttarget != null)
        {
            direction = (currenttarget.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, currenttarget.transform.position, projectileSpeed * Time.deltaTime);
            transform.right = currenttarget.transform.position - transform.position;
        }
        else
        {
            NextBounce(transform.position);
        }
    }
    private void GroundTarget()
    {
        distanceCheckTimer += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, areaTarget, projectileSpeed * Time.deltaTime);

        if (distanceCheckTimer > areaDistanceCheckTime)
        {
            distanceCheckTimer = 0;
            if(Vector2.Distance(transform.position, areaTarget) < 0.1f)
            {
                DealAoeDamage(transform.position, buildingObj.groundAoeSize * 0.01f);

                Destroy(gameObject);
            }
        }
    }
    private void DealDamageOnTargetPosition()
    {
        if (buildingObj.dealAoeDamage == false)
        {
            currenttarget.GetComponent<Enemy>().HealthChange(-damage, buildingObj.attackType);
        }
        else
        {
            DealAoeDamage(currenttarget.transform.position, buildingObj.ontargetAoeSize * 0.01f);
        }
    }
    private void DealAoeDamage(Vector3 position, float range)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(position, range, hitLayer);

        float aoeDamage = damage * buildingObj.aoeDamageReduction;
        foreach (Collider2D coll in colls)
        {
            if(coll.TryGetComponent(out Enemy enemy))
            {
                if(currenttarget != null)
                {
                    if(coll.gameObject == currenttarget) enemy.HealthChange(-damage, buildingObj.attackType);
                    else enemy.HealthChange(Mathf.RoundToInt(-aoeDamage), buildingObj.attackType);
                }
                else enemy.HealthChange(Mathf.RoundToInt(-aoeDamage), buildingObj.attackType);
            }
        }
    }
    private void NextBounce(Vector3 bouncePosition)
    {
        Debug.Log(currentBounce + "/" + maxBounce);
        if(currentBounce < maxBounce)
        {
            currentBounce++;
            Collider2D[] colls = Physics2D.OverlapCircleAll(bouncePosition, bounceRange, hitLayer);

            if(colls.Length > 0)
            {
                GameObject target = null;
                float clostestTargetRange = 100;
                foreach (Collider2D coll in colls)
                {
                    //if (currenttarget != null)
                    //{
                    //    if (coll.gameObject == currenttarget)
                    //    { 
                    //        continue; 
                    //    }
                    //}

                    float distance = Vector2.Distance(transform.position, coll.transform.position);
                    if (distance < clostestTargetRange)
                    {
                        if (coll.gameObject != currenttarget)
                        {
                            clostestTargetRange = distance;
                            target = coll.gameObject;
                        }
                    }
                }
                if (target != null)
                {
                    currenttarget = target;
                    CheckForImmediatelyCollision();                 
                }
                else Destroy(gameObject);
            }
            else Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
    private void CheckForImmediatelyCollision()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius, hitLayer);

        if (colls.Length > 0)
        {
            foreach (Collider2D coll in colls)
            {
                if (coll.gameObject == currenttarget)
                {
                    Vector3 boundsPosition = currenttarget.transform.position;
                    currenttarget.GetComponent<Enemy>().HealthChange(-damage, buildingObj.attackType);
                    NextBounce(boundsPosition);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (attackType)
        {
            case Type.Single:
                if (collision.gameObject == currenttarget)
                {
                    DealDamageOnTargetPosition();
                    Destroy(gameObject);
                }
                break;
            case Type.Bounce:
                if (collision.gameObject == currenttarget)
                {
                    Vector3 boundsPosition = currenttarget.transform.position;
                    DealDamageOnTargetPosition();
                    NextBounce(boundsPosition);
                }
                break;
            case Type.Area:
                break;
        }
    }
}
