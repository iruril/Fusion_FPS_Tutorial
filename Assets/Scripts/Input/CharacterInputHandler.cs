using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : SimulationBehaviour
{
    [SerializeField][Range(0, 100)] private float _mouseRotateSpeed = 1.0f;
    [SerializeField][Range(0, 100)] private float _rotateSpeedOnGamepad = 1.0f;

    public Vector2 MoveInputVector = Vector2.zero;
    private Vector2 _viewInputVector = Vector2.zero;
    private bool _isJumped;
    private bool _isFired;
    private bool _isGrenade;

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
        if (Object != null && !Object.HasInputAuthority) return;
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
        if (Object != null && !Object.HasInputAuthority) return;
        MoveInputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (Object != null && !Object.HasInputAuthority) return;
        if (context.performed)
        {
            _isJumped = true;
        }
        else
        {
            _isJumped = false;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (Object != null && !Object.HasInputAuthority) return;
        if (context.performed)
        {
            _isFired = true;
        }
        else
        {
            _isFired = false;
        }
    }

    public void OnGrenade(InputAction.CallbackContext context)
    {
        if (Object != null && !Object.HasInputAuthority) return;
        if (context.performed)
        {
            _isGrenade = true;
        }
        else
        {
            _isGrenade = false;
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.aimForwardVector = _localCameraHandler.transform.forward;
        networkInputData.movementInput = MoveInputVector;
        networkInputData.isJumpPressed = _isJumped;
        networkInputData.isFirePressed = _isFired;
        networkInputData.isGrenadePressed = _isGrenade;

        return networkInputData;
    }
}
