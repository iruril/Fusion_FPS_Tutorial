using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    [SerializeField] private Transform _camTarget;
    [SerializeField] private float _camSensetivity = 50f;
    private NetworkCharacterController _networkCharacterController;
    private Camera _localCamera;
    public Vector3 _mySpawnPoint { get; set; }

    private Vector2 _viewInput = Vector2.zero;
    private float _camRotationX = 0;

    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterController>();
        _localCamera = Camera.main;
        _localCamera.transform.parent = _camTarget;
        _localCamera.transform.localPosition = Vector3.zero + transform.forward * -5.0f + transform.right * 2.0f;
        _localCamera.transform.localRotation = this.transform.rotation;
    }

    void Start()
    {

    }

    void Update()
    {
        _camRotationX += _viewInput.y * Time.deltaTime * _camSensetivity;
        _camRotationX = Mathf.Clamp(_camRotationX, -60, 60);

        _camTarget.transform.localRotation = Quaternion.Euler(_camRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            _networkCharacterController.Rotate(inputData.rotationInput * _camSensetivity);

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

    public void SetViewInputVector(Vector2 inputAxisData)
    {
        _viewInput = inputAxisData;
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
