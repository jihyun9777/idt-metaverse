using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private bool useDragPan = false;
    [SerializeField] private float fieldOfViewMax = 100;
    [SerializeField] private float fieldOfViewMin = 0;
    //[SerializeField] public TMP_InputField FileName;
    
    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private float targetFieldOfView = 50;

    private void Update()
    {
        //파일이름 쓸때 wasd로 움직이지 않게 할때
        // if (!FileName.isFocused)
        // {
        //     HandleCameraMovement();
        //     HandleCameraRotation();
        //     //HandleCameraDragRotation();

        //     if(useEdgeScrolling) HandleCameraEdgeScrolling();
        //     if(useDragPan) HandleCameraDragPan();
        //     HandleCameraZoom();
        // }

        HandleCameraMovement();
        HandleCameraRotation();
        //HandleCameraDragRotation();

        if(useEdgeScrolling) HandleCameraEdgeScrolling();
        if(useDragPan) HandleCameraDragPan();
        HandleCameraZoom();
    }

    private void HandleCameraMovement() 
    {
        Vector3 inputDir = new Vector3();

        if(Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if(Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if(Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if(Input.GetKey(KeyCode.D)) inputDir.x = +1f;
        if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) inputDir.y = +1f;
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.DownArrow)) inputDir.y = -1f;

        Vector3 moveDir = transform.right * inputDir.x + transform.up * inputDir.y + transform.forward * inputDir.z;

        float moveSpeed = 600f;
        Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;

        // Can't go lower than ground
        if(newPosition.y < 0) newPosition.y = 0;

        transform.position = newPosition;
    }

    private void HandleCameraRotation() 
    {
        float rotateSpeed = 50f;
        float rotateDir = 0f;
        
        if(Input.GetKey(KeyCode.RightArrow)) rotateDir = -1f;
        if(Input.GetKey(KeyCode.LeftArrow)) rotateDir = +1f;

        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraDragRotation()
    {
        float rotationSpeed = 50f;

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) 
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + mouseX, 0);
        }
    }

    private void HandleCameraEdgeScrolling()
    {
        Vector3 inputDir = new Vector3();

        int edgeScrollSize = 20;

        if(Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if(Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if(Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if(Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    } 

    private void HandleCameraDragPan()
    {
        Vector3 inputDir = new Vector3();

        if(Input.GetMouseButtonDown(1))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mouseScrollDelta;
        }
        if(Input.GetMouseButtonUp(1))
        {
            dragPanMoveActive = false;
        }
        if(dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = (Vector2) Input.mousePosition - lastMousePosition;
            
            inputDir.x = mouseMovementDelta.x;
            inputDir.z = mouseMovementDelta.y;

            lastMousePosition = Input.mousePosition;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraZoom()
    {
        if(Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 5;
        }
        if(Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 5;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

        float zoomSpeed = 20f;
        cinemachineVirtualCamera.m_Lens.FieldOfView = 
            Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
    }
}
