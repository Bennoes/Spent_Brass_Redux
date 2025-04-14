using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SingleStackMag : MagazineBase
{

    private new readonly float bulletSizeMod = 5f;

    protected override void PopulatePositionArray()
    {
        //get initial position from rect
        //generate array of ammo size
        int arraySize = Weapon.WeaponData.maxAmmo;
        if(AmmoPositions == null || AmmoPositions.Length != arraySize) AmmoPositions = new Vector2[arraySize];

        //need bullet world size 
        Rect bulletRect = Weapon.WeaponData.AmmoIcon.rect;            
        worldBulletSize = new Vector2(bulletRect.width, bulletRect.height) * bulletSizeMod;

        SetUpMagPanel();

        for (int i = 0; i < AmmoPositions.Length; i++)
        {
            //Debug.Log("print " + i);
            if (i == 0)
            {
                AmmoPositions[i] = initialPosition;
                continue;
            }

            float yOffset = worldBulletSize.x * i;
            AmmoPositions[i] = new Vector2(initialPosition.x, initialPosition.y - yOffset);

        }
    }

    protected override void MoveBulletsToNextPosition()
    {
        for (int i = 0;i < AmmoList.Count; i++)
        {
            LeanTween.moveLocalY(AmmoList[i], AmmoPositions[i].y, Weapon.WeaponData.cycleRate);
        }
    }

    
}
