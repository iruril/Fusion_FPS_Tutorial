using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer LocalPlayer { get; set; }
    public Transform PlayerBody;

    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
            Utils.SerRenderLayerInChildren(PlayerBody, LayerMask.NameToLayer("LocalPlayerModel"));
            Camera.main.gameObject.SetActive(false);

            Debug.Log("로컬 플레이어 생성!");
        }
        else
        {
            Camera camera = GetComponentInChildren<Camera>();
            camera.enabled = false;

            AudioListener listener = GetComponentInChildren<AudioListener>();
            listener.enabled = false;

            Debug.Log("원격 플레이어 생성!");
        }

        transform.name = $"Player_{Object.Id}";
    }
    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
