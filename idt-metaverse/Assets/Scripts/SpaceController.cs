using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void LoadCreateSpaceScene()
    {
        SceneManager.LoadScene("CreateSpaceScene");
    }


}
