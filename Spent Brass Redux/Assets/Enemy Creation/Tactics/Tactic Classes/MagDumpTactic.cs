using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagDumpTactic : TacticBase
{
    public new readonly TacticTitle tacticTitle = TacticTitle.MagDump;

    EnemyWeaponControl enemyWeapon;

    PathFinderController pathFinderController;

    EnemySensor sensor;



    public MagDumpTactic(EnemyBaseControl enemyBase, PathFinderController pathFinder) : base(enemyBase, pathFinder)
    {
        
        pathFinderController = pathFinder;

    }

    public override void InitialiseTactic()
    {
        enemyWeapon = EnemyBase.GetComponentInChildren<EnemyWeaponControl>();
        sensor = EnemyBase.GetComponent<EnemySensor>();

        Vector2 playerPos = sensor.GetPlayerTransform().position;

        TacticContext context = new TacticContext(null, null, playerPos);

        if (enemyWeapon == null) Debug.Log("enemy weapon is null");
        

        for (int i = 0; i < enemyWeapon.currentAmmoCount; i++)
        {
            ShootOneRound step = new ShootOneRound(EnemyBase,context);
            //tactical steps is the name of the list in the base object
            tacticalSteps.Enqueue(step);

        }
        //for each remain ammo, add shoot one round step to list
        //target is the vector the player was seen at.
        //keep panic firing until ammo is spent


        Debug.Log("MAG DUMP");
    }

    public override bool ExecuteStep()
    {
        if (tacticalSteps.Count == 0) return true;

        var step = tacticalSteps.Peek();

        if (step.Execute())
        {
            tacticalSteps.Dequeue();
        }


        return tacticalSteps.Count == 0;
    }
}
   
    

