using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;
using Newtonsoft.Json; // Newtonsoft.Json 추가

public class InfoNetworking : MonoBehaviour
{
    [System.Serializable]
    public class FileList
    {
        public string[] files;
    }
    
    void Start()
    {
        StartCoroutine(GetFileList());
    }

    

    IEnumerator GetFileList()
    {
        string url = "http://127.0.0.1:3000/files";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            // JSON을 배열로 파싱
            string[] fileList = JsonConvert.DeserializeObject<string[]>(www.downloadHandler.text);

            foreach (string file in fileList)
            {
                Debug.Log("File: " + file);
                // 선택한 파일 이름으로 다운로드 호출
                StartCoroutine(UnityWebRequestGet(file));
            }
        }
    }


    IEnumerator UnityWebRequestGet(string fileName)
    {
        string url = "http://127.0.0.1:3000/download/" + fileName;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("Download successful");
            // OBJ 파일을 GameObject로 변환
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject loadedObj = new OBJLoader().Load(textStream);
            AdjustLoadedObject(loadedObj);
            loadedObj.transform.SetParent(transform);
        }
    }

    private void AdjustLoadedObject(GameObject obj)
    {
        obj.transform.position = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }
}

