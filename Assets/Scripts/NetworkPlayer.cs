using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer LocalPlayer { get; set; }


    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            LocalPlayer = this;
            Debug.Log("로컬 플레이어 생성!");
        }
        else
        {
            Debug.Log("원격 플레이어 생성!");
        }
    }
    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
