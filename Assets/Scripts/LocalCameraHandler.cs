using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform CamAnchoredPoint;
    private Camera localCamera;
    private NetworkCharacterController _networkCharacterController;

    private Vector2 viewInput = Vector2.zero;

    private float camRotationX;
    private float camRotationY;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        _networkCharacterController = GetComponentInParent<NetworkCharacterController>();
    }

    void Start()
    {
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    void LateUpdate()
    {
        if (CamAnchoredPoint == null) return;
        if (!localCamera.enabled) return;

        localCamera.transform.position = CamAnchoredPoint.position;

        camRotationX += viewInput.y * Time.deltaTime * _networkCharacterController.viewUpDownRotationSpeed;
        camRotationX = Mathf.Clamp(camRotationX, -90, 90);

        camRotationY += viewInput.x * Time.deltaTime * _networkCharacterController.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(camRotationX, camRotationY, 0);
    }

    public void SetViewIputVector(Vector2 input)
    {
        this.viewInput = input;
    }
}
