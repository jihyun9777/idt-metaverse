using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private Renderer tileRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow; 

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }
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

    void OnMouseDown()
    {
        Destroy(gameObject);
    }
}
