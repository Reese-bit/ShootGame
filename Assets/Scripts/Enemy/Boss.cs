using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyCharacter
{
    [SerializeField] private BossHealthBar bossHealthBar;

    private Canvas healthBarCanvas;

    protected override void Awake()
    {
        base.Awake();
        bossHealthBar = FindObjectOfType<BossHealthBar>();
        healthBarCanvas = bossHealthBar.GetComponentInChildren<Canvas>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        bossHealthBar.Initialize(health,maxHealth);
        healthBarCanvas.enabled = true;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }

    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        bossHealthBar.UpdateState(health,maxHealth);
    }

    protected override void SetHealth()
    {
        maxHealth += EnemyManager.instance.WaveNumber * healthFactor;
    }
}
