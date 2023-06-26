using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] private AudioData missileSFX;
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private int maxMissileAmount = 3;
    [SerializeField] private float coolDownTime = 1f;

    private bool isReady = true;

    private int currentMissileAmount;

    private void Awake()
    {
        currentMissileAmount = maxMissileAmount;
    }

    private void Start()
    {
        MissileDisplay.UpdateAmountText(currentMissileAmount);
    }

    public void PickUp()
    {
        currentMissileAmount++;
        MissileDisplay.UpdateAmountText(currentMissileAmount);

        if(currentMissileAmount == 1)
        {
            MissileDisplay.UpdateCoolDownImage(0f);
            isReady = true;
        }
    }

    public void Launch(Transform mizzleTransform)
    {
        if(currentMissileAmount == 0 || !isReady) return; 
        // TODO : Add SFX && UI VFX

        isReady = false;
        PoolManager.Release(missilePrefab, mizzleTransform.position);
        
        AudioManager.Instance.PlayRandomSFX(missileSFX);

        currentMissileAmount--;
        MissileDisplay.UpdateAmountText(currentMissileAmount);

        if (currentMissileAmount == 0)
        {
            MissileDisplay.UpdateCoolDownImage(1f);
        }
        else
        {
            StartCoroutine(nameof(CoolDownCoroutine));
        }
    }

    IEnumerator CoolDownCoroutine()
    {
        var coolDownValue = coolDownTime;

        while (coolDownValue > 0f)
        {
            MissileDisplay.UpdateCoolDownImage(coolDownValue / coolDownTime);
            coolDownValue = Mathf.Max(coolDownValue - Time.deltaTime, 0);
            //coolDownValue = Mathf.Clamp(coolDownValue, 0f, coolDownValue - Time.deltaTime);

            yield return null;
        }

        isReady = true;
    }
}
