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
    public NetworkString<_16> Nickname { get; set; }

    private bool _isPublicJoinMessegeSent = false;
    private NetworkInGameMessege _networkInGameMessege;

    public LocalCameraHandler LocalCameraHandler;
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
            Camera.main.gameObject.SetActive(false);

            RPC_SetNickname(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("로컬 플레이어 생성!");
        }
        else
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = false;

            Camera camera = GetComponentInChildren<Camera>();
            camera.enabled = false;

            AudioListener listener = GetComponentInChildren<AudioListener>();
            listener.enabled = false;

            localUI.SetActive(false);

            Debug.Log("원격 플레이어 생성!");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"Player_{Object.Id}";
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _networkInGameMessege.SendInGameRPCMessege(Nickname.ToString(), "Has Left!");
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
