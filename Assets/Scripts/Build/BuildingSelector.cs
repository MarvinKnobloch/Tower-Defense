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
    public GameObject currentMouseOverObj;

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
            }

            //Vector3Int tilePosi = unUsedTileMap.WorldToCell(buildingPosition);
            //TileBase tileOnMousePosition = unUsedTileMap.GetTile(tilePosi);
            //if(currentTile != unUsedTileMap.WorldToCell(buildingPosition))
            //{
            //    if (unUsedTileMap.GetTile(currentTile) == greenTile)
            //    {
            //        if (currentTile != null) unUsedTileMap.SetTile(currentTile, whiteTile);
            //    }
            //    currentTile = tilePosi;
            //    if (tileOnMousePosition == whiteTile)
            //    {
            //        unUsedTileMap.SetTile(tilePosi, greenTile);
            //    }

            //}

            if (controls.Player.PlaceBuilding.WasPerformedThisFrame())
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

                    unUsedTileMap.gameObject.SetActive(false);
                    currentBuilding = null;
                    creatingBuilding = false;
                }
            }

            if (controls.Menu.CloseMenu.WasPerformedThisFrame() || controls.Player.CancelBuilding.WasPerformedThisFrame())
            {
                //if (unUsedTileMap.GetTile(currentTile) == greenTile)
                //{
                //    //if (currentTile != null) unUsedTileMap.SetTile(currentTile, whiteTile);
                //}
                CloseBuildingMenu();
            }
        }
        else
        {
            if (controls.Menu.CloseMenu.WasPerformedThisFrame())
            {
                currentMouseOverObj = null;
                PlayerUI.Instance.CloseBuildingValues();
            }

            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 ray = mainCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, ray, Mathf.Infinity, buildingLayer);
            if (hit.collider != null)
            {
                currentMouseOverObj = hit.collider.gameObject;
                if (controls.Menu.SelectBuilding.WasPerformedThisFrame() && currentMouseOverObj.GetComponent<Building>().isBuild)
                {
                    PlayerUI.Instance.ActivateBuildingValues(currentMouseOverObj);
                }
            }
            //else
            //{
            //    currentMouseOverObj = null;
            //}
        }
    }
    public void SellTower()
    {
        if (currentMouseOverObj != null)
        {
            BuildingObj obj = currentMouseOverObj.GetComponent<Building>().buildingObj;

            Vector3Int tilePosition = unUsedTileMap.WorldToCell(currentMouseOverObj.transform.position);

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

            Destroy(currentMouseOverObj);
            currentMouseOverObj = null;
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
