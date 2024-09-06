using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dummiesman;

public class BaseSceneController : MonoBehaviour
{
    #region Tab Variables

    public bool playMode = false;
    public bool pauseMode = false;

    private bool tapOpen = true;

    //Open Tab
    public GameObject openPanel;
    private Button closeButton;
    public TMP_Text nameText;
    private TMP_Text dimText;
    private TMP_Text timesText;
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    //public Button createModeButton;

    //Close Tab
    public GameObject closePanel;
    private Button openButton;

    #endregion
    #region Button Variables

    //Asset Button
    private bool assetTabOpen = false;
    public GameObject assetTab;

    //Basic Object Button
    private bool basicObjectTabOpen = false;
    public GameObject basicObjectTab;

    //Storage Button
    private bool storageTabOpen = false;
    public GameObject storageTab;

    //Storage Button
    private bool conveyorTabOpen = false;
    public GameObject conveyorTab;

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
    #region Asset Variables

    public DBAccess dBAccess;
    //public InfoNetworking infoNetworking;
    private int spaceId;
    private string spaceName;

    public GameObject assetIcon;
    public GameObject obj;
    private Vector3 startPosition = new Vector3(210, 450, 0);
    private List<GameObject> assetIcons = new List<GameObject>();

    private float? positionX = null;
    private float? positionZ = null;

    #endregion

    #region Unity Methods

    void Start()
    {
        #region Tab Start

        //Receive values from other Scene
        spaceId = PlayerPrefs.GetInt("SpaceID");
        intX = PlayerPrefs.GetInt("SpaceX"); 
        intY = PlayerPrefs.GetInt("SpaceY"); 
        inputX.text = intX.ToString();
        inputY.text = intY.ToString();

        nameText.text = PlayerPrefs.GetString("SpaceName");
        spaceName = PlayerPrefs.GetString("SpaceName");

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
    
        #region Asset Start

        CreateAssetList();

        #endregion
    }

