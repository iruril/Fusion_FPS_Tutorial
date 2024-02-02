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

    private Vector2 _viewRotation;
    private float _camRotationX = 0;

    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Object.HasInputAuthority)
        {
            _localCamera = Camera.main;
            _localCamera.transform.parent = _camTarget;
            _localCamera.transform.localPosition = Vector3.zero + transform.forward * -5.0f + transform.right * 2.0f;
            _localCamera.transform.localRotation = this.transform.rotation;
        }
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;

        _camRotationX += _viewRotation.y * Time.deltaTime * _camSensetivity;
        _camRotationX = Mathf.Clamp(_camRotationX, -60, 60);
        _camTarget.transform.localRotation = Quaternion.Euler(_camRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            _networkCharacterController.Rotate(inputData.rotationInput.x * _camSensetivity);

            _viewRotation = inputData.rotationInput;

            Vector3 moveDirection = transform.forward * inputData.movementInput.y + transform.right * inputData.movementInput.x; ;
            moveDirection.Normalize();

            _networkCharacterController.Move(5 * moveDirection * Runner.DeltaTime);

            if(inputData.buttons.IsSet(NetworkInputData.SPACE))
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
