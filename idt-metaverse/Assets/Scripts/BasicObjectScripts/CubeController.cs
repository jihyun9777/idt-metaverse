using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CubeController : MonoBehaviour
{
    public float doubleClickTimeLimit = 0.3f; 
    private float lastClickTime = -1f; 
    private GameObject property;

    private Button closeButton;

    //Position
    private TMP_InputField xPosInputField;
    private TMP_InputField yPosInputField;
    private TMP_InputField zPosInputField;

    //Dimension
    private TMP_InputField lDimInputField;
    private TMP_InputField wDimInputField;
    private TMP_InputField hDimInputField;

    //Rotation
    private TMP_InputField xRotInputField;
    private TMP_InputField yRotInputField;
    private TMP_InputField zRotInputField;

    //Scale
    // private TMP_InputField xScaleInputField;
    // private TMP_InputField yScaleInputField;
    // private TMP_InputField zScaleInputField;

    private Button deleteButton;

    void Start()
    {
        GameObject cubePropertyPrefab = Resources.Load<GameObject>("BasicObjects/" + "CubeProperty");

        if (cubePropertyPrefab != null)
        {
            property = Instantiate(cubePropertyPrefab, transform.position, Quaternion.identity);

            closeButton = property.transform.Find("CloseButton").GetComponent<Button>();
            closeButton.onClick.AddListener(() => CloseTab());

            //Position Inputs
            xPosInputField = property.transform.Find("Position/XPosInputField").GetComponent<TMP_InputField>();
            yPosInputField = property.transform.Find("Position/YPosInputField").GetComponent<TMP_InputField>();
            zPosInputField = property.transform.Find("Position/ZPosInputField").GetComponent<TMP_InputField>();
            xPosInputField.text = "0";
            yPosInputField.text = "0";
            zPosInputField.text = "0";
            xPosInputField.onEndEdit.AddListener(delegate { UpdatePosition(); });
            yPosInputField.onEndEdit.AddListener(delegate { UpdatePosition(); });
            zPosInputField.onEndEdit.AddListener(delegate { UpdatePosition(); });

            //Dimension Inputs
            lDimInputField = property.transform.Find("Dimension/LDimInputField").GetComponent<TMP_InputField>();
            wDimInputField = property.transform.Find("Dimension/WDimInputField").GetComponent<TMP_InputField>();
            hDimInputField = property.transform.Find("Dimension/HDimInputField").GetComponent<TMP_InputField>();
            lDimInputField.text = "1";
            wDimInputField.text = "1";
            hDimInputField.text = "1";
            lDimInputField.onEndEdit.AddListener(delegate { UpdateDimension(); });
            wDimInputField.onEndEdit.AddListener(delegate { UpdateDimension(); });
            hDimInputField.onEndEdit.AddListener(delegate { UpdateDimension(); });

            //Rotation Inputs
            xRotInputField = property.transform.Find("Rotation/XRotInputField").GetComponent<TMP_InputField>();
            yRotInputField = property.transform.Find("Rotation/YRotInputField").GetComponent<TMP_InputField>();
            zRotInputField = property.transform.Find("Rotation/ZRotInputField").GetComponent<TMP_InputField>();
            xRotInputField.text = "0";
            yRotInputField.text = "0";
            zRotInputField.text = "0";
            xRotInputField.onEndEdit.AddListener(delegate { UpdateRotation(); });
            yRotInputField.onEndEdit.AddListener(delegate { UpdateRotation(); });
            zRotInputField.onEndEdit.AddListener(delegate { UpdateRotation(); });

            //Scale Inputs
            // xScaleInputField = property.transform.Find("Scale/XScaleInputField").GetComponent<TMP_InputField>();
            // yScaleInputField = property.transform.Find("Scale/YScaleInputField").GetComponent<TMP_InputField>();
            // zScaleInputField = property.transform.Find("Scale/ZScaleInputField").GetComponent<TMP_InputField>();
            // xScaleInputField.text = "1";
            // yScaleInputField.text = "1";
            // zScaleInputField.text = "1";
            // xScaleInputField.onEndEdit.AddListener(delegate { UpdateScale(); });
            // yScaleInputField.onEndEdit.AddListener(delegate { UpdateScale(); });
            // zScaleInputField.onEndEdit.AddListener(delegate { UpdateScale(); });     

            deleteButton = property.transform.Find("DeleteButton").GetComponent<Button>();
            deleteButton.onClick.AddListener(() => DeleteTab()); 
        }
        else
        {
            Debug.LogError("Prefab 'CubeProperty' not found in Resources folder.");
        }
    }

    void Update()
    {
        
    }

    void OnMouseDown()
    {
        //Get current time
        float timeSinceLastClick = Time.time - lastClickTime;

        //Check if it is doubleclick
        if (timeSinceLastClick <= doubleClickTimeLimit)
            OpenTab();
        
        //Update lastClickTime
        lastClickTime = Time.time;
    }

    private void OpenTab()
    {
        property.SetActive(true);
    }

    private void CloseTab()
    {
        property.SetActive(false);
    }

    private void DeleteTab()
    {
        Destroy(property);
        Destroy(gameObject);
    }

    private void UpdatePosition()
    {
        float xPos, yPos, zPos;

        if (float.TryParse(xPosInputField.text, out xPos) && float.TryParse(yPosInputField.text, out yPos) && float.TryParse(zPosInputField.text, out zPos))
        {
            transform.position = new Vector3(xPos, yPos, zPos);
        }
    }

    private void UpdateDimension()
    {
        float lDim, wDim, hDim;

        if (float.TryParse(lDimInputField.text, out lDim) && float.TryParse(wDimInputField.text, out wDim) && float.TryParse(hDimInputField.text, out hDim))
        {
            transform.localScale = new Vector3(
            transform.localScale.x * (lDim / transform.localScale.x),
            transform.localScale.y * (hDim / transform.localScale.y),
            transform.localScale.z * (wDim / transform.localScale.z)
            );
        }
    }

    private void UpdateRotation()
    {
        float xRot, yRot, zRot;

        if (float.TryParse(xRotInputField.text, out xRot) && float.TryParse(yRotInputField.text, out yRot) && float.TryParse(zRotInputField.text, out zRot))
        {
            transform.rotation = Quaternion.Euler(xRot, yRot, zRot);
        }
    }

    // private void UpdateScale()
    // {
    //     float xScale, yScale, zScale;

    //     if (float.TryParse(xScaleInputField.text, out xScale) && float.TryParse(yScaleInputField.text, out yScale) && float.TryParse(zScaleInputField.text, out zScale))
    //     {
    //         transform.localScale = new Vector3(xScale, yScale, zScale);
    //     }
    // }
}
