using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : EnemyController
{
    [SerializeField] private float continuousFireDuration = 1.5f;

    [Header("----PLAYER DETECTION----")] 
    [SerializeField] private Transform playerDetectionTransform;
    [SerializeField] private Vector3 playerDetectionSize;
    [SerializeField] private LayerMask playerLayer;

    [Header("----BEAM----")] 
    [SerializeField] private float beamCoolDownTime = 12f;
    [SerializeField] private AudioData beamChargingSFX;
    [SerializeField] private AudioData beamLaunchSFX;

    private bool isBeamReady;
    private Animator animator;
    private Transform playerTransform;
    private int launchBeamID = Animator.StringToHash("launchBeam");

    private WaitForSeconds waitForContinuousFireInterval;
    private WaitForSeconds waitForFireInterval;
    private WaitForSeconds waitForBeamCoolDownTime;

    private List<GameObject> magazine;
    private AudioData launchSFX;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        
        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitForBeamCoolDownTime = new WaitForSeconds(beamCoolDownTime);

        magazine = new List<GameObject>(enemyProjectile.Length);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void OnEnable()
    {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCoolDownTimeCoroutine));
        base.OnEnable();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position,playerDetectionSize);
    }

    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    void AnimatorEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    void AnimatorEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCoolDownTimeCoroutine));
        StartCoroutine(nameof(RandomlyEnemyFireCoroutine));
    }

    void LoadProjectiles()
    {
        magazine.Clear();
        
        // playerIsFrontOfBoss
        if (Physics2D.OverlapBox(playerDetectionTransform.position,playerDetectionSize,0f,playerLayer))
        {
            // Launch projectile 1
            magazine.Add(enemyProjectile[0]);
            launchSFX = enemyProjectileLaunchSFX[0];
        }
        else
        {
            // Launch projectile 2 or 3
            if (Random.value < 0.5f)
            {
                magazine.Add(enemyProjectile[1]);
                launchSFX = enemyProjectileLaunchSFX[1];
            }
            else
            {
                for (int i = 2; i < enemyProjectile.Length; i++)
                {
                    magazine.Add(enemyProjectile[i]);
                }

                launchSFX = enemyProjectileLaunchSFX[2];
            }
        }
    }

    protected override IEnumerator RandomlyEnemyFireCoroutine()
    {
        while (isActiveAndEnabled)
        {
            if(GameManager.GameState == GameState.GameOver) yield break;
            
            if (isBeamReady)
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));
                
                yield break;
            }
            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    IEnumerator ContinuousFireCoroutine()
    {
        LoadProjectiles();
        muzzleVFX.Play();
        
        float continuousFireTimer = 0f;

        while (continuousFireTimer < continuousFireDuration)
        {
            foreach (var projectile in magazine)
            {
                PoolManager.Release(projectile, enemyFirePos.position);
            }

            continuousFireTimer += minFireInterval;
            AudioManager.Instance.PlayRandomSFX(launchSFX);

            yield return waitForContinuousFireInterval;
        }
        
        muzzleVFX.Stop();
    }

    IEnumerator BeamCoolDownTimeCoroutine()
    {
        yield return waitForBeamCoolDownTime;

        isBeamReady = true;
    }

    IEnumerator ChasingPlayerCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetPosition.x = Viewport.instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;

            yield return null;
        }
    }
}
