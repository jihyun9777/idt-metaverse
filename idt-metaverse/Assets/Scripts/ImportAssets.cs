using System.Collections;
using System.Collections.Generic;
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
    private InfoNetworking.Asset selectedAsset; //To keep track of the selected asset

    void Start()
    {
        StartCoroutine(networkingScript.GetAssetList(OnAssetsDownloaded));
    }

    void Update()
    {
        
    }

    public void LoadBaseScene()
    {
        SceneManager.LoadScene("BaseScene");
    }

    private void OnAssetsDownloaded(List<InfoNetworking.Asset> assets)
    {
        if (assets == null || assets.Count == 0)
        {
            Debug.LogError("Failed to load the assets.");
            return;
        }

        Debug.Log("Assets downloaded: " + assets.Count);

        for (int i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];
            Debug.Log("Asset ID: " + asset.id);
            Debug.Log("Asset ID Short: " + asset.idShort);
            Debug.Log("Asset Global ID: " + asset.assetInformation.globalAssetId);

            GameObject newButton = Instantiate(AssetButton);
            Button button = newButton.GetComponentInChildren<Button>();
            RectTransform rectTransform = button.GetComponentInChildren<RectTransform>();
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            rectTransform.position = startPosition + new Vector3(0, -80 * i, 0);

            buttonText.text = asset.idShort;

            button.onClick.AddListener(() => OnAssetButtonClicked(asset));
        }
    }

    private void OnAssetButtonClicked(InfoNetworking.Asset asset)
    {
        selectedAsset = asset;
        Debug.Log("Selected Asset: " + asset.idShort);
    }

    public void ImportButton()
    {
        if (selectedAsset == null)
        {
            Debug.LogError("No asset selected.");
            return;
        }

        int spaceId = PlayerPrefs.GetInt("SpaceID");
        string name = selectedAsset.idShort;
        string model = networkingScript.ConstructDownloadUrl(selectedAsset.submodels[2].keys[0].value);

        //임시 x, z, scale
        dbAccess.AddAssetData(spaceId, name, 0f, 0f, 0.01f, model, null);
        Debug.Log("Asset data imported to DB: " + name);

        LoadBaseScene();
    }
}
