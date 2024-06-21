using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private Renderer tileRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow; 

    private GameManager gameManager; 
    private int gridX, gridY; 

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }

        gameManager = FindObjectOfType<GameManager>();
    }

    public void Initialize(GameManager manager, int x, int y)
    {
        gameManager = manager;
        gridX = x;
        gridY = y;
    }

    void OnMouseEnter()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = highlightColor; 
        }
    }

    void OnMouseExit()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor; 
        }
    }

    public void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.OnTileDestroyed(gridX, gridY);
            Debug.Log("Destroying tile at (" + gridX + ", " + gridY + ")");
            Destroy(gameObject);
        }
    }
}
