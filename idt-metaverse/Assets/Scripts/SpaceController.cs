using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceController : MonoBehaviour
{
    public TMP_InputField inputName;
    public TMP_InputField inputX;
    public TMP_InputField inputY;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadSpaceMenuScene()
    {
        SceneManager.LoadScene("SpaceMenuScene");
    }

    public void CreateSpace()
    {
        //If inputs are all entered correctly
        if (!string.IsNullOrEmpty(inputName.text) && !string.IsNullOrEmpty(inputX.text) && !string.IsNullOrEmpty(inputY.text))
        {
            PlayerPrefs.SetString("SpaceName", inputName.text);
            PlayerPrefs.SetInt("SpaceX", int.Parse(inputX.text));
            PlayerPrefs.SetInt("SpaceY", int.Parse(inputY.text));

            StartCoroutine(CreateNewSpace());
        }
        //Else, change boxes' color red
        else
        {
            if (string.IsNullOrEmpty(inputName.text))
                inputName.GetComponent<Image>().color = Color.red;
            if (string.IsNullOrEmpty(inputX.text))
                inputX.GetComponent<Image>().color = Color.red;
            if (string.IsNullOrEmpty(inputY.text))
                inputY.GetComponent<Image>().color = Color.red;
        }
    }

    private IEnumerator CreateNewSpace()
    {
        string newSceneName = inputName.text;
        Scene newScene = SceneManager.CreateScene(newSceneName);

        yield return SceneManager.SetActiveScene(newScene);

        GameObject baseSceneInstance = Instantiate(baseScenePrefab);
        
        InitializeBaseScene(baseSceneInstance);
    }

    private void InitializeBaseScene(GameObject baseSceneInstance)
    {
        int x = int.Parse(PlayerPrefs.GetInt("SpaceX"));
        int y = int.Parse(PlayerPrefs.GetInt("SpaceY"));

        GameObject newObject = new GameObject("NewObject");
        newObject.transform.position = new Vector3(x, y, 0);
        newObject.transform.SetParent(baseSceneInstance.transform);
    }
}
