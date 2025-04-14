using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DoubleStackMag : MagazineBase
{


    private new readonly float bulletSizeMod = 5f;

    protected override void PopulatePositionArray()
    {
        //get initial position from rect
        //generate array of ammo size
        int arraySize = Weapon.WeaponData.maxAmmo;
        if (AmmoPositions == null || AmmoPositions.Length != arraySize) AmmoPositions = new Vector2[arraySize];

        //need bullet world size 
        Rect bulletRect = Weapon.WeaponData.AmmoIcon.rect;
        worldBulletSize = new Vector2(bulletRect.width, bulletRect.height) * bulletSizeMod;

        SetUpMagPanel();

        for (int i = 0; i < AmmoPositions.Length; i++)
        {
            if (i == 0)
            {
                AmmoPositions[i] = initialPosition;
                continue;
            }

            //int x = 0;
            //if i is even make x = -1
            int x = (i % 2 == 0) ? -1 : 1;
            float xAdjustment = (worldBulletSize.x / 2);
            float yAdjustment = ((Mathf.Sqrt(3) * worldBulletSize.x) / 2);

            Vector2 nextPos;
            if (i == 1)
            {
                nextPos = new Vector2(initialPosition.x + xAdjustment, initialPosition.y - yAdjustment);
                AmmoPositions[i] = nextPos;
                continue;
            }
            //need to grt the position from the previous bullet rather than initial position
            Vector2 lastPos = AmmoPositions[i - 1];
            nextPos = new Vector2(lastPos.x + yAdjustment * x, lastPos.y - xAdjustment);

            AmmoPositions[i] = nextPos;

        }

    }



    protected override void MoveBulletsToNextPosition()
    {
        //no 1 bullet needs to meve to pos 0. Others go to next y value
        for (int i = 0; i < AmmoList.Count; i++)
        {
            if (i == 0)
            {
                LeanTween.moveLocal(AmmoList[i], AmmoPositions[i], Weapon.WeaponData.cycleRate);
            }
            else
            {
                LeanTween.moveLocalY(AmmoList[i], AmmoPositions[i].y, Weapon.WeaponData.cycleRate);
            }
           
        }
    }

    protected override void ReSizeMagazine(RectTransform magRect)
    {
        float edgeBuffer = 100;

        float magazineWidth = worldBulletSize.x + (edgeBuffer * 2);
        float magazineHeight = (worldBulletSize.x * Weapon.WeaponData.maxAmmo /2) + worldBulletSize.x;

        magRect = MagazinePanel.GetComponent<RectTransform>();
        Vector2 oldSize = magRect.sizeDelta;
        Vector2 newSize = new(magazineWidth, magazineHeight);

        float anchorMoveY = (newSize.y - oldSize.y) / 2;
        Debug.Log(anchorMoveY);

        magRect.anchoredPosition = new(cachedAnchor.x, cachedAnchor.y + anchorMoveY);

        magRect.sizeDelta = newSize;
    }

   
}
