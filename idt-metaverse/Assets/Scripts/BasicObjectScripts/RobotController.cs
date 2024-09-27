using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotController : MonoBehaviour
{
    private BaseSceneController baseSceneController;

    public float doubleClickTimeLimit = 0.3f; 
    private float lastClickTime = -1f; 

    private GameObject property;
    private Vector3 offset;

    private GameObject drawnCircle;
    private float circleRadius = 200f;
    private List<Quaternion> initialChildRotations = new List<Quaternion>();
    private Vector3 axis;

    private Vector3 targetPosition;
    private float speed = 1f;
    
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

    //Pick up
    private Toggle pickToggle;

    //Place
    private TMP_InputField xPlaceInputField;
    private TMP_InputField yPlaceInputField;
    private TMP_InputField zPlaceInputField;

    //Speed
    private TMP_InputField speedInputField;

    private Button deleteButton;

    #endregion

    #region Unity Methods

    void Start()
    {
        baseSceneController = FindObjectOfType<BaseSceneController>();

        axis = transform.TransformDirection(Vector3.forward);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            initialChildRotations.Add(child.rotation);
        }

        //Tab Start
        GameObject robotProperty = Resources.Load<GameObject>("Robots/" + "RobotProperty");

        if (robotProperty != null)
        {
            property = Instantiate(robotProperty, transform.position, Quaternion.identity);

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

            //Pick up
            pickToggle = property.transform.Find("PickUp/Toggle").GetComponent<Toggle>();
            pickToggle.onValueChanged.AddListener(OnPickToggleChanged);

            //Place Inputs 
            xRotInputField = property.transform.Find("Place/XPlaceInputField").GetComponent<TMP_InputField>();
            yRotInputField = property.transform.Find("Place/YPlaceInputField").GetComponent<TMP_InputField>();
            zRotInputField = property.transform.Find("Place/ZPlaceInputField").GetComponent<TMP_InputField>();
            xRotInputField.onEndEdit.AddListener(text => float.TryParse(text, out targetPosition.x));
            yRotInputField.onEndEdit.AddListener(text => float.TryParse(text, out targetPosition.y));
            zRotInputField.onEndEdit.AddListener(text => float.TryParse(text, out targetPosition.z));

            //Speed Inputs
            speedInputField = property.transform.Find("Speed/SpeedInputField").GetComponent<TMP_InputField>();
            speedInputField.text = "1";
            speedInputField.onEndEdit.AddListener(text => float.TryParse(text, out speed)); 

            deleteButton = property.transform.Find("DeleteButton").GetComponent<Button>();
            deleteButton.onClick.AddListener(() => DeleteTab()); 
        }
        else
        {
            Debug.LogError("Prefab 'robotProperty' not found in Resources folder.");
        }
    }

    void Update()
    {
        //If playMode
        if (baseSceneController.playMode && !baseSceneController.pauseMode)
        {
            //Check if there is Feed in PickUp Boundary
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, circleRadius);
            Transform closestFeedObject = null;
            float closestDistance = Mathf.Infinity;

            //Find closest Feed
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Feed"))
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFeedObject = hitCollider.transform;
                    }
                }
            }

            bool isM21Rotating = false;
            bool isM33Rotating = false;
            Quaternion targetRotation = Quaternion.identity;

            if (closestFeedObject != null)
            {
                //M12 rotation
                for (int i = 11; i < transform.childCount; i++)  
                {
                    Transform child = transform.GetChild(i);

                    Vector3 directionToFeed = closestFeedObject.position - child.position;
                    //y-axis rotation
                    directionToFeed.y = 0;

                    targetRotation = Quaternion.LookRotation(directionToFeed);
                    targetRotation *= Quaternion.Euler(0, 90, 0);

                    //Calculate the angle difference
                    float angleDifference = Quaternion.Angle(child.rotation, targetRotation);

                    //If the angle difference is greater than 180 degrees, invert the target rotation
                    if (angleDifference > 180f)
                    {
                        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + 180f, targetRotation.eulerAngles.z);
                    }

                    child.rotation = Quaternion.Slerp(child.rotation, targetRotation, Time.deltaTime * speed);
                }

                //Check if M12 rotation is finished
                if (Quaternion.Angle(transform.GetChild(11).rotation, targetRotation) < 1f)
                {
                    axis = targetRotation * Vector3.forward;
                    isM21Rotating = true;
                }

                //M33 rotation
                // if (isM21Rotating)
                // {
                //     Transform m35 = transform.Find("M35");

                //     for (int i = 32; i < transform.childCount; i++)
                //     {
                //         Transform child = transform.GetChild(i);
                        
                //         child.RotateAround(m35.position, Vector3.forward, 45f * Time.deltaTime);
                //     }

                //     //Check if M33 rotation is finished
                //     if (Quaternion.Angle(transform.GetChild(32).rotation, targetRotation) < 1f)
                //     {
                //         isM33Rotating = true;
                //     }
                // }

                // //M21 rotation
                // if (isM33Rotating)
                // {
                //     for (int i = 20; i < transform.childCount; i++)
                //     {
                //         Transform child = transform.GetChild(i);
                //         Vector3 directionToFeed = closestFeedObject.position - child.position;
                        
                //         // Calculate the target angle based on height
                //         float targetAngle = Mathf.Atan2(directionToFeed.y, directionToFeed.x) * Mathf.Rad2Deg;
                        
                //         // Create the rotation for the lower arm
                //         targetRotation = Quaternion.Euler(targetAngle, 0, 0);
                        
                //         // Smoothly rotate the lower arm
                //         child.rotation = Quaternion.Slerp(child.rotation, targetRotation, Time.deltaTime * speed);
                //     }

                //     //Check if M21 rotation is finished
                //     if (Quaternion.Angle(transform.GetChild(32).rotation, targetRotation) < 1f)
                //     {
                //         isM33Rotating = false;
                //     }
                // }
            }
        }
        //If pauseMode
        else if (baseSceneController.pauseMode) {}
        //If not playMode
        else if (!baseSceneController.playMode) 
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.rotation = initialChildRotations[0];
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, axis * 200f);
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

        if (drawnCircle != null)
        {
            drawnCircle.transform.position = newPosition;  
        }
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

            if (drawnCircle != null)
            {
                drawnCircle.transform.position = transform.position; 
            }
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

    void OnPickToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (drawnCircle == null)
            {
                drawnCircle = new GameObject("DrawnCircle");
                MeshFilter meshFilter = drawnCircle.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = drawnCircle.AddComponent<MeshRenderer>();

                Material material = new Material(Shader.Find("Sprites/Default"));
                material.color = Color.green; 
                meshRenderer.material = material;

                Mesh mesh = new Mesh();
                meshFilter.mesh = mesh;

                int segments = 50;

                Vector3[] vertices = new Vector3[segments + 1];
                int[] triangles = new int[segments * 3];

                vertices[0] = Vector3.zero;

                for (int i = 0; i < segments; i++)
                {
                    float angle = (i * Mathf.PI * 2f) / segments;
                    float x = Mathf.Cos(angle) * circleRadius;
                    float z = Mathf.Sin(angle) * circleRadius;
                    vertices[i + 1] = new Vector3(x, 0, z);
                }

                for (int i = 0; i < segments - 1; i++)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }

                triangles[(segments - 1) * 3] = 0;
                triangles[(segments - 1) * 3 + 1] = segments;
                triangles[(segments - 1) * 3 + 2] = 1;

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();

                drawnCircle.transform.position = transform.position;
            }
        }
        else
        {
            Destroy(drawnCircle);
        }
    }

    #endregion

}
