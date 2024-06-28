using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;

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

    public void OnMouseDown()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        offset = transform.position - new Vector3(mousePos.x, 0f, mousePos.z);
    }

    public void OnMouseDrag()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        transform.position = new Vector3(mousePos.x, 0f, mousePos.z) + offset;
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
            if (panelController != null && !panelController.panelState)
            {
                canPlacePiece = false;
                break;
            }
        }

        if (canPlacePiece)
        {
            PlacePiece();
        }
        else
        {
            transform.position = originalPosition;
        }

        //Empty 에 놓여져있는경우 추가
    }

    //되긴되는데 정확한 위치 선정 필요
    private void PlacePiece()
    {
        foreach (Transform panel in transform)
        {
            Vector3 panelPosition = panel.position;
            int x = Mathf.FloorToInt(panelPosition.x);
            int y = Mathf.FloorToInt(panelPosition.z);

            panelPosition.x = x;
            panelPosition.z = y;
            panel.position = panelPosition;

            // 타일 상태를 Occupied로 설정
            gameManager.floorGrid[x, y] = TileState.Occupied;
        }
    }

    #endregion

    //highlight 로직 짜기
}
