using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public GameObject tile;
    public GameObject cameraSystem;
    public RectTransform uiElement;

    public int gridX = 100;
    public int gridY = 100;
    public int tileWidth = 10;
    public int tileHeight = 10;

    private int intX = 1;
    private int intY = 1;
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;

    private TileState[,] floorGrid;

    #region Unity Methods

    public void Start()
    {
        // Initialize floor grid with empty values
        floorGrid = new TileState[gridX, gridY];
        InitializeFloorGrid();

        CreateFloor(intX, intY);
    }

    public void Update()
    {
        if (int.TryParse(inputX.text, out int newX))
        {
            intX = newX;
            xReady = true;
        }
        if (int.TryParse(inputY.text, out int newY))
        {
            intY = newY;
            yReady = true;
        }
        
        //If x and y are both entered
        if(xReady && yReady && !floorCreated)
        {
            CreateFloor(intX, intY);
            floorCreated = true;
        }
        //If x and y have changed 
        else if(xReady && yReady)
        {
            if(currentFloor.x != intX || currentFloor.y != intY)
                CreateFloor(intX, intY);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            //Enter UI position manually 
            Rect uiArea = new Rect(0f, 840f, 240f, 240f);

            if(!uiArea.Contains(mousePosition))
                HandleMouseClick(mousePosition);
        }
    }

    #endregion

    #region Create Floor

    void InitializeFloorGrid()
    {
        // Initialize the entire grid as "empty"
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                floorGrid[i, j] = TileState.Empty;
            }
        }
    }

    public void CreateFloor(int x, int y)
    {
        ClearFloor();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(0.5f + i, 0, 0.5f + j), Quaternion.identity);
                newTile.transform.SetParent(transform);

                //Giving TileController position of each tile created
                TileController tileController = newTile.GetComponent<TileController>();
                if (tileController != null)
                {
                    tileController.Initialize(this, gridX / 2 + i, gridY / 2 + j);
                }

                //Update the floor grid at the corresponding position
                floorGrid[gridX / 2 + i, gridY / 2 + j] = TileState.Floor;
            }
        }

        currentFloor = new Vector2(intX, intY);
        //PrintGrid();

        AdjustCameraSystemPosition(x, y);
    }

    public void ClearFloor()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        InitializeFloorGrid();
    }

    private void PrintGrid()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                if(floorGrid[i, j] == TileState.Floor)
                    Debug.Log($"Tile at position ({i}, {j}) is Floor");
            }
        }
    }

    public void SetTileState(int x, int y, TileState state)
    {
        floorGrid[x, y] = state;
    }

    private void HandleMouseClick(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPosition = hit.point;

            // 그리드 좌표로 변환
            int gridX = Mathf.RoundToInt(hitPosition.x / tileWidth);
            int gridZ = Mathf.RoundToInt(hitPosition.z / tileHeight);

            // 중심 좌표 보정
            gridX += gridX / 2;
            gridZ += gridY / 2;

            if (floorGrid[gridX, gridZ] == TileState.Empty)
            {
                Vector3 tilePosition = new Vector3(gridX * tileWidth, 0, gridZ * tileHeight);
                GameObject newTile = Instantiate(tile, tilePosition, Quaternion.identity);
                newTile.transform.SetParent(transform);

                TileController tileController = newTile.GetComponent<TileController>();
                if (tileController != null)
                {
                    tileController.Initialize(this, gridX, gridZ);
                }

                floorGrid[gridX, gridZ] = TileState.Floor;
            }
        }
    }

    #endregion

    //Adjust camera position according to the size of floor
    private void AdjustCameraSystemPosition(int x, int y)
    {
        float totalWidth = x * tileWidth;
        float totalHeight = y * tileHeight;
        Vector3 centerPosition;

        //Calculate center of the floor
        if (totalWidth == 1f)
            centerPosition = new Vector3(0.5f, 0, 0);
        else
            centerPosition = new Vector3(totalWidth / 2f, 0, 0);

        //Calculate distance of camera
        float distance = Mathf.Max(totalWidth, totalHeight) * 0.3f; 
        cameraSystem.transform.position = new Vector3(centerPosition.x, distance, centerPosition.z - distance);
    }



}
