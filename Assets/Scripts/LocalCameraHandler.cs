using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform CamAnchoredPoint;
    public Camera LocalCamera;
    private NetworkCharacterController _networkCharacterController;

    private Vector2 viewInput = Vector2.zero;

    private float camRotationX;
    private float camRotationY;

    private void Awake()
    {
        LocalCamera = GetComponent<Camera>();
        _networkCharacterController = GetComponentInParent<NetworkCharacterController>();
    }

    void Start()
    {
        camRotationX = GameManager.Instance.cameraViewRotation.x;
        camRotationY = GameManager.Instance.cameraViewRotation.y;
    }

    void LateUpdate()
    {
        if (CamAnchoredPoint == null) return;
        if (!LocalCamera.enabled) return;

        LocalCamera.transform.position = CamAnchoredPoint.position;

        camRotationX += viewInput.y * Time.deltaTime * _networkCharacterController.viewUpDownRotationSpeed;
        camRotationX = Mathf.Clamp(camRotationX, -90, 90);

        camRotationY += viewInput.x * Time.deltaTime * _networkCharacterController.rotationSpeed;

        LocalCamera.transform.rotation = Quaternion.Euler(camRotationX, camRotationY, 0);
    }

    public void SetViewIputVector(Vector2 input)
    {
        this.viewInput = input;
    }

    private void OnDestroy()
    {
        if(camRotationX != 0 && camRotationY != 0)
        {
            GameManager.Instance.cameraViewRotation.x = camRotationX;
            GameManager.Instance.cameraViewRotation.y = camRotationY;
        }
    }
}
