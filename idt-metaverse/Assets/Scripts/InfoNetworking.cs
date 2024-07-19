using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;

public class InfoNetworking : MonoBehaviour
{
    public IEnumerator DownloadOBJ(string fileName, System.Action<GameObject> callback)
    {
        string url = "http://127.0.0.1:3000/download/" + fileName;

        UnityWebRequest www = UnityWebRequest.Get(url);

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
