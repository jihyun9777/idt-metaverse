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

    public void LoadBaseScene()
    {
        SceneManager.LoadScene("BaseScene");
    }

    public void CreateSpace()
    {
        //If inputs are all entered correctly
        if (!string.IsNullOrEmpty(inputName.text) && !string.IsNullOrEmpty(inputX.text) && !string.IsNullOrEmpty(inputY.text))
        {
            PlayerPrefs.SetString("SpaceName", inputName.text);
            PlayerPrefs.SetInt("SpaceX", int.Parse(inputX.text));
            PlayerPrefs.SetInt("SpaceY", int.Parse(inputY.text));

            LoadBaseScene();
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
}
