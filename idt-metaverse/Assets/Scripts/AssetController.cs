using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetController : MonoBehaviour
{
    private Vector3 offset;
    private Color originalColor;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    private Renderer objectRenderer;
    private BoxCollider boxCollider;

    private bool isLocated = false;

    public event Action<string, Vector3> OnPositionChanged;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); 
        originalColor = objectRenderer.material.color;
        boxCollider = GetComponent<BoxCollider>();
    }
    
    #region Mouse Drag

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isLocated = false;
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + offset;
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

    private void UpdateObjectColor()
    {
        if (isLocated)
            objectRenderer.material.color = originalColor;
        else if (IsOnTile())
            objectRenderer.material.color = validColor;
        else
            objectRenderer.material.color = invalidColor;
    }

    private bool IsOnTile()
    {
        Bounds objectBounds = boxCollider.bounds;

        //Calculate the bottom corners of the object's bounds
        Vector3[] bottomCorners = new Vector3[4];
        bottomCorners[0] = new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.min.z);
        bottomCorners[1] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.min.z);
        bottomCorners[2] = new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.max.z);
        bottomCorners[3] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.max.z);

        //Check for tiles underneath the object
        foreach (Vector3 corner in bottomCorners)
        {
            Collider[] colliders = Physics.OverlapSphere(corner, 0.1f);

            bool isOnTile = false;
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Tile"))
                {
                    isOnTile = true;
                    break;
                }
            }

            if (!isOnTile)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Locate Asset

    void OnMouseUp()
    {
        Vector3 position = transform.position;
        Vector3 roundedPosition = new Vector3(
            Mathf.Round(position.x),
            position.y,
            Mathf.Round(position.z)
        );

        //Temporarily move the object to the rounded position
        transform.position = roundedPosition;

        //If fully on the tile, keep the rounded position
        if (IsOnTile())
        {
            transform.position = roundedPosition;
            isLocated = true;

            //Trigger the position changed event
            OnPositionChanged?.Invoke(gameObject.name, transform.position);
        }
        //If not fully on the tile, revert to the original position
        else
        {
            transform.position = position;
            isLocated = false;
        }

        UpdateObjectColor();
    }

    #endregion
}
