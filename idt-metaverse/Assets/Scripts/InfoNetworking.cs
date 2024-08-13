using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;

public class InfoNetworking : MonoBehaviour
{
    public string url = "http://172.26.176.1:8081/shells";
    public string baseUrl = "http://172.26.176.1:8081/submodels/";

    [System.Serializable]
    public class Submodel
    {
        public List<Key> keys;
        public string type;
    }

    [System.Serializable]
    public class Key
    {
        public string type;
        public string value;
    }

    [System.Serializable]
    public class AssetInformation
    {
        public string assetKind;
        public string globalAssetId;
    }

    [System.Serializable]
    public class Asset
    {
        public string modelType;
        public AssetInformation assetInformation;
        public List<Submodel> submodels;
        public string id;
        public string idShort;
    }

    [System.Serializable]
    public class Result
    {
        public List<Asset> result;
    }

    public IEnumerator GetAssetList(System.Action<List<Asset>> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
            callback?.Invoke(null);
        }
        else
        {
            Debug.Log("Data: " + www.downloadHandler.text);

            Result result = JsonUtility.FromJson<Result>(www.downloadHandler.text);
            callback?.Invoke(result.result);
        }
    }

    public IEnumerator DownloadOBJ(string objUrl, System.Action<GameObject> callback)
    {
        Debug.Log("Downloading OBJ from: " + objUrl);

        UnityWebRequest www = UnityWebRequest.Get(objUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
            callback?.Invoke(null);
        }
        else
        {
            Debug.Log("Download successful");

            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject loadedObj = new OBJLoader().Load(textStream);

            callback?.Invoke(loadedObj);
        }
    }

    //Method to download OBJ and save it directly to a file
    public IEnumerator DownloadOBJ(string url, string filePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download OBJ: " + webRequest.error);
            }
            else
            {
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
                Debug.Log("OBJ file saved to: " + filePath);
            }
        }
    }

    private string EncodeToBase64(string url)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(url);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public string ConstructDownloadUrl(string submodelUrl)
    {
        string encodedUrl = EncodeToBase64(submodelUrl);
        return $"{baseUrl}{encodedUrl}/submodel-elements/File/attachment";
    }

    // Server 연결할때
    // public IEnumerator DownloadOBJ(string fileName, System.Action<GameObject> callback)
    // {
    //     string url = "http://127.0.0.1:3000/download/" + fileName;

    //     UnityWebRequest www = UnityWebRequest.Get(url);

    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("Error: " + www.error);
    //         callback?.Invoke(null);
    //     }
    //     else
    //     {
    //         Debug.Log("Download successful");

    //         MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
    //         GameObject loadedObj = new OBJLoader().Load(textStream);

    //         callback?.Invoke(loadedObj);
    //     }
    // }

    //엣날 DownloadOBJ by url
    // public IEnumerator DownloadOBJ(string url, Action<GameObject> callback)
    // {
    //     UnityWebRequest www = UnityWebRequest.Get(url);

    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.LogError($"Failed to download model: {www.error}");
    //         callback?.Invoke(null);
    //         yield break;
    //     }

    //     try
    //     {
    //         byte[] data = www.downloadHandler.data; // Get raw bytes directly
    //         using (MemoryStream textStream = new MemoryStream(data))
    //         {
    //             GameObject loadedObj = new OBJLoader().Load(textStream);

    //             if (loadedObj != null)
    //             {
    //                 callback?.Invoke(loadedObj);
    //             }
    //             else
    //             {
    //                 Debug.LogError("Failed to load model from OBJLoader.");
    //                 callback?.Invoke(null);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError($"Exception occurred while loading model: {ex.Message}");
    //         callback?.Invoke(null);
    //     }
    // }

    //StarterKit obj 파일 받아오는거
    // public IEnumerator DownloadOBJ(System.Action<GameObject> callback)
    // {
    //     string url = "http://172.21.50.200:8081/submodels/aHR0cHM6Ly9leGFtcGxlLmNvbS9pZHMvc20vNjQ5Ml83MDMyXzcwNDJfMDY2OQ/submodel-elements/File/attachment";

    //     UnityWebRequest www = UnityWebRequest.Get(url);

    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("Error: " + www.error);
    //         callback?.Invoke(null);
    //     }
    //     else
    //     {
    //         Debug.Log("Download successful");

    //         MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
    //         GameObject loadedObj = new OBJLoader().Load(textStream);

    //         callback?.Invoke(loadedObj);
    //     }
    // }
}