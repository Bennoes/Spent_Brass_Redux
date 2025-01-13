using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager
{
    public WeaponSO WeaponData { get; set; }
    public WeaponState State { get; set; }

    public WeaponManager(WeaponSO weaponData, WeaponState state)
    {
        this.WeaponData = weaponData;
        this.State = state;
    }


}
