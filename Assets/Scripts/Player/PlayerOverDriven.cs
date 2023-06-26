using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOverDriven : MonoBehaviour
{
    public static UnityAction on = delegate {};
    public static UnityAction off = delegate {};

    [SerializeField] private GameObject triggerVFX;
    [SerializeField] private GameObject engineVFXNormal;
    [SerializeField] private GameObject engineVFXOverDriven;

    [SerializeField] private AudioData onSFX;
    [SerializeField] private AudioData offSFX;

    private void Awake()
    {
        on += On;
        off += Off;
    }

    private void OnDestroy()
    {
        on -= On;
        off -= Off;
    }

    void On()
    {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverDriven.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    void Off()
    {
        engineVFXNormal.SetActive(true);
        engineVFXOverDriven.SetActive(false);
        AudioManager.Instance.PlayRandomSFX(offSFX);
    }
}
