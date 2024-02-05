using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    private ChangeDetector _changeDetector;

    public static NetworkPlayer LocalPlayer { get; set; }
    public Transform PlayerBody;
    public TextMeshProUGUI PlayerNickname;
    
    [Networked]
    public int token { get; set; }
    [Networked]
    public NetworkString<_16> Nickname { get; set; }

    private bool _isPublicJoinMessegeSent = false;
    private NetworkInGameMessege _networkInGameMessege;

    public LocalCameraHandler MyLocalCameraHandler;
    public GameObject localUI;

    private void Awake()
    {
        _networkInGameMessege = GetComponent<NetworkInGameMessege>();
    }

    void Start()
    {

    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
            Utils.SerRenderLayerInChildren(PlayerBody, LayerMask.NameToLayer("LocalPlayerModel"));

            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;

            AudioListener listener = GetComponentInChildren<AudioListener>(true);
            listener.enabled = true;

            MyLocalCameraHandler.LocalCamera.enabled = true;
            MyLocalCameraHandler.LocalCamera.transform.parent = null;
            localUI.SetActive(true);

            RPC_SetNickname(GameManager.Instance.playerNickname);

            Debug.Log("로컬 플레이어 생성!");
        }
        else
        {
            MyLocalCameraHandler.LocalCamera.enabled = false;
            localUI.SetActive(false);

            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = false;

            Camera camera = GetComponentInChildren<Camera>();
            camera.enabled = false;

            AudioListener listener = GetComponentInChildren<AudioListener>();
            listener.enabled = false;

            Debug.Log("원격 플레이어 생성!");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"Player_{Object.Id}";
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _networkInGameMessege.SendInGameRPCMessege(Nickname.ToString(), "Has Left!");
    }

    private void OnDestroy()
    {
        if(MyLocalCameraHandler != null)
        {
            Destroy(MyLocalCameraHandler.gameObject);
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Nickname):
                    OnNicknameChanged();
                    break;
            }
        }

        if(PlayerNickname.text != Nickname.ToString())
        {
            OnNicknameChanged();
        }
    }

    private void OnNicknameChanged()
    {
        PlayerNickname.text = Nickname.ToString();
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetNickname(string nickName, RpcInfo rpcInfo = default)
    {
        this.Nickname = nickName;

        if (!_isPublicJoinMessegeSent)
        {
            _networkInGameMessege.SendInGameRPCMessege(nickName, "Has Joined!");
            _isPublicJoinMessegeSent = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessege(string nickName, string messege, RpcInfo rpcInfo = default)
    {
        _networkInGameMessege.SendInGameRPCMessege(nickName, messege);
    }
}
