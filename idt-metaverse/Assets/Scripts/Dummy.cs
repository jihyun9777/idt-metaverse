using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    #region piece

    // void OnMouseDown()
    // {
    //     Vector3 mousePos = GetMouseWorldPosition();
    //     offset = transform.position - new Vector3(mousePos.x, 0f, mousePos.z);

    //     if (placed)
    //     {
    //         ResetTilesToFloor();
    //         placed = false;
    //     }

    // }

    // void OnMouseDrag()
    // {
    //     Vector3 mousePos = GetMouseWorldPosition();
    //     transform.position = new Vector3(mousePos.x, 0f, mousePos.z) + offset;

    //     UpdatePanelColors();
    // }

    // private Vector3 GetMouseWorldPosition()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         return hit.point;
    //     }

    //     return transform.position;
    // }

    // private void PlacePiece()
    // {
    //     foreach (Transform panel in transform)
    //     {
    //         // //Set panel position
    //         // Vector3 panelPosition = panel.position;
    //         // int x = Mathf.FloorToInt(panelPosition.x);
    //         // int y = Mathf.FloorToInt(panelPosition.z);

    //         // panel.position = new Vector3(x + 0.5f, panel.position.y, y + 0.5f);

    //         // //Set tiles state to Occupied
    //         // int[] center = CurrentGridCoordinate();
    //         // gameManager.floorGrid[x + center[0], y + center[1]] = TileState.Occupied;

    //         PanelController panelController = panel.GetComponent<PanelController>();
    //         if (panelController != null)
    //         {
    //             TileController currentTile = panelController.GetClosestTile();
    //             if (currentTile != null)
    //             {
    //                 Vector3 tilePosition = currentTile.transform.position;
    //                 panel.position = new Vector3(tilePosition.x, panel.position.y, tilePosition.z);
    //                 currentTile.SetTileState(TileState.Occupied);

    //                 panelController.ResetPanelColor();
    //             }
    //         }
    //     }
    // }

    // private void ResetTilesToFloor()
    // {
    //     foreach (Transform panel in transform)
    //     {
    //         // Vector3 panelPosition = panel.position;
    //         // int x = Mathf.FloorToInt(panelPosition.x);
    //         // int y = Mathf.FloorToInt(panelPosition.z);

    //         // int[] center = CurrentGridCoordinate();

    //         // gameManager.floorGrid[x + center[0], y + center[1]] = TileState.Floor;

    //         // Debug.Log((x + center[0]) + " " + (y + center[1]));

    //         PanelController panelController = panel.GetComponent<PanelController>();
    //         if (panelController != null)
    //         {
    //             TileController currentTile = panelController.GetClosestTile();
    //             if (currentTile != null)
    //             {
    //                 currentTile.SetTileState(TileState.Floor);
    //             }
    //         }
    //     }
    // }

    // public int[] CurrentGridCoordinate()
    // {
    //     int x = Mathf.FloorToInt(gameManager.gridX / 2f);
    //     int y = Mathf.FloorToInt(gameManager.gridY / 2f);
    //     return new int[] {x, y};
    // }

    #endregion

    #region panel

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

    #endregion
}
