using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportAssets : MonoBehaviour
{
    public GameObject obj;
    public InfoNetworking infoNetworking;

    void Start()
    {
        StartCoroutine(infoNetworking.DownloadOBJ(OnObjectDownloaded));
    }


    void Update()
    {
        
    }

    private void OnObjectDownloaded(GameObject downloadedObj)
    {
        if (downloadedObj == null)
        {
            Debug.LogError("Failed to load the object.");
            return;
        }

        obj = downloadedObj;
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ImportAssets : MonoBehaviour
// {
//     public GameObject obj;
//     public InfoNetworking infoNetworking;

//     void Start()
//     {
//         StartCoroutine(infoNetworking.GetAssets(OnAssetsDownloaded));
//     }

//     void Update()
//     {
        
//     }

//     private void OnAssetsDownloaded(List<InfoNetworking.Asset> assets)
//     {
//         if (assets == null || assets.Count == 0)
//         {
//             Debug.LogError("Failed to load the assets.");
//             return;
//         }

//         Debug.Log("Assets downloaded: " + assets.Count);

//         foreach (var asset in assets)
//         {
//             Debug.Log("Asset ID: " + asset.id);
//             Debug.Log("Asset ID Short: " + asset.idShort);
//             Debug.Log("Asset Global ID: " + asset.assetInformation.globalAssetId);

//             foreach (var submodel in asset.submodels)
//             {
//                 foreach (var key in submodel.keys)
//                 {
//                     Debug.Log("Submodel Key Type: " + key.type);
//                     Debug.Log("Submodel Key Value: " + key.value);

//                     if (key.type == "Submodel" && key.value.EndsWith(".obj"))
//                     {
//                         StartCoroutine(networkingScript.DownloadOBJ(key.value, OnObjectDownloaded));
//                     }
//                 }
//             }
//         }
//     }

//     private void OnObjectDownloaded(GameObject downloadedObj)
//     {
//         if (downloadedObj == null)
//         {
//             Debug.LogError("Failed to load the object.");
//             return;
//         }

//         obj = downloadedObj;
//         Debug.Log("Loaded Object: " + obj.name);
//     }
// }

