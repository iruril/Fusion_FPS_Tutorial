using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _moveInputVector = Vector2.zero;
    private Vector2 _viewInputVector = Vector2.zero;
    private bool _isJumped;

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

    void Update()
    {
        _viewInputVector.x = Input.GetAxis("Mouse X");
        _viewInputVector.y = -Input.GetAxis("Mouse Y");

        _localCameraHandler.SetViewIputVector(_viewInputVector);

        _moveInputVector.x = Input.GetAxisRaw("Horizontal");
        _moveInputVector.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
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
