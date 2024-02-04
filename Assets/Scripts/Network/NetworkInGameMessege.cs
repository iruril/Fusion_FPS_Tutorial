using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInGameMessege : MonoBehaviour
{
    InGameMessegedUIHandler _inGameMessegedUIHandler;

    public void SendInGameRPCMessege(string userNickname, string messege)
    {
        RPC_InGameMessege($"<b>{userNickname}</b> : {messege}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_InGameMessege(string messege, RpcInfo rpcInfo = default)
    {
        if(_inGameMessegedUIHandler == null)
        {
            _inGameMessegedUIHandler = NetworkPlayer.LocalPlayer.LocalCameraHandler.GetComponentInChildren<InGameMessegedUIHandler>();
        }

        if (_inGameMessegedUIHandler != null)
        {
            _inGameMessegedUIHandler.OnGameMessegeRecieved(messege);
        }
    }
}
