using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyWeaponControl : EnemyPartControl
{

    public WeaponSO enemyWeapon;
    public Animator animator;
    [HideInInspector] public EnemyBaseControl enemyBaseControl;

    //[HideInInspector] public EnemyParts arms = EnemyParts.ARMS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", enemyBaseControl.travelDirection.x);
            animator.SetFloat("Vertical", enemyBaseControl.travelDirection.y);

        }

        
    }
}
