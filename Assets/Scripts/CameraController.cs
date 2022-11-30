using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private const float MIN_FOLLOW_Y_OFFSET = 2.0f;
    private const float MAX_FOLLOW_Y_OFFSET = 12.0f;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        float moveSpeed = 10.0f;
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();
        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        float rotationSpeed = 100.0f;
        Vector3 rotationVector = new Vector3(0, 0, 0);
        
        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void ZoomCamera()
    {
        float zoomSpeed = 5.0f;

        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
