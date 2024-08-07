using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SpaceMenuController : MonoBehaviour
{
    public DBAccess dBAccess;
    public GameObject spaceIcon;

    private Vector3 startPosition = new Vector3(510, 620, 0);
    private float iconSpacing = 300f;

    void Start()
    {
        List<SpaceData> spaces = dBAccess.SearchAllSpaces();
        for (int i = 1; i <= spaces.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;

            Vector3 position = startPosition + new Vector3(iconSpacing * column, -iconSpacing * row, 0);
            CreateSpaceIcon(spaces[i - 1], position);
        }
    }

    void Update()
    {
        
    }

    public void LoadCreateSpaceScene()
    {
        SceneManager.LoadScene("CreateSpaceScene");
    }

    public void CreateSpaceIcon(SpaceData spaceData, Vector3 position)
    {
        //Set Icon position
        GameObject newIcon = Instantiate(spaceIcon);

        //Change Button position
        Button spaceButton = newIcon.GetComponentInChildren<Button>();
        RectTransform rectTransform = spaceButton.GetComponentInChildren<RectTransform>();
        rectTransform.position = position;

        //Change Button Name
        TMP_Text spaceText = spaceButton.GetComponentInChildren<TMP_Text>();
        spaceText.text = spaceData.Name;

        //Check if Space has a Preview image
        if (spaceData.Preview != null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(spaceData.Preview);
            newIcon.GetComponentInChildren<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        spaceButton.onClick.AddListener(() => LoadScene(spaceData.Name));
    }

    public void LoadScene(string spaceName)
    {
        SpaceData spaceData = dBAccess.SearchSpaceData(spaceName);

        if (spaceData != null)
        {
            PlayerPrefs.SetInt("SpaceID", spaceData.ID);
            PlayerPrefs.SetString("SpaceName", spaceData.Name);
            PlayerPrefs.SetInt("SpaceX", spaceData.X);
            PlayerPrefs.SetInt("SpaceY", spaceData.Y);
            SceneManager.LoadScene("BaseScene");
        }
        else
        {
            Debug.LogError("Space data not found for: " + spaceName);
        }


    }
}
