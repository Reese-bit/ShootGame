using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private GameObject hitVFX;
    
    protected virtual void OnCollisionStay2D(Collision2D other) 
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(damage);
            
            PoolManager.Release(hitVFX,other.GetContact(0).point,Quaternion.LookRotation(other.GetContact(0).normal));
        }
    }
}
