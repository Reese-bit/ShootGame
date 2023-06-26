using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class MissileDisplay : MonoBehaviour
{
    private static Text amountText;
    private static Image coolDownImage;

    private void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        coolDownImage = transform.Find("CoolDown Image").GetComponent<Image>();
    }

    public static void UpdateAmountText(int amount)
    {
        amountText.text = amount.ToString();
    }

    public static void UpdateCoolDownImage(float fillAmount)
    {
        coolDownImage.fillAmount = fillAmount;
    }
}
