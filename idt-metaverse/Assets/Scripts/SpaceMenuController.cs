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
    public Transform iconParent;  // Parent transform to hold the icons

    private Vector3 startPosition = new Vector3(-150, 80, 0);
    private float iconSpacing = 100f;

    void Start()
    {
        List<SpaceData> spaces = dBAccess.ReadAllSpaces();
        for (int i = 0; i < spaces.Count; i++)
        {
            Vector3 position = startPosition + new Vector3(0, -iconSpacing * i, 0);
            CreateSpaceIcon(spaces[i], position);
        }
    }

    void Update()
    {
        
    }

    public void LoadCreateSpaceScene()
    {
        SceneManager.LoadScene("CreateSpaceScene");
    }

    void CreateSpaceIcon(SpaceData spaceData, Vector3 position)
    {
        //Set Icon position
        GameObject newIcon = Instantiate(spaceIcon, iconParent);

        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///버튼 포지션이 바껴야되는데 그 parent 가 바뀌는 이슈
        RectTransform rectTransform = newIcon.GetComponentInChildren<RectTransform>();

        rectTransform.position = position;

        //Change Button Name
        Button spaceButton = newIcon.GetComponentInChildren<Button>();
        TMP_Text spaceText = spaceButton.GetComponentInChildren<TMP_Text>();
        spaceText.text = spaceData.Name;

        //Check if Space has a Preview image
        if (spaceData.Preview != null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(spaceData.Preview);
            newIcon.GetComponentInChildren<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
