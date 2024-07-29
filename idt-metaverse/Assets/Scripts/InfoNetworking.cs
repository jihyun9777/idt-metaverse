// using System.IO;
// using System.Text;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.Networking;
// using Dummiesman;

// public class InfoNetworking : MonoBehaviour
// {
//     public IEnumerator DownloadOBJ(string fileName, System.Action<GameObject> callback)
//     {
//         string url = "http://127.0.0.1:3000/download/" + fileName;

//         UnityWebRequest www = UnityWebRequest.Get(url);

//         yield return www.SendWebRequest();

//         if (www.result != UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Error: " + www.error);
//             callback?.Invoke(null);
//         }
//         else
//         {
//             Debug.Log("Download successful");

//             MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
//             GameObject loadedObj = new OBJLoader().Load(textStream);

//             callback?.Invoke(loadedObj);
//         }
//     }

//     public IEnumerator DownloadOBJ(System.Action<GameObject> callback)
//     {
//         string url = "http://172.21.50.200:8081/submodels/aHR0cHM6Ly9leGFtcGxlLmNvbS9pZHMvc20vNjQ5Ml83MDMyXzcwNDJfMDY2OQ/submodel-elements/File/attachment";

//         UnityWebRequest www = UnityWebRequest.Get(url);

//         yield return www.SendWebRequest();

//         if (www.result != UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Error: " + www.error);
//             callback?.Invoke(null);
//         }
//         else
//         {
//             Debug.Log("Download successful");

//             MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
//             GameObject loadedObj = new OBJLoader().Load(textStream);

//             callback?.Invoke(loadedObj);
//         }
//     }
// }

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;

public class InfoNetworking : MonoBehaviour
{
    public string jsonUrl = "http://172.21.50.200:8081/shells";

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
        public List<DisplayName> displayName;
    }

    [System.Serializable]
    public class DisplayName
    {
        public string language;
        public string text;
    }

    [System.Serializable]
    public class Result
    {
        public List<Asset> result;
    }

    public IEnumerator GetAssets(System.Action<List<Asset>> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(jsonUrl);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
            callback?.Invoke(null);
        }
        else
        {
            Debug.Log("JSON Download successful");
            Debug.Log("JSON Data: " + www.downloadHandler.text);

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
}

