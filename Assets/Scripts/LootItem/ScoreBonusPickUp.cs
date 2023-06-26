using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBonusPickUp : LootItem
{
    [SerializeField]int scoreBouns;

    protected override void PickUp()
    {
        ScoreManager.Instance.AddScore(scoreBouns);
        base.PickUp();
    }
}
