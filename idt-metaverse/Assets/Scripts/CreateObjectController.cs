using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateObjectController : MonoBehaviour
{
    //Initialize outside
    public TMP_InputField pieceSizeX;
    public TMP_InputField pieceSizeY;
    public TMP_InputField FileName;
    public GameObject panel;

    public GameObject piece;
    public PieceController pieceController;
    public GameObject obj;
    public ObjectController ObjectController;

    private int pieceintX = 1;
    private int pieceintY = 1;
    private bool pieceXReady = false;
    private bool pieceYReady = false;
    private bool objectReady = false;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    //When Load Object Button is pressed
    public void LoadPrefabButton()
    {
        GameObject file = Resources.Load<GameObject>(FileName.text);

        if (file != null)
        {
            obj = Instantiate(file, Vector3.zero, Quaternion.identity);
            Debug.Log(obj.transform.position);
        }
        else
        {
            Debug.LogError($"Failed to load prefab from Resources folder: {FileName}");
        }
    }

    //When Add Object Button is pressed
    public void CreateObjectButton()
    {
        if (int.TryParse(pieceSizeX.text, out pieceintX))
            pieceXReady = true;
        if (int.TryParse(pieceSizeY.text, out pieceintY))
            pieceYReady = true;
        //Check imported game object
        if (obj != null)
            objectReady = true;
        if (pieceXReady && pieceYReady && objectReady)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        CreatePiece();

        //Place Object on the center of Piece
        Vector3 pieceCenter = pieceController.PieceCenterPosition();
        obj.transform.position = new Vector3(pieceCenter.x, 0, pieceCenter.z);
        Debug.Log(obj.transform.position);

        piece.transform.SetParent(obj.transform, true); 

        obj.AddComponent<BoxCollider>();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<Rigidbody>().isKinematic = true;
        ObjectController = obj.AddComponent<ObjectController>();
    }

    private void CreatePiece()
    {
        piece = new GameObject("Piece");
        // piece.AddComponent<BoxCollider>();
        // piece.AddComponent<Rigidbody>().isKinematic = true;

        for (int i = 0; i < pieceintX; i++)
        {
            for (int j = 0; j < pieceintY; j++)
            {
                GameObject newPanel = Instantiate(panel, piece.transform);
                Vector3 panelPosition = startPosition + new Vector3(i, 0, j);
                newPanel.transform.position = panelPosition;
            }
        }

        pieceController = piece.AddComponent<PieceController>();
    }
}
