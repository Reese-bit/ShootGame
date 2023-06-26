using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverDriven : PlayerProjectile
{
    [SerializeField] private ProjectileGuidanceSystem projectileGuidanceSystem;

    protected override void OnEnable()
    {
        SetTarget(EnemyManager.instance.randomEnemy);
        transform.rotation = Quaternion.identity;
        
        if (target == null)
        {
            base.OnEnable();
        }
        else
        {
            // Track target
            StartCoroutine(projectileGuidanceSystem.HomingCoroutine(target));
        }
    }
}
