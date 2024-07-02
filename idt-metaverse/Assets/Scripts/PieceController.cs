using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    private Vector3 offset;
    public bool placed = false;
    HashSet<TileController> occupiedTiles = new HashSet<TileController>();

    #region Mouse Drag

    void OnMouseDown()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        offset = transform.position - new Vector3(mousePos.x, 0f, mousePos.z);

        if (placed)
        {
            ResetTilesToFloor();
            placed = false;
        }

    }

    void OnMouseDrag()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        transform.position = new Vector3(mousePos.x, 0f, mousePos.z) + offset;

        UpdatePieceColor();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }

        return transform.position;
    }

    #endregion

    #region Locate Piece
    
    public void OnMouseUp()
    {
        bool canPlacePiece = true;

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
            }
        }
        occupiedTiles.Clear();

        //If all tiles are Floor state, locate each panel
        if (canPlacePiece)
        {
            foreach (Transform panel in transform)
            {
                PanelController panelController = panel.GetComponent<PanelController>();
                if (panelController != null)
                {
                    panelController.LocatePanel();
                }
            }
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

    #region Highlight

    public void UpdatePieceColor()
    {
        bool allOnFloor = true;
        bool anyOnOccupied = false;

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
                if (anyOnOccupied)  panelController.SetInvalidColor();
                if (allOnFloor)     panelController.SetValidColor();
                else                panelController.ResetPanelColor();
            }
        }
    }

    #endregion
}
