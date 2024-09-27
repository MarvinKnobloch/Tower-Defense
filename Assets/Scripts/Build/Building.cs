using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingObj buildingObj;
    private BoxCollider2D boxCollider;

    public bool isBuild;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(buildingObj.buildingSize.x, buildingObj.buildingSize.y);
    }
}
