using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon Creation")]

public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public TextField weaponDescription;

    public Vector2[] shootPoints;

    public WeaponType weaponType;


    //weapon attributes
    public float projectileSpeed = 20;
    public float reloadTime;
    public float damage;
    public float range;
    public float accuracy;
    public float recoil;
    public float recoilRecovery;

    

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