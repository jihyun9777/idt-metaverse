using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool placed = false;

    private GameManager gameManager;

    #region Unity Methods

    void Start()
    {
        originalPosition = transform.position;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        
    }

    #endregion

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

        UpdatePanelColors();
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

    #region Locate Panel
    
    void OnMouseUp()
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
            }
        }

        //If all tiles are Floor state
        if (canPlacePiece)
        {
            PlacePiece();
            placed = true;
        }
    }

    private void PlacePiece()
    {
        foreach (Transform panel in transform)
        {
            // //Set panel position
            // Vector3 panelPosition = panel.position;
            // int x = Mathf.FloorToInt(panelPosition.x);
            // int y = Mathf.FloorToInt(panelPosition.z);

            // panel.position = new Vector3(x + 0.5f, panel.position.y, y + 0.5f);

            // //Set tiles state to Occupied
            // int[] center = CurrentGridCoordinate();
            // gameManager.floorGrid[x + center[0], y + center[1]] = TileState.Occupied;

            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                TileController currentTile = panelController.GetClosestTile();
                if (currentTile != null)
                {
                    Vector3 tilePosition = currentTile.transform.position;
                    panel.position = new Vector3(tilePosition.x, panel.position.y, tilePosition.z);
                    currentTile.SetTileState(TileState.Occupied);

                    panelController.ResetPanelColor();
                }
            }
        }
    }

    private void ResetTilesToFloor()
    {
        foreach (Transform panel in transform)
        {
            // Vector3 panelPosition = panel.position;
            // int x = Mathf.FloorToInt(panelPosition.x);
            // int y = Mathf.FloorToInt(panelPosition.z);

            // int[] center = CurrentGridCoordinate();

            // gameManager.floorGrid[x + center[0], y + center[1]] = TileState.Floor;

            // Debug.Log((x + center[0]) + " " + (y + center[1]));

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

    public int[] CurrentGridCoordinate()
    {
        int x = Mathf.FloorToInt(gameManager.gridX / 2f);
        int y = Mathf.FloorToInt(gameManager.gridY / 2f);
        return new int[] {x, y};
    }

    #endregion

    #region Highlight

    private void UpdatePanelColors()
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
                    if (currentTile.GetTileState() == TileState.Occupied)
                    {
                        anyOnOccupied = true;
                        break;
                    }   
                }
                else
                {
                    allOnFloor = false;
                    break;
                }
            }
        }

        foreach (Transform panel in transform)
        {
            PanelController panelController = panel.GetComponent<PanelController>();
            if (panelController != null)
            {
                if (anyOnOccupied)
                    panelController.SetInvalidColor();
                else if (allOnFloor)    
                    panelController.SetValidColor();
                else
                    panelController.ResetPanelColor();
            }
        }
    }

    #endregion
}
