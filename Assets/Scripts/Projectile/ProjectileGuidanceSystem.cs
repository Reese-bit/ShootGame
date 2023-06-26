using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileGuidanceSystem : MonoBehaviour
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private float minBllisticAngle = 50f;
    [SerializeField] private float maxBallisticAngle = 75f;

    private float randomBallisticAngle;

    private Vector3 targetDirection;
    
    public IEnumerator HomingCoroutine(GameObject target)
    {
        randomBallisticAngle = Random.Range(minBllisticAngle, maxBallisticAngle);
        
        while (gameObject.activeSelf)
        {
            if (target.activeSelf)
            {
                // move to target
                targetDirection = target.transform.position - transform.position;

                //rotate to target
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg,Vector3.forward);
                transform.rotation *= Quaternion.Euler(0f,0f,randomBallisticAngle);
                
                //move projectile
                projectile.Move();
            }
            else
            {
                // move to its precious direction
                projectile.Move();
            }

            yield return null;
        }
    }
}
