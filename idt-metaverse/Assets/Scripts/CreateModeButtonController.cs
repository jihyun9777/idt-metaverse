using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateModeButtonController : MonoBehaviour
{
    private GameManager gameManager; 
    public Color OnColor = Color.green;
    public Color OffColor = Color.white;
    public Button button;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if(gameManager.createMode)
            ChangeButtonColors(OnColor);
    }

    public void ChangeCreateMode()
    {
        gameManager.createMode = !gameManager.createMode;

        if (gameManager.createMode)
            ChangeButtonColors(OnColor);
        else
            ChangeButtonColors(OffColor);
    }

    private void ChangeButtonColors(Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        button.colors = cb;
    }
}
