using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    private ChangeDetector _changeDetector;
    [Networked]
    public byte HP { get; set; }

    [Networked]
    public bool IsDead { get; set; }

    private const byte _startHP = 5;
    //private bool _isInitialized = false;

    public Color ColorOnHit;
    public Image OnHitImage;

    public GameObject PlayerModel;
    public GameObject DeathGameObjectPrefab;

    private HitboxRoot hitboxRoot;
    private CharacterMovementHandler characterMovementHandler;
    private NetworkInGameMessege _networkInGameMessege;
    private NetworkPlayer _networkPlayer;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponent<HitboxRoot>();
        _networkInGameMessege = GetComponent<NetworkInGameMessege>();
        _networkPlayer = GetComponent<NetworkPlayer>();
    }

    void Start()
    {
        HP = _startHP;
        IsDead = false;
        //_isInitialized = true;
    }

    private IEnumerator OnHitEffect()
    {
        if (Object.HasInputAuthority)
        {
            OnHitImage.color = ColorOnHit;
        }
        yield return new WaitForSeconds(0.1f);
        if (Object.HasInputAuthority && !IsDead)
        {
            OnHitImage.color = new Color(0, 0, 0, 0);
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(HP):
                    StartCoroutine(OnHitEffect());
                    break;
                case nameof(IsDead):
                    if(IsDead) StartCoroutine(OnDeadCoroutine());
                    break;
            }
        }
    }

    public void OnTakeDamage(string damageDealer)
    {
        if(IsDead) return;

        HP -= 1;

        if(HP <= 0)
        {
            //_networkInGameMessege.SendInGameRPCMessege(damageDealer, $"Killed <b>{_networkPlayer.Nickname.ToString()}</b>");
            _networkPlayer.RPC_SendMessege(damageDealer, $"Killed <b>{_networkPlayer.Nickname.ToString()}</b>");

            StartCoroutine(OnDeadCoroutine());
            IsDead = true;
        }
    }

    private IEnumerator OnDeadCoroutine()
    {
        OnDeath();
        yield return new WaitForSeconds(3.0f);
        OnRevive();
    }

    public void OnDeath()
    {
        PlayerModel.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        GameObject deathParticle = Instantiate(DeathGameObjectPrefab, transform.position, Quaternion.identity);
        Destroy(deathParticle, 2.0f);
    }
    public void OnRevive()
    {
        if (Object.HasInputAuthority)
        {
            OnHitImage.color = new Color(0, 0, 0, 0);
        }

        characterMovementHandler.RequestRespawn();
        PlayerModel.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawn()
    {
        HP = _startHP;
        IsDead = false;
    }
}
