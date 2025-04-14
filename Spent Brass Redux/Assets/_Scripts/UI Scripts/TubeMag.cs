using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class TubeMag : MagazineBase
{

    private new readonly float bulletSizeMod = 5f;
 
    protected override void PopulatePositionArray()
    {
        int arraySize = Weapon.WeaponData.maxAmmo;
        if (AmmoPositions == null || AmmoPositions.Length != arraySize) AmmoPositions = new Vector2[arraySize];

        //need bullet world size 
        Rect bulletRect = Weapon.WeaponData.AmmoIcon.rect;
        worldBulletSize = new Vector2(bulletRect.width, bulletRect.height) * bulletSizeMod;

        SetUpMagPanel();

        //might need to add an offset between shells
        for (int i = 0; i < AmmoPositions.Length; i++)
        {
            float yAdjustFromInitial = (worldBulletSize.y - worldBulletSize.x) * (Weapon.WeaponData.maxAmmo - i);


            Vector2 shellPosition = new Vector2(initialPosition.x, initialPosition.y - yAdjustFromInitial);
            AmmoPositions[i] = shellPosition;
        }
    }

    protected override void MoveBulletsToNextPosition()
    {
        for (int i = 0; i < AmmoList.Count; i++)
        {
            LeanTween.moveLocalY(AmmoList[i], AmmoPositions[i].y, Weapon.WeaponData.cycleRate);
        }
    }

    public override void ReloadOneRound()
    {
       

       GameObject thisIcon = GameObject.Instantiate(AmmoIconHolder, MagazinePanel.transform);

        Vector2 loadPosition = new(AmmoPositions[0].x, AmmoPositions[0].y - worldBulletSize.y);

        AmmoList.Insert(0, thisIcon);

        thisIcon.transform.localPosition = loadPosition;

        Image iconImage = thisIcon.GetComponent<Image>();
        iconImage.sprite = Weapon.WeaponData.AmmoIcon;
        RectTransform holderRectTransform = thisIcon.GetComponent<RectTransform>();
        
        holderRectTransform.sizeDelta = worldBulletSize;

        MoveBulletsToNextPosition();
        
    }
}
