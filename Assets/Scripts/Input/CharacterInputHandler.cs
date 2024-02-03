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

    private LocalCameraHandler _localCameraHandler;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        _playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //_viewInputVector.x = Input.GetAxis("Mouse X");
        //_viewInputVector.y = -Input.GetAxis("Mouse Y");

        //_localCameraHandler.SetViewIputVector(_viewInputVector);

        //_moveInputVector.x = Input.GetAxisRaw("Horizontal");
        //_moveInputVector.y = Input.GetAxisRaw("Vertical");

        //if (Input.GetButtonDown("Jump"))
        //{
        //    _isJumped = true;
        //}
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

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.aimForwardVector = _localCameraHandler.transform.forward;
        networkInputData.movementInput = _moveInputVector;
        networkInputData.isJumpPressed = _isJumped;

        _isJumped = false;

        return networkInputData;
    }
}
