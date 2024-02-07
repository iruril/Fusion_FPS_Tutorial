using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    private CharacterInputHandler characterInputHandler;
    private SessionListUIHandler sessionListUIHandler;
    private Dictionary<int, NetworkPlayer> mapTokenIDWithPlayer;
    public List<NetworkPlayer> networkPlayers = new();

    private void Awake()
    {
        mapTokenIDWithPlayer = new Dictionary<int, NetworkPlayer>();
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }

    private int GetPlayerToken(NetworkRunner runner, PlayerRef player)
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
        networkPlayers.Add(player);
        Debug.Log("Connection Token : " + token + ", " + player.Nickname + " Added.");
    }

    public void OnHostMigrationCleanUP()
    {
        foreach(var item in mapTokenIDWithPlayer)
        {
            NetworkObject objectInMap = item.Value.GetComponent<NetworkObject>();

            if (objectInMap.InputAuthority.IsNone)
            {
                objectInMap.Runner.Despawn(objectInMap);
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
        Debug.Log("OnHostMigration");
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
            int token = GetPlayerToken(runner, player);

            if(mapTokenIDWithPlayer.TryGetValue(token, out NetworkPlayer networkPlayer))
            {
                Debug.Log($"Found old connection token for token {token}. Assigning controls to that player.");
                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);
                networkPlayer.Spawned();
            }
            else
            {
                Debug.Log($"[Player Entered] : We are Server. Generates User by token {token}.");
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
            Debug.Log("[Player Entered]");
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
        if (sessionListUIHandler == null) return;
        if(sessionList.Count == 0)
        {
            Debug.Log("No Sessions Found!");
            sessionListUIHandler.OnFoundSessionFailed();
        }
        else
        {
            sessionListUIHandler.ClearList();
            foreach(var item in sessionList)
            {
                sessionListUIHandler.AddToList(item);
                Debug.Log($"Session Named {item.Name} / PlayerCount {item.PlayerCount} Found, And Added on List.");
            }
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
}