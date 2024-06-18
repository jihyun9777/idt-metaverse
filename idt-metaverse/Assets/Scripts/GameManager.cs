using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_InputField inputX;
    public TMP_InputField inputY;
    public GameObject tile;

    private int intX = 1;
    private int intY = 1;
    private bool xReady = false;
    private bool yReady = false;
    private bool floorCreated = false;
    private Vector2 currentFloor;

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
        
        //x 랑 y 가 다 정상 값일때 
        if(xReady && yReady && !floorCreated)
        {
            CreateFloor(intX, intY);
            floorCreated = true;
        }
        //x 랑 y 값이 바뀌었을때 
        else if(xReady && yReady)
        {
            if(currentFloor.x != intX || currentFloor.y != intY)
                CreateFloor(intX, intY);
        }
    }

    public void CreateFloor(int x, int y)
    {
        ClearFloor();

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity);
                
                newTile.transform.SetParent(transform);
            }
        }

        currentFloor = new Vector2(intX, intY);
        Debug.Log("Floor Created");
    }

    public void ClearFloor()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }



}
