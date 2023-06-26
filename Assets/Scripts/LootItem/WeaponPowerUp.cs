using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPowerUp : LootItem
{
    [SerializeField]AudioData fullPowerPickUpSFX;
    [SerializeField]int fullPowerScoreBouns = 200;

    protected override void PickUp()
    {
        if(player.IsFullHealth)
        {
            pickUpSFX = fullPowerPickUpSFX;
            lootMessage.text = $"SCORE + {fullPowerScoreBouns}";
            ScoreManager.Instance.AddScore(fullPowerScoreBouns);
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"POWER UP!";
            player.PowerUp();
        }

        base.PickUp();
    }
}
