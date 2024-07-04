using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    //private Vector3 offset;
    public bool placed = false;
    public Vector3 piecePosition;
    public Vector3 centerPosition;
    public bool allOnFloor = true;
    public bool anyOnOccupied = false;

    HashSet<TileController> occupiedTiles = new HashSet<TileController>();

    #region Mouse Drag

    // public void OnMouseDown()
    // {
    //     Vector3 mousePos = GetMouseWorldPosition();
    //     offset = transform.position - new Vector3(mousePos.x, 0f, mousePos.z);

    //     if (placed)
    //     {
    //         ResetTilesToFloor();
    //         placed = false;
    //     }

    // }

    // public void OnMouseDrag()
    // {
    //     Vector3 mousePos = GetMouseWorldPosition();
    //     transform.position = new Vector3(mousePos.x, 0f, mousePos.z) + offset;

    //     UpdatePieceColor();
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

    #endregion

    #region Locate Piece
    
    public void LocatePiece()
    {
        bool canPlacePiece = true;
        piecePosition = new Vector3(float.MaxValue, 0, float.MaxValue);

        foreach (Transform panel in transform)
        {
            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                if (panelController.panelState == TileState.Empty || panelController.panelState == TileState.Occupied)
                {
                    canPlacePiece = false;
                    break;
                }

                //Prevent panels pointing same tile
                TileController currentTile = panelController.GetClosestTile();
                if (occupiedTiles.Contains(currentTile))
                {
                    canPlacePiece = false;
                    break;
                }
                occupiedTiles.Add(currentTile);

                // Update the minimum position
                piecePosition = new Vector3( Mathf.Min(piecePosition.x, currentTile.transform.position.x), 0f, 
                                                Mathf.Min(piecePosition.z, currentTile.transform.position.z));
            }
        }
        occupiedTiles.Clear();

        if (canPlacePiece)
        {
            //transform.position = piecePosition;

            Vector3 minPosition = new Vector3(float.MaxValue, 0, float.MaxValue);
            Vector3 maxPosition = new Vector3(float.MinValue, 0, float.MinValue);

            foreach (Transform panel in transform)
            {
                PanelController panelController = panel.GetComponent<PanelController>();
                TileController currentTile = panelController.GetClosestTile();

                if (panelController != null && currentTile != null)
                {
                    Vector3 tilePos = currentTile.transform.position;
                    minPosition = Vector3.Min(minPosition, tilePos);
                    maxPosition = Vector3.Max(maxPosition, tilePos);

                    //panelController.LocatePanel();
                    currentTile.SetTileState(TileState.Occupied);
                    panelController.ResetPanelColor();
                }
            }

            centerPosition = (minPosition + maxPosition) / 2f;
            Debug.Log("CenterPostion" + centerPosition);
            placed = true;
        }
    }

    public void ResetTilesToFloor()
    {
        foreach (Transform panel in transform)
        {
            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                TileController currentTile = panelController.GetClosestTile();
                if (currentTile != null)
                {
                    currentTile.SetTileState(TileState.Floor);
                }
            }
        }
    }

    #endregion

    #region Highlight Piece

    public void UpdatePieceColor()
    {
        allOnFloor = true;
        anyOnOccupied = false;

        foreach (Transform panel in transform)
        {
            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                TileController currentTile = panelController.GetClosestTile();
                if (currentTile != null)
                {
                    //Check Occupied tiles
                    if (currentTile.GetTileState() == TileState.Occupied)
                    {
                        anyOnOccupied = true;
                        allOnFloor = false;
                        break;
                    }
                    //Check multiple pointed tiles
                    if (occupiedTiles.Contains(currentTile))   
                    {
                        allOnFloor = false;
                        break;
                    }
                    occupiedTiles.Add(currentTile);
                }
                //Check Empty tiles
                else
                {
                    allOnFloor = false;
                    break;
                }
            }
        }
        occupiedTiles.Clear();

        foreach (Transform panel in transform)
        {
            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                if (anyOnOccupied)      panelController.SetInvalidColor();
                else if (allOnFloor)    panelController.SetValidColor();
                else                    panelController.ResetPanelColor();
            }
        }
    }

    #endregion
}
