using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Diagnostics;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterController _networkCharacterController;
    private Animator _animator;
    private HPHandler _hPHandler;
    private NetworkPlayer _networkPlayer;

    private bool isRespawnReqeusted = false;

    [Networked]
    public Vector3 _mySpawnPoint { get; set; }

    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterController>();
        _animator = GetComponent<Animator>();
        _hPHandler = GetComponent<HPHandler>();
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

            _animator.SetFloat("XVelocity", inputData.movementInput.x);
            _animator.SetFloat("YVelocity", inputData.movementInput.y);

            _networkCharacterController.Move(moveDirection);

            if (inputData.isJumpPressed)
            {
                _networkCharacterController.Jump();
                StartCoroutine(DoJump());
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
                _networkPlayer.RPC_SendMessege(_networkPlayer.Nickname.ToString(), "Fell of the world");
                Respawn();
            }
        }
    }

    private IEnumerator DoJump()
    {
        _animator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.1f);
        _animator.ResetTrigger("Jump");
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
