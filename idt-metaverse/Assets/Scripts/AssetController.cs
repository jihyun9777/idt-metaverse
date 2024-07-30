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
    private Bounds objectBounds;
    private Vector3[] bottomCorners;

    private bool isLocated = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); 
        originalColor = objectRenderer.material.color;
        boxCollider = GetComponent<BoxCollider>();

        objectBounds = boxCollider.bounds;
        bottomCorners = new Vector3[4];
        bottomCorners[0] = new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.min.z);
        bottomCorners[1] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.min.z);
        bottomCorners[2] = new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.max.z);
        bottomCorners[3] = new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.max.z);
    }

    void Update()
    {
        
    }

    
    #region Mouse Drag

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isLocated = false;
    }

    void OnMouseDrag()
    {
        Vector3 newPos = GetMouseWorldPosition() + offset;
        transform.position = newPos;
        //UpdateObjectColor();
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

    public void UpdateObjectColor()
    {
        if (isLocated)
            objectRenderer.material.color = originalColor;
        else if (IsFullyOnTile())
            objectRenderer.material.color = validColor;
        else
            objectRenderer.material.color = invalidColor;
    }

    bool IsFullyOnTile()
    {
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
        if (IsFullyOnTile())
            Debug.Log("Hey");
        // if (IsFullyOnTile())
        // {
        //     Vector3 objectSize = objectBounds.size;

        //     Vector3 objectCenter = objectBounds.center;
        //     Vector3 objectBottomCenter = new Vector3(objectCenter.x, objectBounds.min.y, objectCenter.z);

        //     //Determine the tile size (1x1) and position
        //     float tileSize = 1f;

        //     //Calculate the offset required to move the object to the closest tile center
        //     Vector3 offset = new Vector3(
        //         Mathf.Round(objectBottomCenter.x / tileSize) * tileSize - objectBottomCenter.x,
        //         0,
        //         Mathf.Round(objectBottomCenter.z / tileSize) * tileSize - objectBottomCenter.z
        //     );

        //     transform.position += offset;
        //     isLocated = true;
        // }
    }

    #endregion
}
