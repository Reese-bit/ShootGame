using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWaveUI : MonoBehaviour
{
    #region FIELDS

    //the time to play aniamtion
    [SerializeField] private float animationTime = 1f;

    [Header("---- LINE MOVE ----")] 
    [SerializeField]
    private Vector2 lineTopStartPosition = new Vector2(-1250f, 140f);
    [SerializeField] private Vector2 lineTopTargetPosition = new Vector2(0f, 140f);
    [SerializeField] private Vector2 lineBottomStartPosition = new Vector2(1250f, 0f);
    [SerializeField]Vector2 lineBottomTargetPosition = Vector2.zero;

    [Header("---- TEXT SCALE ----")] 
    [SerializeField]
    private Vector2 waveTextStartScale = new Vector2(1f, 0f);
    [SerializeField] private Vector2 waveTextTargetScale = Vector2.one;

    private RectTransform lineTop;
    private RectTransform lineBottom;
    private RectTransform waveText;

    //the time to show UI
    private WaitForSeconds waitStayTime;

    #endregion

    #region UNITY EVENT FUNCTIONS

    private void Awake()
    {
        if (TryGetComponent<Animator>(out Animator animator))
        {
            if (animator.isActiveAndEnabled)
            {
                Destroy(this);
            }
        }

        //show UI time = one wave time - play animation time * 2
        waitStayTime = new WaitForSeconds(EnemyManager.instance.TimeBetweenWaves - animationTime * 2f);

        lineTop = transform.Find("Line Top").GetComponent<RectTransform>();
        lineBottom = transform.Find("Line Bottom").GetComponent<RectTransform>();
        waveText = transform.Find("Wave Text").GetComponent<RectTransform>();

        lineTop.localPosition = lineTopStartPosition;
        lineBottom.localPosition = lineBottomStartPosition;
        waveText.localPosition = waveTextStartScale;
    }

    private void OnEnable()
    {
        StartCoroutine(LineMoveCoroutine(lineTop,lineTopTargetPosition,lineTopStartPosition));
        StartCoroutine(LineMoveCoroutine(lineBottom, lineBottomTargetPosition, lineBottomStartPosition));
        StartCoroutine(TextScaleCoroutine(waveText, waveTextTargetScale, waveTextStartScale));
    }

    #endregion

    #region LINE MOVE

    IEnumerator LineMoveCoroutine(RectTransform rect, Vector2 targetPosition, Vector2 startPosition)
    {
        yield return StartCoroutine(UIMoveCoroutine(rect,targetPosition));
        yield return waitStayTime;
        yield return StartCoroutine(UIMoveCoroutine(rect,startPosition));
    }

    IEnumerator UIMoveCoroutine(RectTransform rect, Vector2 position)
    {
        float t = 0f;
        Vector2 localPosition = rect.localPosition;

        while (t < 1f)
        {
            t += Time.deltaTime / animationTime;
            rect.localPosition = Vector2.Lerp(localPosition, position, t);

            yield return null;
        }
    }

    #endregion

    #region TEXT SCALE

    IEnumerator TextScaleCoroutine(RectTransform rect, Vector2 targetScale, Vector2 startScale)
    {
        yield return StartCoroutine(UIScaleCoroutine(rect,targetScale));
        yield return waitStayTime;
        yield return StartCoroutine(UIScaleCoroutine(rect,startScale));
    }

    IEnumerator UIScaleCoroutine(RectTransform rect, Vector2 scale)
    {
        float t = 0f;
        Vector2 localScale = rect.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime / animationTime;
            rect.localScale = Vector2.Lerp(localScale, scale, t);

            yield return null;
        }
    }

    #endregion
}
