using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public TileState panelState = TileState.Empty;
    private Dictionary<TileController, int> overlappingTiles = new Dictionary<TileController, int>();

    private Color originalColor;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    private Renderer panelRenderer;

    #region Unity Methods

    void Start()
    {
        panelRenderer = GetComponent<Renderer>(); 
        originalColor = panelRenderer.material.color;
    }

    #endregion

    #region Locate Panel

    public void LocatePanel()
    {
        TileController currentTile = GetClosestTile();

        if (currentTile != null)
        {
            if (currentTile.GetTileState() == TileState.Floor)
            {
                Vector3 tilePosition = currentTile.transform.position;
                transform.position = new Vector3(tilePosition.x, 0, tilePosition.z);

                currentTile.SetTileState(TileState.Occupied);
                ResetPanelColor();
            }
        }
    }

    public void SetEmpty()
    {
        panelState = TileState.Empty;
    }

    //Return the closest tile from this panel
    public TileController GetClosestTile()
    {
        float closestDistance = float.MaxValue;
        TileController closestTile = null;

        Vector3 panelCenter = transform.position;

        //Iterate through all overlapping tiles
        foreach (var pair in overlappingTiles)
        {
            TileController tile = pair.Key;
            Vector3 tileCenter = tile.transform.position;

            //Calculate distance between panel center and tile center
            float distance = Vector3.Distance(panelCenter, tileCenter);

            //Update closest tile if this tile is closer
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    //Add a tile to overlappingTiles
    void OnTriggerEnter(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null)
        {
            overlappingTiles[tile] = 1; 
        
            if (tile.GetTileState() == TileState.Floor)
                panelState = TileState.Floor;
            else if(tile.GetTileState() == TileState.Occupied)
                panelState = TileState.Occupied;
        }
    }

    //Remove a tile from overlappingTiles
    void OnTriggerExit(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null && overlappingTiles.ContainsKey(tile))
        {
            overlappingTiles.Remove(tile);
        }
        if (tile != null && tile.GetTileState() == TileState.Floor)
        {
            panelState = TileState.Empty;
        }
    }

    #endregion

    #region Highlight Panel

    public void SetValidColor()
    {
        if (panelRenderer != null)  panelRenderer.material.color = validColor;
    }

    public void SetInvalidColor()
    {
        if (panelRenderer != null)  panelRenderer.material.color = invalidColor;
    }

    public void ResetPanelColor()
    {
        if (panelRenderer != null)  panelRenderer.material.color = originalColor;
    }

    #endregion

}