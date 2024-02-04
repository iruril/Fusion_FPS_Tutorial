using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    private ChangeDetector _changeDetector;
    private HPHandler _handler;

    [Networked]
    public bool IsFiring {  get; set; }
    [Networked]
    public Vector3 HitPosition { get; set; }

    public ParticleSystem FireParticle;
    public ParticleSystem HitParticle;
    public Transform aimPoint;
    public LayerMask collisionLayers;
    private LineRenderer lineRenderer;

    private float _weaponRange = 100f;
    private bool _isFiring = false;
    private float _rateOfFireDelay = 0.08f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        _handler = GetComponent<HPHandler>();
        lineRenderer.enabled = false;
        HitParticle.transform.parent = GameObject.FindWithTag("ParticlePool").transform;   
    }

    private void OnDestroy()
    {
        if(HitParticle != null) Destroy(HitParticle.gameObject);
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(IsFiring):
                    OnFireRemote();
                    break;
                case nameof(HitPosition):
                    OnHitRemote();
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_handler.IsDead) return;

        if(GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFirePressed)
            {
                ShootBullet(networkInputData.aimForwardVector);
            }
            else
            {
                _isFiring = false;
            }
        }
    }

    private void ShootBullet(Vector3 aimForwardVector)
    {
        if (!_isFiring) StartCoroutine(Shoot(aimForwardVector));
    }

    private IEnumerator Shoot(Vector3 aimForwardVector)
    {
        _isFiring = true;
        Fire(aimForwardVector);
        yield return new WaitForSeconds(_rateOfFireDelay);
        _isFiring = false;
    }

    private void Fire(Vector3 aimForwardVector)
    {
        IsFiring = true;
        float hitDistance = _weaponRange;
        bool isHitOnPlayer = false;
        if (Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, _weaponRange,
            Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX))
        {
            hitDistance = hitInfo.Distance;
        }

        HitPosition = aimPoint.position + aimForwardVector * hitDistance;

        if (hitInfo.Hitbox != null)
        {
            if (Object.HasStateAuthority && hitInfo.Hitbox.transform.root != this.transform)
            {
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage();
            }
            isHitOnPlayer = true;
        }
        else if(hitInfo.Collider != null)
        {

        }

        if (isHitOnPlayer)
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 0.2f);
        }
        else
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 0.2f);
        }

        StartCoroutine(FireEffect());
        StartCoroutine(FireLine());

        HitParticle.transform.position = hitInfo.Point;
        HitParticle.transform.LookAt(aimPoint.position);
        HitParticle.Play();
    }

    private IEnumerator FireEffect()
    {
        FireParticle.Play();
        yield return new WaitForSeconds(_rateOfFireDelay);
        IsFiring = false;
    }

    private IEnumerator FireLine()
    {
        lineRenderer.SetPosition(0, FireParticle.transform.position);
        lineRenderer.SetPosition(1, HitPosition);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }

    private void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
        {
            FireParticle.transform.LookAt(HitPosition);
            FireParticle.Play();
            StartCoroutine(FireLine());
        }
    }

    private void OnHitRemote()
    {
        if (!Object.HasInputAuthority)
        {
            HitParticle.transform.position = HitPosition;
            HitParticle.transform.LookAt(aimPoint.position);
            HitParticle.Play();
        }
    }
}
