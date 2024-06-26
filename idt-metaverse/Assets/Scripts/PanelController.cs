using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private TileController currentTile;
    public float fixedYPosition;

    void Start()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Rigidbody를 추가하고 kinematic으로 설정
        }
    }

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        Vector3 newPos = GetMouseWorldPosition() + offset;
        transform.position = new Vector3(newPos.x, fixedYPosition, newPos.z);
    }

    void OnMouseUp()
    {
        if (currentTile != null)
        {
            Debug.Log("Here");
            Vector3 tilePosition = currentTile.transform.position;
            transform.position = new Vector3(tilePosition.x, fixedYPosition, tilePosition.z);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedYPosition, 0));

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null)
        {
            Debug.Log("Here");
            currentTile = tile;
        }
    }

    void OnTriggerExit(Collider other)
    {
        TileController tile = other.GetComponent<TileController>();
        if (tile != null && currentTile == tile)
        {
            currentTile = null;
        }
    }
}
