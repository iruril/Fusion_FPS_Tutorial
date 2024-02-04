using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterController _networkCharacterController;
    private HPHandler _hPHandler;
    private NetworkInGameMessege _networkInGameMessege;
    private NetworkPlayer _networkPlayer;

    private bool isRespawnReqeusted = false;
    public Vector3 _mySpawnPoint { get; set; }

    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterController>();
        _hPHandler = GetComponent<HPHandler>();
        _networkInGameMessege = GetComponent<NetworkInGameMessege>();
        _networkPlayer = GetComponent<NetworkPlayer>();
    }

    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isRespawnReqeusted)
            {
                Respawn();
                return;
            }

            if (_hPHandler.IsDead) return;
        }

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
            if (Object.HasStateAuthority)
            {
                //_networkInGameMessege.SendInGameRPCMessege(_networkPlayer.Nickname.ToString(), "Fell of the world");
                _networkPlayer.RPC_SendMessege(_networkPlayer.Nickname.ToString(), "Fell of the world");
                Respawn();
            }
        }
    }

    public void RequestRespawn()
    {
        isRespawnReqeusted = true;
    }

    private void Respawn()
    {
        Vector3 spawnPoint = Utils.GetRandom2X2PositionByVector3(_mySpawnPoint);
        _networkCharacterController.Teleport(spawnPoint);
        _networkCharacterController.Velocity = Vector3.zero;

        _hPHandler.OnRespawn();

        isRespawnReqeusted = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        _networkCharacterController.Controller.enabled = isEnabled;
    }
}
