using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StateBar : MonoBehaviour
{
    [SerializeField]Image fillImageBack;
    [SerializeField]Image fillImageFront;
    [SerializeField]float fillSpeed = 0.1f;
    [SerializeField]float delaySpeed = 0.5f;
    [SerializeField]bool delayFill = true;

    protected float currentFillAmount;
    protected float targetFillAmount;
    private float preciousFillAmount;
    float t;

    WaitForSeconds waitForDelayFill;
    
    Coroutine bufferFillImageCoroutine;

    private void Awake() 
    {
        if (TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        waitForDelayFill = new WaitForSeconds(delaySpeed);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentHealth,float maxHealth)
    {
        currentFillAmount = currentHealth / maxHealth;
        targetFillAmount = currentFillAmount;

        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateState(float currentHealth,float maxHealth)
    {
        targetFillAmount = currentHealth / maxHealth;

        if(bufferFillImageCoroutine != null)
        {
            StopCoroutine(bufferFillImageCoroutine);
        }

        //if state reduced
        //fill image front amount = target fill amount
        //slowly reduce fill image back
        if(currentFillAmount > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferFillImageCoroutine = StartCoroutine(BufferFillCoroutine(fillImageBack));
        }

        //if state restore
        //fill image back amount = target fill amount
        //slowly restore fill image front
        if(currentFillAmount < targetFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;
            bufferFillImageCoroutine = StartCoroutine(BufferFillCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferFillCoroutine(Image image)
    {
        if(delayFill)
        {
            yield return waitForDelayFill;
        }

        preciousFillAmount = currentFillAmount;
        t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(preciousFillAmount,targetFillAmount,t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}
