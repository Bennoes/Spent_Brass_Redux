using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon Creation")]

public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public TextField weaponDescription;

    public Vector2[] shootPoints;
    public Vector2[] ejectionPoints;

    public WeaponType weaponType;
    public MagazineType MagazineType;

    public bool primaryWeapon = false;


    //weapon attributes
    public bool FullAuto = false;
    public bool sequencialReload = false;
    public bool piercingAmmo = false;   

    public int maxAmmo;

    public int shotsPerPull = 1;
    public float cycleRate;
    public float projectileSpeed = 20;
    public float reloadTime;
    public float damage;
    public float rangeNominal;
    public float rangeFuzz;

    public float maxSpreadFuzz;
    public float spreadNominalFuzz;
    public float recoilRate;
    public float recoilRecoveryRate;

    public float tracerLength;

    //this needs to be handed to the projectile which will alter its bahavior
    public AnimationCurve ProjectileSpeedDistance;
    public AnimationCurve DamageOverDistance;

    public RuntimeAnimatorController animationController;

    public GameObject SpentAmmo;

    public Sprite weaponIcon;
    public Sprite AmmoIcon;

    

}


public enum WeaponType
{
    PISTOL,
    ASSUALT_RIFLE,
    SHOTGUN,
    MACHINE_PISTOL,
    LMG,
    BATTLE_RIFLE,
    DMR
}

public enum MagazineType
{
    SINGLE_STACK,
    DOUBLE_STACK,
    TUBE,
    DRUM,
    C_MAG,
    P90
}