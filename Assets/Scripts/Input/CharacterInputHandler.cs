using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : SimulationBehaviour
{
    [SerializeField][Range(0, 100)] private float _mouseRotateSpeed = 1.0f;
    [SerializeField][Range(0, 100)] private float _rotateSpeedOnGamepad = 1.0f;

    private Vector2 _moveInputVector = Vector2.zero;
    private Vector2 _viewInputVector = Vector2.zero;
    private bool _isJumped;
    private bool _isFired;

    private LocalCameraHandler _localCameraHandler;

    private void Awake()
    {
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnCamRotation(InputAction.CallbackContext context)
    {
        Vector2 inputValues = context.ReadValue<Vector2>();
        float sensitivity;
        if (context.control.device is Mouse)
        {
            sensitivity = _mouseRotateSpeed * Time.deltaTime;

            _viewInputVector.x = inputValues.x * sensitivity;
            _viewInputVector.y = inputValues.y * sensitivity;
        }
        else
        {
            sensitivity = _rotateSpeedOnGamepad * Time.deltaTime;

            _viewInputVector.x = inputValues.x * sensitivity;
            _viewInputVector.y = inputValues.y * sensitivity;
        }
        _localCameraHandler.SetViewIputVector(_viewInputVector);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isJumped = true;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isFired = true;
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.aimForwardVector = _localCameraHandler.transform.forward;
        networkInputData.movementInput = _moveInputVector;
        networkInputData.isJumpPressed = _isJumped;
        networkInputData.isFirePressed = _isFired;

        _isJumped = false;
        _isFired = false;

        return networkInputData;
    }
}
