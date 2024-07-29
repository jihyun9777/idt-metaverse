using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseSceneController : MonoBehaviour
{
    #region Tab Variables

    bool tapOpen = true;

    //Open Tab
    public Button closeButton;
    public TMP_Text nameText;
    public TMP_Text DimText;
    public TMP_Text timesText;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public Button CreateModeButton;

    public TMP_Text assetsText;
    public GameObject openPanel;

    //Close Tab
    public Button openButton;
    public GameObject closePanel;

    #endregion
    #region Tile Variables

    Renderer tileRenderer;
    float tileWidth;
    float tileHeight;

    #endregion
    #region Floor Variables

    public GameObject floor;
    public GameObject tile;
    public GameObject previewTile;
    
    private int intX;
    private int intY;
    //To check if both X and Y are entered or changed
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;

    //CreatMode Variables
    public bool createMode = false;
    public bool canCreateTile = true;
    public Color OnColor = Color.green;
    public Color OffColor;

    private GameObject currentPreviewTile;
    private Vector3 previewTilePosition;

    #endregion

    #region Unity Methods

    void Start()
    {
        #region Tab Start

        openButton.gameObject.SetActive(false);
        closePanel.SetActive(false);

        //Receive values from Create Space Scene
        intX = PlayerPrefs.GetInt("SpaceX"); 
        intY = PlayerPrefs.GetInt("SpaceY"); 
        inputX.text = intX.ToString();
        inputY.text = intY.ToString();
        nameText.text = PlayerPrefs.GetString("SpaceName");

        #endregion

        #region Tile Start

        tileRenderer = tile.GetComponent<Renderer>();
        tileWidth = tileRenderer.bounds.size.x;
        tileHeight = tileRenderer.bounds.size.z;

        #endregion

        #region Floor Start

        currentPreviewTile = Instantiate(previewTile);
        currentPreviewTile.SetActive(false);

        CreateFloor(intX, intY);

        //CreateMode Button original color
        ColorUtility.TryParseHtmlString("#B0E8F9", out OffColor);

        #endregion
    }

    void Update()
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
            {
                CreateFloor(intX, intY);
                floorCreated = true;
            }
        }

        //Avoid UI click or hover
        if(createMode)
        {
            Vector3 mousePosition = Input.mousePosition;
            Rect uiArea;
            
            if (tapOpen)
                uiArea = GetPanelRect(openPanel);
            else
                uiArea = GetPanelRect(closePanel);

            if(!uiArea.Contains(mousePosition))
                UpdateTilePlacement(mousePosition);
        }
        else
            currentPreviewTile.SetActive(false);

        #endregion
    }

    #endregion

    #region Tab Controller

    public void ToggleTab()
    {
        tapOpen = !tapOpen; 

        closeButton.gameObject.SetActive(tapOpen);
        nameText.gameObject.SetActive(tapOpen);
        DimText.gameObject.SetActive(tapOpen);
        timesText.gameObject.SetActive(tapOpen);
        inputX.gameObject.SetActive(tapOpen);
        inputY.gameObject.SetActive(tapOpen);
        CreateModeButton.gameObject.SetActive(tapOpen);

        assetsText.gameObject.SetActive(tapOpen);
        openPanel.gameObject.SetActive(tapOpen);

        openButton.gameObject.SetActive(!tapOpen);
        closePanel.gameObject.SetActive(!tapOpen);
    }

    private Rect GetPanelRect(GameObject panel)
    {
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        Vector3 bottomLeft = worldCorners[0];
        Vector3 topRight = worldCorners[2];
        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    public void LoadSpaceMenuScene()
    {
        SceneManager.LoadScene("SpaceMenuScene");
    }

    #endregion

    #region Create Floor

    //Create floor according to user input
    public void CreateFloor(int x, int y)
    {
        ClearFloor();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Vector3 tilePosition = new Vector3(i * tileWidth, 0, j * tileHeight);
                GameObject newTile = Instantiate(tile, tilePosition, Quaternion.identity);
                newTile.transform.SetParent(floor.transform);
            }
        }

        currentFloor = new Vector2(x, y);
        
        AdjustCameraSystemPosition(x, y);
    }

    //Clear any created floor
    public void ClearFloor()
    {
        foreach (Transform child in floor.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ChangeCreateMode()
    {
        createMode = !createMode;

        if (createMode)
            ChangeButtonColors(OnColor);
        else
            ChangeButtonColors(OffColor);
    }

    private void ChangeButtonColors(Color color)
    {
        ColorBlock cb = CreateModeButton.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        cb.selectedColor = color;
        cb.disabledColor = color;
        CreateModeButton.colors = cb;
    }

    private void UpdateTilePlacement(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 

        //When hovered on the ground, create preview tile
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPosition = ray.GetPoint(enter);
            hitPosition = new Vector3(Mathf.Round(hitPosition.x), 0, Mathf.Round(hitPosition.z));

            currentPreviewTile.transform.position = hitPosition;
            currentPreviewTile.SetActive(true);

            //When clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (canCreateTile)
                {
                    Collider[] colliders = Physics.OverlapBox(hitPosition, Vector3.one * 0.1f);
                    bool tileExists = false;
                    foreach (Collider collider in colliders)
                    {
                        //If there is a tile, destroy
                        if (collider.gameObject.CompareTag("Tile"))
                        {
                            Destroy(collider.gameObject);
                            StartCoroutine(PreventTileCreationTemporarily());
                            tileExists = true;
                            break;
                        }
                    }

                    //If there is no tile, create one
                    if (!tileExists)
                    {
                        GameObject newTile = Instantiate(tile, hitPosition, Quaternion.identity);
                        newTile.transform.SetParent(floor.transform);
                    }
                }
            }
        }
        else
        {
            //If mouse is not hovering on ground, disable PreviewTile
            currentPreviewTile.SetActive(false);
        }
    }

    private IEnumerator PreventTileCreationTemporarily()
    {
        canCreateTile = false;
        yield return new WaitForSeconds(0.5f);
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

    // private void AdjustCameraSystemPosition()
    // {
    //     if (obj == null) return;

    //     // Get the bounds of the loaded object
    //     Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
    //     if (renderers.Length == 0) return;

    //     Bounds bounds = renderers[0].bounds;
    //     foreach (Renderer renderer in renderers)
    //         bounds.Encapsulate(renderer.bounds);

    //     float objectWidth = bounds.size.x;
    //     float objectHeight = bounds.size.z;
    //     Vector3 centerPosition = bounds.center;

    //     // Calculate distance of camera
    //     float distance = Mathf.Max(objectWidth, objectHeight) * 1f; 
    //     cameraSystem.transform.position = new Vector3(centerPosition.x, distance, centerPosition.z - distance);
    // }

    #endregion

    #region Create Asset

    public void LoadCreateAssetScene()
    {
        SceneManager.LoadScene("CreateAssetScene");
    }

    #endregion
}
