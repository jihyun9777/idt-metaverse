using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Floor Variables

    //Floor size input X and Y
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public GameObject tile;
    public GameObject previewTile;

    public Vector3 origin;
    public int gridX;
    public int gridY;
    public int tileWidth;
    public int tileHeight;
    public bool createMode;
    public bool canCreateTile;
    public float FixedTileYPosition = -0.505f;

    //Integer form of input X and Y
    private int intX = 1;
    private int intY = 1;
    //To check if both X and Y are entered
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;
    private GameObject currentPreviewTile;
    private Vector3 previewTilePosition;

    public TileState[,] floorGrid;
    #endregion
    #region Piece Variables

    //Initialize outside
    public TMP_InputField pieceSizeX;
    public TMP_InputField pieceSizeY;
    public TMP_InputField FilePath;
    public GameObject panel;

    public PieceController pieceController;
    public GameObject piece;
    public GameObject obj;

    private int pieceintX = 1;
    private int pieceintY = 1;
    private bool pieceXReady = false;
    private bool pieceYReady = false;
    private bool objectReady = false;
    private int pieceCount = 0;

    private Vector3 startPosition;

    #endregion

    #region Unity Methods

    public void Start()
    {
        #region Floor Start

        floorGrid = new TileState[gridX, gridY];

        currentPreviewTile = Instantiate(previewTile);
        currentPreviewTile.SetActive(false);

        CreateFloor(intX, intY);

        #endregion

        #region Object Start

        startPosition = new Vector3(0f, 0f, 0f);

        #endregion
    }

    public void Update()
    {
        #region Floor Update

        if (int.TryParse(inputX.text, out intX))
            xReady = true;
        if (int.TryParse(inputY.text, out intY))
            yReady = true;
        
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

        if(createMode)
        {
            //Avoid UI click or hover
            Vector3 mousePosition = Input.mousePosition;
            //Enter UI position manually (left x, top/bottom y, width, height)
            Rect uiArea = new Rect(0f, 650f, 300f, 500f);

            if(!uiArea.Contains(mousePosition))
            {
                if(Input.GetMouseButtonDown(0))
                    HandleMouseClick(mousePosition);
                else
                    UpdatePreviewTile(Input.mousePosition);
            }
        }
        else
            currentPreviewTile.SetActive(false);

        #endregion
        //CheckAllFloor();
        //PrintGrid();

        #region Object Update

        

        #endregion
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
                Vector3 tilePosition = new Vector3((gridX / 2 + i - gridX / 2 + origin.x), FixedTileYPosition, (gridY / 2 + j - gridY / 2 + origin.z));
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
                // if(floorGrid[i, j] == TileState.Floor)
                //     Debug.Log($"Tile at position ({i}, {j}) is Floor");

                if(floorGrid[i, j] == TileState.Occupied)
                    Debug.Log($"Tile at position ({i}, {j}) is Occupied");
            }
        }
    }

    private void CheckAllFloor()
    {
        bool allFloor = true;

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                if(floorGrid[i, j] == TileState.Occupied)
                {
                    allFloor = false;
                    break;
                }
            }
        }

        if(allFloor)
            Debug.Log("Floor is clear");
    }

    //Set a tile state <empty, occupied, floor>
    public void SetTileState(int x, int y, TileState state)
    {
        floorGrid[x, y] = state;
    }

    //Create a tile when clicked
    private void HandleMouseClick(Vector3 mousePosition)
    {
        //Prevent tile creation if a tile was just destroyed
        if (!canCreateTile) return;

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
                Vector3 tilePosition = new Vector3(pointX - gridX / 2 + origin.x, FixedTileYPosition, pointZ - gridY / 2 + origin.z);

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
                previewTilePosition = new Vector3(pointX - gridX / 2 + origin.x, FixedTileYPosition, pointZ - gridY / 2 + origin.z);
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

    public void OnTileDestroyed(int x, int y)
    {
        SetTileState(x, y, TileState.Empty);
        StartCoroutine(PreventTileCreationTemporarily());
    }

    private IEnumerator PreventTileCreationTemporarily()
    {
        canCreateTile = false;
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        canCreateTile = true;
    }

    #endregion

    #region Reposition Camera

    public GameObject cameraSystem;

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

    #endregion

    #region Create Object

    public void CreatePiece(Transform parent = null)
    {
        piece = new GameObject("Piece" + pieceCount);
        // piece.AddComponent<BoxCollider>();
        // piece.AddComponent<Rigidbody>().isKinematic = true;

        for (int i = 0; i < pieceintX; i++)
        {
            for (int j = 0; j < pieceintY; j++)
            {
                GameObject newPanel = Instantiate(panel, piece.transform);
                Vector3 panelPosition = startPosition + new Vector3(i, 0, j);
                newPanel.transform.position = panelPosition;
            }
        }

        piece.AddComponent<PieceController>();
        pieceCount ++;

        if (parent != null)
        {
            piece.transform.SetParent(parent, true); 
        }
    }

    public void CreateObject()
    {
        //Find Piece center, and place object

        CreatePiece(obj.transform);
    }

    public void LoadPrefab()
    {
        
        
    }

    public void CreateObjectButton()
    {
        if (int.TryParse(pieceSizeX.text, out int x))
            pieceXReady = true;
        if (int.TryParse(pieceSizeY.text, out int y))
            pieceYReady = true;
        //Check imported game object
        if (obj != null)
            objectReady = true;
        if (pieceXReady && pieceYReady && objectReady)
        {
            CreateObject();
        }
    }

    #endregion

}
