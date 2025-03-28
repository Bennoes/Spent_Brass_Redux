using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Library", menuName = "Enemy Creation/Enemy Weapons")]
public class EnemyWeaponsLIbrary : ScriptableObject
{
    public List<WeaponSO> enemyWeapons = new List<WeaponSO>();
}
