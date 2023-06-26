using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] private EnergyBar energyBar;
    [SerializeField] private float overDrivenInterval = 0.07f;

    private bool available = true;

    public const int MAX = 100;
    public const int PERCENT = 1;
    private int energy;

    private WaitForSeconds waitForOverDrivenInterval;

    protected override void Awake()
    {
        base.Awake();
        waitForOverDrivenInterval = new WaitForSeconds(overDrivenInterval);
    }

    private void OnEnable()
    {
        PlayerOverDriven.on += PlayerOverDrivenOn;
        PlayerOverDriven.off += PlayerOverDrivenOff;
    }

    private void OnDisable()
    {
        PlayerOverDriven.on -= PlayerOverDrivenOn;
        PlayerOverDriven.off -= PlayerOverDrivenOff;
    }

    private void Start()
    {
        energyBar.Initialize(energy,MAX);
        Obtain(MAX);
    }

    public void Obtain(int value)
    {
        if(energy == MAX || !available || !gameObject.activeSelf)
            return;
        energy += value;
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateState(energy,MAX);
    }

    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateState(energy,MAX);

        if (energy == 0 && !available)
        {
            PlayerOverDriven.off.Invoke();
        }
    }

    public bool IsEnough(int value)
    {
        return energy >= value;
    }

    void PlayerOverDrivenOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    void PlayerOverDrivenOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverDrivenInterval;
            
            //use 1% of max energy with waitForOverDrivenInterval
            //means the sum time = waitForOverDrivenInterval * 100
            
            Use(PERCENT);
        }
    }
}
