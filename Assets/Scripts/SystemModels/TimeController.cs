using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Resources;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0f, 1f)] private float bulletTimeScale = 0.1f;
     
    private float t;
    private float defaultFixedDeltaTime;
    private float timeScaleBeforePause;

    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        Time.timeScale = timeScaleBeforePause;
    }

    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    public void BulletTime(float inDuration,float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration,outDuration));
    }
    
    public void BulletTime(float inDuration,float keepDuration,float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutCoroutine(inDuration,keepDuration,outDuration));
    }

    public void SlowIn(float inDuration)
    {
        StartCoroutine(SlowInCoroutine(inDuration));
    }

    public void SlowOut(float outDuration)
    {
        StartCoroutine(SlowOutCoroutine(outDuration));
    }
    
    private IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;

        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }
    
    private IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;

        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInKeepAndOutCoroutine(float inDuration, float keepDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepDuration);

        StartCoroutine(SlowOutCoroutine(outDuration));
    }
}
