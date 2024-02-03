using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterController _networkCharacterController;
    private Camera _localCamera;
    public Vector3 _mySpawnPoint { get; set; }

    private Vector2 _viewInput = Vector2.zero;

    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterController>();
        _localCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            transform.forward = inputData.aimForwardVector;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.eulerAngles.z);

            Vector3 moveDirection = transform.forward * inputData.movementInput.y + transform.right * inputData.movementInput.x; ;
            moveDirection.Normalize();

            _networkCharacterController.Move(moveDirection);

            if (inputData.isJumpPressed)
            {
                _networkCharacterController.Jump();
            }

            CheckFallRespawn();
        }
    }

    private void CheckFallRespawn()
    {
        if(this.transform.position.y < 10)
        {
            Vector3 spawnPoint = Utils.GetRandom2X2PositionByVector3(_mySpawnPoint);
            _networkCharacterController.Teleport(spawnPoint);
            _networkCharacterController.Velocity = Vector3.zero;
        }
    } 
}
