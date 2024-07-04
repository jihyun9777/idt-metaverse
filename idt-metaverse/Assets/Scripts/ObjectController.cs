using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 offset;
    private PieceController piece;

    private Color originalColor;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    private Renderer objectRenderer;

    #region Unity Methods

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); 
        originalColor = objectRenderer.material.color;
    }

    #endregion

    #region Mouse Drag

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();

        piece = GetComponentInChildren<PieceController>();

        if (piece.placed)
        {
            piece.ResetTilesToFloor();
            piece.placed = false;
        }
    }

    void OnMouseDrag()
    {
        Vector3 newPos = GetMouseWorldPosition() + offset;
        transform.position = new Vector3(newPos.x, 0, newPos.z);
        UpdateObjectColor();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    #endregion

    #region Locate Object
    
    void OnMouseUp()
    {
        piece.LocatePiece();

        if(piece.placed)
        {
            transform.position = new Vector3 (piece.centerPosition.x, 0f, piece.centerPosition.z);
            objectRenderer.material.color = originalColor;
        }
    }

    #endregion

    #region Highlight Object

    public void UpdateObjectColor()
    {
        piece.UpdatePieceColor();

        if (piece.anyOnOccupied)    objectRenderer.material.color = invalidColor;
        else if (piece.allOnFloor)  objectRenderer.material.color = validColor;
        else                        objectRenderer.material.color = originalColor;
    }

    #endregion
}
