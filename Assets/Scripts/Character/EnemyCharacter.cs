using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    //the reward when killed the enemy
    [SerializeField] private int deathEnergyBouns = 3;
    [SerializeField] private int scorePoint = 100;
    [SerializeField] protected int healthFactor;

    LootSpawner lootSpawner;

    protected virtual void Awake() {
        lootSpawner = GetComponent<LootSpawner>();
    }

    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }

    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.instance.Obtain(deathEnergyBouns);
        EnemyManager.instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);
        base.Die();
    }

    protected virtual void SetHealth()
    {
        maxHealth += (int)(EnemyManager.instance.WaveNumber / 2);
    }
}
