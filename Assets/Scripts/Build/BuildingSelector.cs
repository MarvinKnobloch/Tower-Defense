using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class BuildingSelector : MonoBehaviour
{
    private Camera mainCam;
    private Controls controls;

    [SerializeField] private BuildingObj[] availableBuildings;
    [SerializeField] private GameObject buildingGrid;

    [SerializeField] private GameObject buttonPrefab;
    private bool creatingBuilding;
    private GameObject currentBuilding;
    [SerializeField] private Vector3Int currentBuildingSize;
    [SerializeField] private Tilemap unUsedTileMap;
    [SerializeField] private GameObject usedTileMap;

    private List<Vector3Int> currentColoredTiles = new List<Vector3Int>();
    private Vector3Int currentTile;
    private bool canPlaceCurrentTile;

    [SerializeField] Tile whiteTile;
    [SerializeField] Tile greenTile;
    [SerializeField] Tile redTile;

    [SerializeField] private LayerMask buildingLayer;
    public GameObject currentValuesUIBuilding;

    private void Awake()
    {
        mainCam = Camera.main;
        controls = new Controls();

        for (int i = 0; i < availableBuildings.Length; i++)
        {
            GameObject newBuilding = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, buildingGrid.transform);
            newBuilding.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableBuildings[i].BuildingName;
            BuildingButton buildingButton = newBuilding.GetComponent<BuildingButton>();
            buildingButton.buildingSelector = this;
            buildingButton.buildingObj = availableBuildings[i];

            Button button = newBuilding.GetComponent<Button>();
            button.onClick.AddListener(() => buildingButton.CreateBuildingButton());
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void Update()
    {
        if (creatingBuilding == true)
        {
            //Debug.Log(currentBuildingSize);
            Vector3 buildingPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            buildingPosition = new Vector3(Mathf.Round(buildingPosition.x + currentBuildingSize.x * 0.5f) - currentBuildingSize.x * 0.5f, Mathf.Round(buildingPosition.y + currentBuildingSize.y * 0.5f) - currentBuildingSize.y * 0.5f, 0);

            //buildingPosition = new Vector3(Mathf.Round(buildingPosition.x + 0.5f) - 0.5f, Mathf.Round(buildingPosition.y + 0.5f) - 0.5f, 0);
            currentBuilding.transform.position = buildingPosition;


            Vector3Int tilePosition = unUsedTileMap.WorldToCell(buildingPosition);
            if (currentTile != tilePosition)
            {
                //Debug.Log("Change");
                //currentBuildingSize.position = tilePosi;
                //TileBase[] allTiles = unUsedTileMap.GetTilesBlock(currentBuildingSize);

                ResetTiles();

                currentTile = tilePosition;
                int counter = 0;
                int canPlaceCounter = 0;

                BoundsInt bounds = new BoundsInt(new Vector3Int(currentTile.x, currentTile.y, 0), size: new Vector3Int(currentBuildingSize.x, currentBuildingSize.y, 1));
                foreach (var position in bounds.allPositionsWithin)
                {
                    Vector3Int tilePosi = new Vector3Int(position.x, position.y);
                    TileBase tile = unUsedTileMap.GetTile(tilePosi);
                    if (tile == whiteTile)
                    {
                        unUsedTileMap.SetTile(tilePosi, greenTile);
                        canPlaceCounter++;
                    }
                    currentColoredTiles.Add(tilePosi);
                    counter++;
                }
                if(canPlaceCounter == currentColoredTiles.Count)
                {
                    canPlaceCurrentTile = true;
                }

                PlayerUI.Instance.DrawRangeCicrle(buildingPosition, currentBuilding.GetComponent<Building>().buildingObj);
            }

            if (controls.Player.PlaceBuilding.WasPerformedThisFrame())
            {
                PlaceBuilding();
            }

            if (controls.Menu.CloseMenu.WasPerformedThisFrame() || controls.Player.CancelBuilding.WasPerformedThisFrame())
            {
                CloseBuildingMenu();
            }
        }
        else
        {
            if (controls.Menu.CloseMenu.WasPerformedThisFrame())
            {
                currentValuesUIBuilding = null;
                PlayerUI.Instance.CloseBuildingValues();
            }

            CheckForBuildingValues();
        }
    }
    private void PlaceBuilding()
    {
        Building building = currentBuilding.GetComponent<Building>();
        if (canPlaceCurrentTile && EventSystem.current.IsPointerOverGameObject() == false && PlayerUI.Instance.ressourceManager.CheckForMoney(building.buildingObj.Costs))
        {
            int counter = 0;
            BoundsInt bounds = new BoundsInt(new Vector3Int(currentTile.x, currentTile.y, 0), size: new Vector3Int(currentBuildingSize.x, currentBuildingSize.y, 1));
            foreach (var position in bounds.allPositionsWithin)
            {
                Vector3Int tile = new Vector3Int(position.x, position.y);
                TileBase test = unUsedTileMap.GetTile(tile);
                unUsedTileMap.SetTile(tile, redTile);
                currentColoredTiles.Add(tile);
                counter++;
            }
            //unUsedTileMap.SetTile(tilePosi, redTile);

            PlayerUI.Instance.ressourceManager.ChangeMoneyCount(-building.buildingObj.Costs);
            building.isBuild = true;
            building.OnBuild();

            if (PlayerUI.Instance.buildingValuesObj.activeSelf)
            {
                PlayerUI.Instance.ActivateBuildingValues(currentBuilding);
            }
            else
            {
                PlayerUI.Instance.DisableRangeCircle();
            }

            unUsedTileMap.gameObject.SetActive(false);
            currentBuilding = null;
            creatingBuilding = false;
        }
    }
    private void CheckForBuildingValues()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 ray = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, Mathf.Infinity, buildingLayer);
        if (hit.collider != null)
        {
            if (controls.Menu.SelectBuilding.WasPerformedThisFrame() && hit.collider.gameObject.GetComponentInParent<Building>().isBuild)
            {
                currentValuesUIBuilding = hit.collider.transform.parent.gameObject;
                PlayerUI.Instance.ActivateBuildingValues(currentValuesUIBuilding);
            }
        }
    }
    public void SellTower()
    {
        if (currentValuesUIBuilding != null)
        {
            BuildingObj obj = currentValuesUIBuilding.GetComponent<Building>().buildingObj;

            Vector3Int tilePosition = unUsedTileMap.WorldToCell(currentValuesUIBuilding.transform.position);

            currentTile = tilePosition;
            int counter = 0;

            Vector3Int size = obj.buildingSize;

            BoundsInt bounds = new BoundsInt(new Vector3Int(currentTile.x, currentTile.y, 0), size: new Vector3Int(size.x, size.y, 1));
            foreach (var position in bounds.allPositionsWithin)
            {
                Vector3Int tilePosi = new Vector3Int(position.x, position.y);
                TileBase tile = unUsedTileMap.GetTile(tilePosi);
                if (tile == redTile)
                {
                    unUsedTileMap.SetTile(tilePosi, whiteTile);
                }
                counter++;
            }

            PlayerUI.Instance.ressourceManager.ChangeMoneyCount(obj.Costs);

            Destroy(currentValuesUIBuilding);
            currentValuesUIBuilding = null;
            PlayerUI.Instance.CloseBuildingValues();
        }
    }
    public void CloseBuildingMenu()
    {
        ResetTiles();

        unUsedTileMap.gameObject.SetActive(false);
        creatingBuilding = false;
        Destroy(currentBuilding);

        PlayerUI.Instance.CloseBuildingMenu();
        PlayerUI.Instance.ResetRangeCircle();
    }
    private void ResetTiles()
    {
        for (int i = 0; i < currentColoredTiles.Count; i++)
        {
            TileBase resetTile = unUsedTileMap.GetTile(currentColoredTiles[i]);
            if (resetTile == greenTile)
            {
                unUsedTileMap.SetTile(currentColoredTiles[i], whiteTile);
            }
        }
        currentColoredTiles.Clear();
        canPlaceCurrentTile = false;
    }
    public void CreateBuildingPrefab(BuildingObj buildingObj)
    {
        if (creatingBuilding == false)
        {
            creatingBuilding = true;
            unUsedTileMap.gameObject.SetActive(true);
            currentBuilding = Instantiate(buildingObj.prefab, Vector3.zero, Quaternion.identity);
            currentBuildingSize = buildingObj.buildingSize;
        }
        else
        {
            ResetTiles();
            Destroy(currentBuilding);

            currentBuilding = Instantiate(buildingObj.prefab, Vector3.zero, Quaternion.identity);
            currentBuildingSize = buildingObj.buildingSize;
        }
    }
}
