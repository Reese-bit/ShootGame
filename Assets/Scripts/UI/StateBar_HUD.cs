using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateBar_HUD : StateBar
{
    [SerializeField] protected Text percentText;

    protected virtual void SetPercentText()
    {
        // transform it to a percent without 'dot'.
        // percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
        percentText.text = targetFillAmount.ToString("P0");
    }

    public override void Initialize(float currentHealth, float maxHealth)
    {
        base.Initialize(currentHealth, maxHealth);
        SetPercentText();
    }

    protected override IEnumerator BufferFillCoroutine(Image image)
    {
        SetPercentText();
        return base.BufferFillCoroutine(image);
    }
}
