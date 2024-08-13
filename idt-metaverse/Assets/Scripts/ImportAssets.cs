using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ImportAssets : MonoBehaviour
{
    public InfoNetworking networkingScript;
    public DBAccess dbAccess;

    public GameObject AssetButton;
    private Vector3 startPosition = new Vector3(960, 700, 0);
    private string selectedModel = null;
    private string selectedName = null;

    private string modelsDirectory = "Assets/Resources/Models/";

    void Start()
    {
        if (!Directory.Exists(modelsDirectory))
        {
            Directory.CreateDirectory(modelsDirectory);
            StartCoroutine(networkingScript.GetAssetList(OnAssetsDownloadedAndSaveToLocal));
        }
        else
        {
            LoadAssetsFromLocal();
        }
    }

    public void LoadBaseScene()
    {
        SceneManager.LoadScene("BaseScene");
    }

    //Get obj files and save to local folder
    private void OnAssetsDownloadedAndSaveToLocal(List<InfoNetworking.Asset> assets)
    {
        if (assets == null || assets.Count == 0)
        {
            Debug.LogError("Failed to load the assets.");
            return;
        }

        for (int i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];

            string modelUrl = networkingScript.ConstructDownloadUrl(asset.submodels[2].keys[0].value);

            //StartCoroutine(DownloadAndExportModel(asset.idShort, modelUrl));

            string filePath = Path.Combine(modelsDirectory, asset.idShort + ".obj");
            StartCoroutine(networkingScript.DownloadOBJ(modelUrl, filePath));

            CreateAssetButton(filePath, asset.idShort, i);
        }
    }

    // private IEnumerator DownloadAndExportModel(string idShort, string modelUrl)
    // {
    //     //Download and load OBJ model
    //     GameObject loadedObj = null;
    //     yield return StartCoroutine(networkingScript.DownloadOBJ(modelUrl, (GameObject obj) =>
    //     {
    //         loadedObj = obj;
    //     }));

    //     if (loadedObj != null)
    //     {
    //         //Save the OBJ file locally
    //         string filePath = Path.Combine(modelsDirectory, idShort + ".obj");
    //         MyOBJExporter.Export(loadedObj, filePath);

    //         Debug.Log("Model saved to: " + filePath);
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to download model: " + modelUrl);
    //     }
    // }

    //Load obj files from local folder
    private void LoadAssetsFromLocal()
    {
        string[] files = Directory.GetFiles(modelsDirectory, "*.obj");
        for (int i = 0; i < files.Length; i++)
        {
            CreateAssetButton(files[i], Path.GetFileNameWithoutExtension(files[i]), i);
        }
    }

    private void CreateAssetButton(string filePath, string idShort, int index)
    {
        GameObject newButton = Instantiate(AssetButton);
        Button button = newButton.GetComponentInChildren<Button>();
        RectTransform rectTransform = button.GetComponentInChildren<RectTransform>();
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        rectTransform.position = startPosition + new Vector3(0, -80 * index, 0);

        buttonText.text = idShort;

        button.onClick.AddListener(() => OnAssetButtonClicked(filePath, idShort));
    }

    private void OnAssetButtonClicked(string filePath, string modelName)
    {
        selectedModel = filePath;
        selectedName = modelName;
    }

    public void ImportButton()
    {
        if (selectedModel == null || selectedName == null)
        {
            Debug.LogError("No asset selected.");
            return;
        }

        int spaceId = PlayerPrefs.GetInt("SpaceID");

        // 임시 x, z, scale
        dbAccess.AddAssetData(spaceId, selectedName, 0f, 0f, 0.01f, selectedModel, null);
        Debug.Log("Asset data imported to DB: " + selectedName);

        LoadBaseScene();
    }

}
