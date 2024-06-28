using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    // private Vector3 offset;
    // private Dictionary<TileController, int> overlappingTiles = new Dictionary<TileController, int>();
    // public float fixedYPosition;
    // private Vector3 originalPosition;

    // private Color originalColor;
    // public Color validColor = Color.green;
    // public Color invalidColor = Color.red;
    // private Renderer panelRenderer;

    // void Start()
    // {
    //     panelRenderer = GetComponent<Renderer>(); 
    //     originalColor = panelRenderer.material.color;
    //     originalPosition = transform.position;
    // }

    // #region Mouse Drag

    // void OnMouseDown()
    // {
    //     offset = transform.position - GetMouseWorldPosition();

    //     TileController currentTile = GetClosestTile();
    //     if (currentTile != null)
    //     {
    //         currentTile.SetTileState(TileState.Floor);
    //     }
    // }

    // void OnMouseDrag()
    // {
    //     Vector3 newPos = GetMouseWorldPosition() + offset;
    //     transform.position = new Vector3(newPos.x, fixedYPosition, newPos.z);
    //     UpdatePanelColor();
    // }

    // private Vector3 GetMouseWorldPosition()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     Plane plane = new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0));

    //     float distance;
    //     if (plane.Raycast(ray, out distance))
    //     {
    //         return ray.GetPoint(distance);
    //     }

    //     return transform.position;
    // }

    // #endregion

    // #region Locate Panel
    
    // void OnMouseUp()
    // {
    //     TileController currentTile = GetClosestTile();

    //     if (currentTile != null)
    //     {
    //         //Place panel on a tile (Floor)
    //         if (currentTile.GetTileState() == TileState.Floor)
    //         {
    //             Vector3 tilePosition = currentTile.transform.position;
    //             transform.position = new Vector3(tilePosition.x, fixedYPosition, tilePosition.z);
    //             originalPosition = new Vector3(tilePosition.x, fixedYPosition, tilePosition.z);

    //             currentTile.SetTileState(TileState.Occupied);
    //             SetPanelColor(originalColor);
    //         }
    //         //Banning panel to place on an occupied tile
    //         else if (currentTile.GetTileState() == TileState.Occupied)
    //         {
    //             //Reset panel to original position
    //             transform.position = originalPosition;
    //             SetPanelColor(originalColor);
    //         }
    //     }
    //     else
    //         UpdatePanelColor();
    // }

    // //Return the closest tile from this panel
    // private TileController GetClosestTile()
    // {
    //     float closestDistance = float.MaxValue;
    //     TileController closestTile = null;

    //     Vector3 panelCenter = transform.position;

    //     //Iterate through all overlapping tiles
    //     foreach (var pair in overlappingTiles)
    //     {
    //         TileController tile = pair.Key;
    //         Vector3 tileCenter = tile.transform.position;

    //         //Calculate distance between panel center and tile center
    //         float distance = Vector3.Distance(panelCenter, tileCenter);

    //         //Update closest tile if this tile is closer
    //         if (distance < closestDistance)
    //         {
    //             closestDistance = distance;
    //             closestTile = tile;
    //         }
    //     }

    //     return closestTile;
    // }

    // //Add a tile to overlappingTiles
    // void OnTriggerEnter(Collider other)
    // {
    //     TileController tile = other.GetComponent<TileController>();
    //     if (tile != null)
    //     {
    //         overlappingTiles[tile] = 1; 
    //     }
    // }

    // //Remove a tile from overlappingTiles
    // void OnTriggerExit(Collider other)
    // {
    //     TileController tile = other.GetComponent<TileController>();
    //     if (tile != null && overlappingTiles.ContainsKey(tile))
    //     {
    //         overlappingTiles.Remove(tile);
    //     }
    // }

    // #endregion

    // #region Highlight Panel

    // private void UpdatePanelColor()
    // {
    //     TileController currentTile = GetClosestTile();

    //     if (currentTile != null)
    //     {
    //         TileState tileState = currentTile.GetTileState();
    //         if (tileState == TileState.Floor)
    //         {
    //             panelRenderer.material.color = validColor;
    //         }
    //         else if (tileState == TileState.Occupied)
    //         {
    //             panelRenderer.material.color = invalidColor;
    //         }
    //     }
    //     else
    //     {
    //         panelRenderer.material.color = originalColor;
    //     }
    // }

    // private void SetPanelColor(Color color)
    // {
    //     panelRenderer.material.color = color;
    // }

    // #endregion

    public bool panelState = false;

    void OnTriggerEnter(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null && tile.GetTileState() == TileState.Floor)
        {
            panelState = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null && tile.GetTileState() == TileState.Floor)
        {
            panelState = false;
        }
    }
}