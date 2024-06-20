using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public GameObject tile;
    public GameObject previewTile;
    public GameObject cameraSystem;
    public RectTransform uiElement;

    public Vector3 origin = new Vector3(0.5f, 0, 0.5f);
    public int gridX = 100;
    public int gridY = 100;
    public int tileWidth = 10;
    public int tileHeight = 10;
    public bool createMode = false;

    private int intX = 1;
    private int intY = 1;
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;
    private GameObject currentPreviewTile;
    private Vector3 previewTilePosition;

    private TileState[,] floorGrid;

    #region Unity Methods

    public void Start()
    {
        floorGrid = new TileState[gridX, gridY];

        currentPreviewTile = Instantiate(previewTile);
        currentPreviewTile.SetActive(false);

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

        //Avoid UI click or hover
        Vector3 mousePosition = Input.mousePosition;
        //Enter UI position manually 
        Rect uiArea = new Rect(0f, 840f, 240f, 240f);

        if (Input.GetMouseButtonDown(0))
        {
            if(!uiArea.Contains(mousePosition))
                HandleMouseClick(mousePosition);
        }
        else if(createMode)
        {
            if(!uiArea.Contains(mousePosition))
                UpdatePreviewTile(Input.mousePosition);
        }
    }

    #endregion

    #region Create Floor

    //Initialize FloorGrid 2D array to zero 
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

    //Create floor according to user input
    public void CreateFloor(int x, int y)
    {
        ClearFloor();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Vector3 tilePosition = new Vector3((gridX / 2 + i - gridX / 2 + origin.x), 0, (gridY / 2 + j - gridY / 2 + origin.z));
                GameObject newTile = Instantiate(tile, tilePosition, Quaternion.identity);
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

        //버튼만들기 전까지 임시
        createMode = true;

        AdjustCameraSystemPosition(x, y);
    }

    //Clear any created floor
    public void ClearFloor()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        InitializeFloorGrid();
    }

    //Print floor grid 2D array
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

    //Set a tile state <empty, occupied, floor>
    public void SetTileState(int x, int y, TileState state)
    {
        floorGrid[x, y] = state;
    }

    //Create a tile when clicked
    private void HandleMouseClick(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPosition = hit.point;

            //Set position of tile (X, Z)
            int pointX = Mathf.FloorToInt(hitPosition.x / tileWidth) + (this.gridX / 2);
            int pointZ = Mathf.FloorToInt(hitPosition.z / tileHeight) + (this.gridY / 2);

            //If there is no tile
            if (pointX < gridX && pointZ < gridY && floorGrid[pointX, pointZ] == TileState.Empty)
            {
                //Had to set it this way, since a tile on (50, 50) in floorGrid is actually on (0,0)
                Vector3 tilePosition = new Vector3(pointX - gridX / 2 + origin.x, 0, pointZ - gridY / 2 + origin.z);

                GameObject newTile = Instantiate(tile, tilePosition, Quaternion.identity);
                newTile.transform.SetParent(transform);

                TileController tileController = newTile.GetComponent<TileController>();
                if (tileController != null)
                {
                    tileController.Initialize(this, pointX, pointZ);
                }

                floorGrid[pointX, pointZ] = TileState.Floor;
            }
        }
    }

    //Create a preview tile when hovered
    private void UpdatePreviewTile(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPosition = hit.point;

            int pointX = Mathf.FloorToInt(hitPosition.x / tileWidth) + (this.gridX / 2);
            int pointZ = Mathf.FloorToInt(hitPosition.z / tileHeight) + (this.gridY / 2);

            if (pointX < gridX && pointZ < gridY && floorGrid[pointX, pointZ] == TileState.Empty)
            {
                previewTilePosition = new Vector3(pointX - gridX / 2 + origin.x, 0, pointZ - gridY / 2 + origin.z);
                currentPreviewTile.transform.position = previewTilePosition;
                currentPreviewTile.SetActive(true);
            }
            else
            {
                currentPreviewTile.SetActive(false);
            }
        }
        else
        {
            currentPreviewTile.SetActive(false);
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