    void Update()
    {
        #region Button Update



        #endregion

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
                dBAccess.SetSpaceData(spaceName, intX, intY);
                PlayerPrefs.SetInt("SpaceX", intX);
                PlayerPrefs.SetInt("SpaceY", intY);
                floorCreated = true;
            }
        }

        // //Avoid UI click or hover
        // if(createMode)
        // {
        //     Vector3 mousePosition = Input.mousePosition;
        //     Rect uiArea;
            
        //     if (tapOpen)
        //         uiArea = GetPanelRect(openPanel);
        //     else
        //         uiArea = GetPanelRect(closePanel);

        //     if(!uiArea.Contains(mousePosition))
        //         UpdateTilePlacement(mousePosition);
        // }
        // else
        //     currentPreviewTile.SetActive(false);

        #endregion
    
        #region Asset Update

        if (obj != null)
        {
            Transform child = obj.transform.GetChild(0);
            Vector3 childPosition = child.position;

            //Check if positionX and positionZ are set and compare them
            if (positionX.HasValue && positionZ.HasValue)
            {
                //Check if the Asset position differs from previous position
                if (Mathf.Abs(childPosition.x - positionX.Value) > Mathf.Epsilon ||
                    Mathf.Abs(childPosition.z - positionZ.Value) > Mathf.Epsilon)
                {
                    //나중에 Asset obj 파일 넣었을때 다시 시도
                    //dBAccess.UpdateAssetData(spaceId, spaceName, Mathf.RoundToInt(childPosition.x), Mathf.RoundToInt(childPosition.z));

                    positionX = childPosition.x;
                    positionZ = childPosition.z;
                }
            }
            else
            {
                positionX = childPosition.x;
                positionZ = childPosition.z;
            }
        }

        #endregion
    }

    #endregion

    #region Tab Controller

    public void PlayOn()
    {
        playMode = true;

        if (pauseMode)
            pauseMode = false;
    }
    
    public void PlayOff()
    {
        playMode = false;
    }

    public void PauseOn()
    {
        if (playMode)
            pauseMode = true;
    }

    public void ToggleTab()
    {
        tapOpen = !tapOpen; 

        openPanel.gameObject.SetActive(tapOpen);
        SetAssetIconsActive(tapOpen);

        closePanel.gameObject.SetActive(!tapOpen);

        //Close all button tab
        if (!tapOpen)
            DeactivateAllButtonTab();
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

    //Close all button tab
    public void DeactivateAllButtonTab()
    {
        assetTabOpen = false;
        assetTab.SetActive(false);

        basicObjectTabOpen = false;
        basicObjectTab.SetActive(false);

        storageTabOpen = false;
        storageTab.SetActive(false);

        conveyorTabOpen = false;
        conveyorTab.SetActive(false);
    }

    #endregion

    #region Button Controller

    //When AssetButton is Clicked
    public void ToggleAssetTab()
    {
        //Close all tap before openning this tab
        if (!assetTabOpen)
            DeactivateAllButtonTab();

        assetTabOpen = !assetTabOpen;
        assetTab.SetActive(assetTabOpen);
    }

    //When BasicObjectButton is Clicked
    public void ToggleBasicObjectTab()
    {
        if (!basicObjectTabOpen)
            DeactivateAllButtonTab();

        basicObjectTabOpen = !basicObjectTabOpen;
        basicObjectTab.SetActive(basicObjectTabOpen);
    }

    //Instantiate Basic Object
    public void InstantiateBasicObject(string name)
    {
        GameObject basicObject = Resources.Load<GameObject>(name);
        if (basicObject != null)
        {
            Instantiate(basicObject, Vector3.zero, Quaternion.identity);
        }
        else    Debug.LogError("Basic Object not found: " + name);
    }

    //When StorageButton is Clicked
    public void ToggleStorageTab()
    {
        if (!storageTabOpen)
            DeactivateAllButtonTab();

        storageTabOpen = !storageTabOpen;
        storageTab.SetActive(storageTabOpen);
    }

    //Instantiate Storage Object with BoxCollider and CubeController script
    public void InstantiateStorage(string name)
    {
        GameObject temp = Resources.Load<GameObject>(name);
        if (temp != null)
        {
            GameObject storageObject = Instantiate(temp, Vector3.zero, Quaternion.identity);

            //Calculate bounds of every child objects
            Bounds combinedBounds = new Bounds(storageObject.transform.position, Vector3.zero);
            foreach (Renderer renderer in storageObject.GetComponentsInChildren<Renderer>())
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            //Create BoxCollider
            BoxCollider boxCollider = storageObject.AddComponent<BoxCollider>();
            boxCollider.center = combinedBounds.center - storageObject.transform.position;
            boxCollider.size = combinedBounds.size;

            storageObject.AddComponent<CubeController>();
        }
        else    Debug.LogError("Storage Object not found: " + name);
    }

    //When ConveyorButton is Clicked
    public void ToggleConveyorTab()
    {
        if (!conveyorTabOpen)
            DeactivateAllButtonTab();

        conveyorTabOpen = !conveyorTabOpen;
        conveyorTab.SetActive(conveyorTabOpen);
    }

    public void InstantiateConveyor(string name)
    {
        GameObject temp = Resources.Load<GameObject>(name);
        if (temp != null)
        {
            GameObject conveyorObject = Instantiate(temp, Vector3.zero, Quaternion.identity);

            //Calculate bounds of every child objects
            Bounds combinedBounds = new Bounds(conveyorObject.transform.position, Vector3.zero);
            foreach (Renderer renderer in conveyorObject.GetComponentsInChildren<Renderer>())
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            //Create BoxCollider
            BoxCollider boxCollider = conveyorObject.AddComponent<BoxCollider>();
            boxCollider.center = combinedBounds.center - conveyorObject.transform.position;
            boxCollider.size = combinedBounds.size;

            conveyorObject.AddComponent<ConveyorController>();
        }
        else
        {
            Debug.LogError("Conveyor Object not found: " + name);
        }
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

    // public void ChangeCreateMode()
    // {
    //     createMode = !createMode;

    //     if (createMode)
    //         ChangeButtonColors(OnColor);
    //     else
    //         ChangeButtonColors(OffColor);
    // }

    // private void ChangeButtonColors(Color color)
    // {
    //     ColorBlock cb = createModeButton.colors;
    //     cb.normalColor = color;
    //     cb.highlightedColor = color;
    //     cb.pressedColor = color;
    //     cb.selectedColor = color;
    //     cb.disabledColor = color;
    //     createModeButton.colors = cb;
    // }

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

    public void CreateAssetList()
    {
        List<AssetData> assets = dBAccess.SearchAllAssets(spaceId);

        for (int i = 0; i < assets.Count; i++)
        {
            AssetData asset = assets[i];

            //Instantiate assetIcon
            GameObject icon = Instantiate(assetIcon);
            Button button = icon.GetComponentInChildren<Button>();
            RectTransform rectTransform = button.GetComponentInChildren<RectTransform>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            rectTransform.position = startPosition + new Vector3(0, -80 * i, 0);
            buttonText.text = asset.Name;

            assetIcons.Add(icon);

            //If X and Z are not null, download and instantiate the model
            if (asset.X.HasValue && asset.Z.HasValue)
            {
                StartCoroutine(LoadAndInstantiateModel(asset.ID, asset.Name, asset.Model, new Vector3(asset.X.Value, 0f, asset.Z.Value), asset.Scale));
            }
        }
    }

    private IEnumerator LoadAndInstantiateModel(int assetId, string assetName, string modelPath, Vector3 position, float? scale)
    {
        GameObject loadedObj = null;

        //Remove file extension and adjust the path to be relative to the Resources folder
        string relativePath = Path.Combine("Models", Path.GetFileNameWithoutExtension(modelPath));
        string fullPath = Path.Combine(Application.dataPath, "Resources", relativePath + ".obj");

        //Load the OBJ file using OBJLoader
        loadedObj = new OBJLoader().Load(fullPath);

        if (loadedObj == null)
        {
            Debug.LogError("Model could not be loaded from path: " + fullPath);
            yield break;
        }

        loadedObj = Instantiate(loadedObj);

        // Wait until the object is fully loaded
        while (loadedObj != null && loadedObj.transform.childCount == 0)
        {
            yield return null;
        }

        if (loadedObj != null && loadedObj.transform.childCount > 0)
        {
            Transform child = loadedObj.transform.GetChild(0);
            child.position = position;

            if (scale.HasValue)
            {
                child.localScale = Vector3.one * scale.Value;
            }

            // Add components to the object if not already present
            if (child.gameObject.GetComponent<Rigidbody>() == null)
            {
                child.gameObject.AddComponent<Rigidbody>().isKinematic = true;
                child.gameObject.AddComponent<BoxCollider>();
                child.gameObject.AddComponent<AssetController>();

                AssetController assetController = child.GetComponent<AssetController>();
                assetController.OnPositionChanged += (updatedAsset, newPosition) =>
                {
                    HandleAssetPositionChanged(assetId, newPosition);
                };
            }
        }
    }

    private void SetAssetIconsActive(bool isActive)
    {
        foreach (GameObject icon in assetIcons)
        {
            icon.SetActive(isActive);
        }
    }

    private void HandleAssetPositionChanged(int assetId, Vector3 newPosition)
    {
        //Update the asset position in the database
        dBAccess.SetAssetLocation(assetId, newPosition.x, newPosition.z);
    }

    #endregion

    #region ScreenShot

    public void LoadSpaceMenuSceneWithScreenShot()
    {
        StartCoroutine(ScreenshotAndSaveToDB());
    }

    private IEnumerator ScreenshotAndSaveToDB()
    {
        //Close Tab before ScreenShot
        if (tapOpen)
        {
            ToggleTab();
        }

        //Wait for end of frame to capture screenshot
        yield return new WaitForEndOfFrame();

        //Create a texture to read the screen contents
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        //Encode texture to PNG
        byte[] screenshotBytes = screenTexture.EncodeToPNG();
        Destroy(screenTexture);

        dBAccess.SetSpacePreview(spaceName, screenshotBytes);

        if (!tapOpen)
        {
            ToggleTab();
        }

        SceneManager.LoadScene("SpaceMenuScene");
    }

    #endregion

}
