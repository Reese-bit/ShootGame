using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("----MOVE----")]
    [SerializeField]float moveSpeed = 2f;
    [SerializeField]float moveRotationAngle = 25f;

    [Header("----FIRE----")]
    [SerializeField] protected GameObject[] enemyProjectile;
    [SerializeField] protected AudioData[] enemyProjectileLaunchSFX;
    [SerializeField] protected Transform enemyFirePos;
    [SerializeField] protected  ParticleSystem muzzleVFX;
    [SerializeField] protected float minFireInterval;
    [SerializeField] protected float maxFireInterval;
    
    protected float paddingX;
    protected float paddingY;
    protected Vector3 targetPosition;

    private WaitForFixedUpdate waitForFixedUpdate;

    protected virtual void Awake()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    protected virtual void OnEnable() 
    {
        StartCoroutine(nameof(RandomlyEnemyMovingCoroutine));
        StartCoroutine(nameof(RandomlyEnemyFireCoroutine));    
    }

    protected virtual void OnDisable() 
    {
        StopAllCoroutines();
    }

    IEnumerator RandomlyEnemyMovingCoroutine()
    {
        transform.position = Viewport.instance.RandomlyEnemySpawnPosition(paddingX,paddingY);

        targetPosition = Viewport.instance.RandomEnemyRightHalfPosition(paddingX,paddingY);

        while(gameObject.activeSelf)
        {
            //if has not arrived to the targetPosition
            //keep moving to the targetPosition
            //if arrived
            //set a new targetPosition
            if(Vector3.Distance(transform.position,targetPosition) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position,targetPosition,moveSpeed * Time.fixedDeltaTime);

                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle,Vector3.right);
            }
            else
            {
                targetPosition = Viewport.instance.RandomEnemyRightHalfPosition(paddingX,paddingY);
            }

            yield return waitForFixedUpdate;
        }

    }

    protected virtual IEnumerator RandomlyEnemyFireCoroutine()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval,maxFireInterval));

            if (GameManager.GameState == GameState.GameOver)
            {
                // StopCoroutine
                yield break;
            }

            foreach(var projectile in enemyProjectile)
            {
                PoolManager.Release(projectile,enemyFirePos.position);
            }
            
            AudioManager.Instance.PlayRandomSFX(enemyProjectileLaunchSFX);
            muzzleVFX.Play();
        }
    }
}
