using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RocketHandler : NetworkBehaviour
{
    public GameObject ExplosionVFXObject;
    public Transform DetonatePosition;
    public LayerMask CollisionLayers;

    private TickTimer _maxLiveDuration = TickTimer.None;
    private int _rocketSpeed = 20;

    private List<LagCompensatedHit> _onExplosionHits = new List<LagCompensatedHit>();

    private PlayerRef _playerWhoFired;
    private string _playerNameWhoFired;
    private NetworkObject _netObjWhoFired;

    private NetworkObject _myNetworkObject;

    public void Fire(PlayerRef playerWhoFired, NetworkObject netObjWhoFired, string playerNameWhoFired)
    {
        _playerWhoFired = playerWhoFired;
        _netObjWhoFired = netObjWhoFired;
        _playerNameWhoFired = playerNameWhoFired;

        _myNetworkObject = GetComponent<NetworkObject>();
        _maxLiveDuration = TickTimer.CreateFromSeconds(Runner, 10);
    }

    public override void FixedUpdateNetwork()
    {
        transform.position += transform.forward * Runner.DeltaTime * _rocketSpeed;

        if (Object.HasStateAuthority)
        {
            if (_maxLiveDuration.Expired(Runner))
            {
                Runner.Despawn(_myNetworkObject);
                return;
            }

            int hitCount = Runner.LagCompensation.OverlapSphere(
                DetonatePosition.position,
                0.5f,
                _playerWhoFired,
                _onExplosionHits,
                CollisionLayers,
                HitOptions.IncludePhysX
                );

            bool isValidHit = false;
            if(hitCount > 0) isValidHit = true;

            for(int i = 0; i < hitCount; i++)
            {
                if (_onExplosionHits[i].Hitbox != null)
                {
                    if (_onExplosionHits[i].Hitbox.Root.GetBehaviour<NetworkObject>() == _netObjWhoFired)
                    {
                        isValidHit = false;
                        break;
                    }
                }
            }

            if (isValidHit)
            {
                hitCount = Runner.LagCompensation.OverlapSphere(
                DetonatePosition.position,
                4.0f,
                _playerWhoFired,
                _onExplosionHits,
                CollisionLayers,
                HitOptions.None
                );

                for(int i = 0; i < hitCount; i++)
                {
                    HPHandler hPHandler = _onExplosionHits[i].Hitbox.transform.root.GetComponent<HPHandler>();

                    if(hPHandler != null)
                    {
                        hPHandler.OnTakeDamage(_playerNameWhoFired, 100);
                    }
                }

                Runner.Despawn(_myNetworkObject);
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GameObject vfx = Instantiate(ExplosionVFXObject, DetonatePosition.position, Quaternion.identity);
        Destroy(vfx, 3f);
    }
}
