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
    public int gridX, gridY; 

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

    public TileState GetTileState()
    {
        return gameManager.floorGrid[gridX, gridY];
    }

    public void SetTileState(TileState state)
    {
        gameManager.floorGrid[gridX, gridY] = state;
    }

    //Highlight this tile when mouse enter
    void OnMouseEnter()
    {
        if (tileRenderer != null && gameManager.createMode)
        {
            tileRenderer.material.color = highlightColor; 
        }
    }

    //Un-do highlight when mouse exit
    void OnMouseExit()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor; 
        }
    }

    //Destroy this tile when clicked
    public void OnMouseDown()
    {
        if (gameManager != null && gameManager.createMode)
        {
            gameManager.OnTileDestroyed(gridX, gridY);
            Destroy(gameObject);
        }
    }
}
