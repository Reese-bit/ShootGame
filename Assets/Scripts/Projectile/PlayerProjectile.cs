using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;

    protected virtual void Awake() 
    {
        trail = GetComponentInChildren<TrailRenderer>();    

        if(moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right,moveDirection);
        }
    }

    private void OnDisable() 
    {
        trail.Clear();    
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        PlayerEnergy.instance.Obtain(PlayerEnergy.PERCENT);
    }
}
