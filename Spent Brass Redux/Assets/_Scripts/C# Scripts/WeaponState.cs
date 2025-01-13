using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponState
{
    //Data holder for specific weapons

    public int currentAmmoCount;
    public int reloadsRemaining;

    //potential buffs
    public bool extendedMag;
    public bool jungleMag;
    public bool supressor;
    public bool primaryWeapon;

    public WeaponState(int currentAmmo, int reloadsRemaining, bool primaryWeapon)
    {
        this.currentAmmoCount = currentAmmo;
        this.reloadsRemaining = reloadsRemaining;
        
        this.primaryWeapon = primaryWeapon;
    }

}
