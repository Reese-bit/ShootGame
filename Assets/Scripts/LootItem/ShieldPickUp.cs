using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : LootItem
{
    [SerializeField]AudioData fullHealthPickUPSFX;
    [SerializeField]int fullHealthScoreBonus = 200;
    [SerializeField]float shieldBouns = 20f;

    protected override void PickUp()
    {
        if(player.IsFullHealth)
        {
            pickUpSFX = fullHealthPickUPSFX;
            lootMessage.text = $"SCORE + {fullHealthScoreBonus}";
            ScoreManager.Instance.AddScore(fullHealthScoreBonus);
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"SHIELD + {shieldBouns}";
            player.RestoreHealth(shieldBouns);
        }

        base.PickUp();
    }
}
