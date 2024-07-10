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
        if (pieceXReady && pieceYReady && objectReady)
        { 
            Vector3 pieceCenter = pieceController.PieceCenterPosition();
            obj.transform.position = new Vector3(pieceCenter.x, 0, pieceCenter.z);
            piece.transform.SetParent(obj.transform, true);

            objectReady = false;
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    //When Load Object Button is pressed
    public void LoadObject()
    {
        GameObject file = Resources.Load<GameObject>(FileName.text);

        if (file != null)
        {
            obj = Instantiate(file, Vector3.zero, Quaternion.identity);
            objectReady = true;

            BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
            obj.AddComponent<Rigidbody>().isKinematic = true;
            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<ObjectController>();
            
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;

                foreach (Renderer renderer in renderers)
                    bounds.Encapsulate(renderer.bounds);

                boxCollider.size = bounds.size;
                boxCollider.center = bounds.center - obj.transform.position;
            }
        }
        else
        {
            Debug.LogError($"Failed to load prefab from Resources folder: {FileName}");
        }
    }

    //When Add Object Button is pressed
    public void Import()
    {    
        DontDestroyOnLoad(obj);
        PlayerPrefs.SetString("ObjectName", FileName.text);
        LoadMainScene();
    }

    // //Create Piece and set as child
    // public void CreateObject()
    // {
    //     CreatePiece();

    //     //Place Object on the center of Piece
    //     Vector3 pieceCenter = pieceController.PieceCenterPosition();
    //     obj.transform.position = new Vector3(pieceCenter.x, 0, pieceCenter.z);
    //     Debug.Log(obj.transform.position);

    //     piece.transform.SetParent(obj.transform, true); 

    //     obj.AddComponent<BoxCollider>();
    //     obj.AddComponent<MeshRenderer>();
    //     obj.AddComponent<Rigidbody>().isKinematic = true;
    //     obj.AddComponent<ObjectController>();
    // }

    public void CreatePiece()
    {
        if (int.TryParse(pieceSizeX.text, out pieceintX))
            pieceXReady = true;
        if (int.TryParse(pieceSizeY.text, out pieceintY))
            pieceYReady = true;

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
