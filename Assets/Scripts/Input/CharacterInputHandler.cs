using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _moveInputVector = Vector2.zero;
    private Vector2 _viewInputVector = Vector2.zero;
    private bool _isJumped;

    private CharacterMovementHandler _movementHandler;

    private void Awake()
    {
        _movementHandler = GetComponent<CharacterMovementHandler>();
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

        _movementHandler.SetViewInputVector(_viewInputVector);

        _moveInputVector.x = Input.GetAxisRaw("Horizontal");
        _moveInputVector.y = Input.GetAxisRaw("Vertical");

        _isJumped = Input.GetButtonDown("Jump");
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.rotationInput = _viewInputVector.x;
        networkInputData.movementInput = _moveInputVector;
        networkInputData.isJumpPressed = _isJumped;

        return networkInputData;
    }
}
