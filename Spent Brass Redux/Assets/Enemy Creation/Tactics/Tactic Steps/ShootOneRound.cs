using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class ShootOneRound : ITacticalStep
{
    public EnemyBaseControl EnemyBase { get; set; }

    //context holds info that step might need
    public TacticContext Context { get; set; }

    private EnemyWeaponControl enemyWeapon;

    public ShootOneRound(EnemyBaseControl enemyBase, TacticContext context)
    {
        EnemyBase = enemyBase;
        Context = context;

        enemyWeapon = enemyBase.GetComponentInChildren<EnemyWeaponControl>();
        if (enemyWeapon == null) Debug.Log("enemyWeapon not assigned in tactic step");

    }

    public void InitialiseStep(TacticContext context)
    {

    }

    public bool Execute()
    {
        if(enemyWeapon.cycleTimer > 0) return false;        //weapon not ready to fire yet
        if(Context.Target == null) return false;

        Vector2 actualTarget = (Vector2)Context.Target;

        enemyWeapon.ShootAt(actualTarget);
        //get ref to enemy weapon via enemyBase
        //run weapon fire logic 
        

        return true;
    }
}
