using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUtils
{

    public void SetShootAndEjectionPoints(WeaponSO weaponData, Vector2 aimDirection, GameObject actualShootPoint, GameObject ejectionPoint, Animator weaponAnimator)
    {
        if (weaponData.shootPoints.Length < 8)
        {
            Debug.LogWarning("Weapon data is missing or incomplete.");
            return;
        }

        aimDirection.Normalize();

        float highestDot = -Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < weaponData.shootPoints.Length; i++)
        {
            Vector2 candidate = weaponData.shootPoints[i].normalized;
            float dot = Vector2.Dot(candidate, aimDirection);
            if (dot > highestDot)
            {
                highestDot = dot;
                closestIndex = i;
            }
        }

        actualShootPoint.transform.localPosition = weaponData.shootPoints[closestIndex];
        ejectionPoint.transform.localPosition = weaponData.ejectionPoints[closestIndex];

        Vector2 animDirection = weaponData.shootPoints[closestIndex].normalized;
        weaponAnimator.SetFloat("Horizontal", animDirection.x);
        weaponAnimator.SetFloat("Vertical", animDirection.y);
    }


}
