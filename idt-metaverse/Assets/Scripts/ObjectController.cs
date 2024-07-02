using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // private Vector3 offset;

    // private PieceController piece;

    // void Start()
    // {
    //     //로직에 따라 선택
    //     piece = GetComponentInChildren<PieceController>();
    // }

    // #region Mouse Drag

    // void OnMouseDown()
    // {
    //     offset = transform.position - GetMouseWorldPosition();

    //     //로직에 따라 선택
    //     piece = GetComponentInChildren<PieceController>();

    //     if (piece.placed)
    //     {
    //         piece.ResetTilesToFloor();
    //         piece.placed = false;
    //     }
    // }

    // void OnMouseDrag()
    // {
    //     Vector3 newPos = GetMouseWorldPosition() + offset;
    //     transform.position = new Vector3(newPos.x, 0, newPos.z);
    //     piece.UpdatePieceColor();
    // }

    // private Vector3 GetMouseWorldPosition()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));

    //     float distance;
    //     if (plane.Raycast(ray, out distance))
    //     {
    //         return ray.GetPoint(distance);
    //     }

    //     return transform.position;
    // }

    // #endregion

    // #region Locate Object
    
    // void OnMouseUp()
    // {
    //     piece.OnMouseUp();
    //     //transform.position = piece.transform.position;
    // }

    // #endregion

}
