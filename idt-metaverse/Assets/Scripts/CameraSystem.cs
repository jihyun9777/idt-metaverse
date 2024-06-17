using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private bool useDragPan = false;
    [SerializeField] private float fieldOfViewMax = 100;
    [SerializeField] private float fieldOfViewMin = 10;

    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private float targetFieldOfView = 50;

    private float mouseX;
    private float mouseY;

    private void Update()
    {
        HandleCameraMovement();
        HandleCameraRotation();
        if(useEdgeScrolling) HandleCameraEdgeScrolling();
        if(useDragPan) HandleCameraDragPan();
        HandleCameraZoom();
        HandleCameraVertical();
    }

    private void HandleCameraMovement() 
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) inputDir.z = +1f;
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) inputDir.z = -1f;
        if(Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if(Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation() 
    {
        float rotateDir = 0f;
        float rotateSpeed = 200f;
        

        if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.LeftArrow)) rotateDir = +1f;
        if(Input.GetMouseButton(1) || Input.GetKey(KeyCode.RightArrow)) rotateDir = -1f;

        
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraEdgeScrolling()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

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
        Vector3 inputDir = new Vector3(0, 0, 0);

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

    //수정 필요
    private void HandleCameraVertical()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if(Input.GetKey(KeyCode.UpArrow)) inputDir.y = +1f;
        if(Input.GetKey(KeyCode.DownArrow)) inputDir.y = -1f;

        Vector3 moveDir = transform.up * inputDir.y;

        float rotateSpeed = 50f;
        //transform.rotate += moveDir * moveSpeed * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, 0, inputDir.y * rotateSpeed * Time.deltaTime);

        //transform.RotateAround(Target.position, Vector3.up, 20 * Time.deltaTime);
    }
}
