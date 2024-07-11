using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SFB;
using Dummiesman;

public class OpenFile : MonoBehaviour
{
    public delegate void ObjLoadedHandler(GameObject loadedObj);

    public void OnClickOpen(ObjLoadedHandler callback)
    {
        //WebGL 추가 

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "obj", false);

        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutineOpen(new System.Uri(paths[0]).AbsoluteUri, callback));
        }
    }

    private IEnumerator OutputRoutineOpen(string url, ObjLoadedHandler callback)
    {
        //WebGL 추가 

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log("WWW ERROR: " + www.error);
        else
        {
            MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject loadedObj = new OBJLoader().Load(textStream);
            callback?.Invoke(loadedObj);
        }
    }

    // private FitOnScreen()
    // {
    //     Bounds bound = GetBound(obj);
    //     Vector3 boundSize = bound.sixe;
    //     float diagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));

    // }
    
}
