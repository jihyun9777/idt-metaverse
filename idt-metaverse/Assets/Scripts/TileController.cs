using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2 gridPosition;

    private Renderer tileRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow; 

    private GameManager gameManager; 
    private int gridX, gridY; 

    void Start()
    {
        gridPosition = new Vector2(transform.position.x, transform.position.z);
        
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
        if (tileRenderer != null && gameManager.createMode)
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
        if (gameManager != null && gameManager.createMode)
        {
            gameManager.OnTileDestroyed(gridX, gridY);
            Destroy(gameObject);
        }
    }
}
