using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public GameObject tile;
    public GameObject cameraSystem;

    public float tileWidth = 10f;
    public float tileHeight = 10f;

    private int intX = 1;
    private int intY = 1;
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;

    public void Start()
    {
        CreateFloor(intX, intY);
    }

    public void Update()
    {
        if (int.TryParse(inputX.text, out int newX))
        {
            intX = newX;
            xReady = true;
        }
        if (int.TryParse(inputY.text, out int newY))
        {
            intY = newY;
            yReady = true;
        }
        
        //If x and y are both entered
        if(xReady && yReady && !floorCreated)
        {
            CreateFloor(intX, intY);
            floorCreated = true;
        }
        //If x and y have changed 
        else if(xReady && yReady)
        {
            if(currentFloor.x != intX || currentFloor.y != intY)
                CreateFloor(intX, intY);
        }


        //마우스 클릭시 grid 에 타일 생성
    }

    public void CreateFloor(int x, int y)
    {
        ClearFloor();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(i * tileWidth, 0, j * tileHeight), Quaternion.identity);
                
                newTile.transform.SetParent(transform);
            }
        }

        currentFloor = new Vector2(intX, intY);
        Debug.Log("Floor Created");

        AdjustCameraSystemPosition(x, y);
    }

    public void ClearFloor()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    //Adjust camera position according to the size of floor
    private void AdjustCameraSystemPosition(int x, int y)
    {
        float totalWidth = x * tileWidth;
        float totalHeight = y * tileHeight;
        Vector3 centerPosition;

        //Calculate center of the floor
        if (totalWidth == 10f)
            centerPosition = new Vector3(0, 0, 0);
        else
            centerPosition = new Vector3(totalWidth / 2f, 0, 0);

        //Calculate distance of camera
        float distance = Mathf.Max(totalWidth, totalHeight) * 0.7f; 
        cameraSystem.transform.position = new Vector3(centerPosition.x, distance, centerPosition.z - distance);
    }



}
