using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    private CharacterInputHandler characterInputHandler;
    private Dictionary<int, NetworkPlayer> mapTokenIDWithPlayer = new Dictionary<int, NetworkPlayer>();

    private int GetplayerToken(NetworkRunner runner, PlayerRef player)
    {
        if(runner.LocalPlayer == player)
        {
            return ConnectionTokenUtils.HashToken(GameManager.Instance.GetConnectionToken());
        }
        else
        {
            var token = runner.GetPlayerConnectionToken(player);

            if(token != null)
            {
                return ConnectionTokenUtils.HashToken(token);
            }
        }
        Debug.LogError("GetPlayerToken has returned invalid token!");
        return 0;
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer player)
    {
        mapTokenIDWithPlayer.Add(token, player);
    }

    public void OnHostMigrationCleanUP()
    {
        foreach(var item in mapTokenIDWithPlayer)
        {
            NetworkObject objectInMap = item.Value.GetComponent<NetworkObject>();

            if (objectInMap.InputAuthority.IsNone)
            {
                int key = item.Key;
                objectInMap.Runner.Despawn(objectInMap);
                mapTokenIDWithPlayer.Remove(key);
            }
        }
    }

    #region Fusion Network Callbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(characterInputHandler == null && NetworkPlayer.LocalPlayer != null)
        {
            characterInputHandler = NetworkPlayer.LocalPlayer.GetComponent<CharacterInputHandler>();
        }

        if(characterInputHandler != null) 
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            int token = GetplayerToken(runner, player);

            if(mapTokenIDWithPlayer.TryGetValue(token, out var networkPlayer))
            {
                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);
                networkPlayer.Spawned();
            }
            else
            {
                Debug.Log("[플레이어 입장] : 우리는 서버입니다. 유저를 생성합니다.");
                Vector3 spawnPoint = Utils.GetRandom2X2PositionByVector3(transform.position);
                NetworkPlayer netPlayerObject = runner.Spawn(playerPrefab, spawnPoint, Quaternion.identity, player);
                if (netPlayerObject.TryGetComponent<CharacterMovementHandler>(out var movementHandler))
                {
                    movementHandler._mySpawnPoint = transform.position;
                }

                netPlayerObject.token = token;
                mapTokenIDWithPlayer[token] = netPlayerObject;
            }
        }
        else
        {
            Debug.Log("[플레이어 입장]");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion

    void Start()
    {
        
    }
}