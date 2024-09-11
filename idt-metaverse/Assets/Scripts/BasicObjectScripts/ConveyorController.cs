using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConveyorController : MonoBehaviour
{
    private BaseSceneController baseSceneController;
    private BoxCollider boxCollider;

    public float doubleClickTimeLimit = 0.3f; 
    private float lastClickTime = -1f; 

    private GameObject property;
    private Vector3 offset;

    private bool positiveDir = true;
    private float speed = 1f;

    //For collision detection
    private Vector3 boxSize;

    Color activeColor = new Color32(0xD1, 0xD1, 0xD1, 0xFF); 
    Color defaultColor = new Color32(0x51, 0x51, 0x51, 0xFF);

    #region Button Variables

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

    //Direction
    private Button positiveButton;
    private Button negativeButton;

    //Speed
    private TMP_InputField speedInputField;

    private Button deleteButton;

    #endregion

    #region Unity Methods

    void Start()
    {
        baseSceneController = FindObjectOfType<BaseSceneController>();
        boxCollider = GetComponent<BoxCollider>();
        boxSize = new Vector3(boxCollider.size.x, 0.1f, boxCollider.size.z);
        Debug.Log(boxSize);

        GameObject conveyorProperty = Resources.Load<GameObject>("Conveyors/" + "ConveyorProperty");

        if (conveyorProperty != null)
        {
            property = Instantiate(conveyorProperty, transform.position, Quaternion.identity);

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

            //Direction Inputs
            positiveButton = property.transform.Find("Direction/PositiveButton").GetComponent<Button>();
            negativeButton = property.transform.Find("Direction/NegativeButton").GetComponent<Button>();
            positiveButton.onClick.AddListener(() => UpdateDirection(positiveButton, true));
            negativeButton.onClick.AddListener(() => UpdateDirection(negativeButton, false));

            //Speed Inputs
            speedInputField = property.transform.Find("Speed/SpeedInputField").GetComponent<TMP_InputField>();
            speedInputField.text = "1";
            speedInputField.onEndEdit.AddListener(text => float.TryParse(text, out speed)); 

            deleteButton = property.transform.Find("DeleteButton").GetComponent<Button>();
            deleteButton.onClick.AddListener(() => DeleteTab()); 
        }
        else
        {
            Debug.LogError("Prefab 'conveyorProperty' not found in Resources folder.");
        }
    }

    void Update()
    {
        //If playMode
        if (baseSceneController.playMode && !baseSceneController.pauseMode)
        {
            Vector3 direction = positiveDir ? transform.right : -transform.right;
            Vector3 boxOrigin = transform.position + new Vector3(0, boxCollider.size.y - 2f, 0);

            RaycastHit[] hits = Physics.BoxCastAll(boxOrigin, boxSize / 2, Vector3.up, Quaternion.identity, 5f);

            foreach (RaycastHit hit in hits)
            {
                Transform obj = hit.transform;

                //움질일 물체 여기 추가
                if (obj.CompareTag("Feed"))
                {
                    Collider objCollider = obj.GetComponent<Collider>();
                    float objectBottomY = objCollider.bounds.min.y;
                    float conveyorTopY = transform.position.y + boxCollider.bounds.extents.y;

                    //Move the object only if its entire collider is above the conveyor
                    if (objectBottomY >= conveyorTopY)
                    {
                        Vector3 localPosition = transform.InverseTransformPoint(obj.position);
                        float conveyorHalfLength = boxCollider.size.x / 2f;

                        //Move the object if it hasn't reached the end of the conveyor
                        if ((positiveDir && localPosition.x < conveyorHalfLength) || (!positiveDir && localPosition.x > -conveyorHalfLength))
                        {
                            obj.Translate(speed * direction * Time.deltaTime);
                        }
                    }
                }
            }
        }
        //If pauseMode
        else if (baseSceneController.pauseMode) {}
        else if (!baseSceneController.playMode) {}
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 boxOrigin = transform.position + new Vector3(0, boxCollider.size.y - 2f, 0);
        Vector3 direction = positiveDir ? transform.right : -transform.right;

        RaycastHit hit;
        if (Physics.BoxCast(boxOrigin, boxSize/2, Vector3.up, out hit, Quaternion.identity, 5f))
        {
            Gizmos.DrawRay(boxOrigin, Vector3.up * hit.distance);
            Gizmos.DrawWireCube(boxOrigin + Vector3.up * hit.distance, boxSize);
        }
        else
        {
            Gizmos.DrawRay(boxOrigin, Vector3.up * 10f);
            Gizmos.DrawWireCube(boxOrigin + Vector3.up * 10f, boxSize);
        }
    }

    #endregion

    #region Mouse Interaction

    void OnMouseDown()
    {
        //Get current time
        float timeSinceLastClick = Time.time - lastClickTime;
        //Check if it is doubleclick
        if (timeSinceLastClick <= doubleClickTimeLimit)
            OpenTab();
        //Update lastClickTime
        lastClickTime = Time.time;

        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = GetMouseWorldPosition() + offset;
        transform.position = newPosition;

        //Update input fields
        xPosInputField.text = newPosition.x.ToString();
        yPosInputField.text = newPosition.y.ToString();
        zPosInputField.text = newPosition.z.ToString();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    #endregion

    #region Tab Control

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

    #endregion

    #region Update Variables

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

            boxSize = new Vector3(boxSize.x * lDim, boxSize.y, boxSize.z * wDim);
        }
        Debug.Log(boxSize);
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

    private void UpdateDirection(Button clickedButton, bool dir)
    {
        positiveDir = dir;

        Button otherButton = clickedButton == positiveButton ? negativeButton : positiveButton;

        ColorBlock clickedColorBlock = clickedButton.colors;
        ColorBlock otherColorBlock = otherButton.colors;

        //Change clicked button to activeColor
        clickedColorBlock.normalColor = activeColor;
        clickedColorBlock.highlightedColor = activeColor;
        clickedColorBlock.pressedColor = activeColor;
        clickedColorBlock.selectedColor = activeColor;
        clickedColorBlock.disabledColor = activeColor;

        //Change other button to defaultColor
        otherColorBlock.normalColor = defaultColor;
        otherColorBlock.highlightedColor = defaultColor;
        otherColorBlock.pressedColor = defaultColor;
        otherColorBlock.selectedColor = defaultColor;
        otherColorBlock.disabledColor = defaultColor;

        clickedButton.colors = clickedColorBlock;
        otherButton.colors = otherColorBlock;
    }

    #endregion


}
