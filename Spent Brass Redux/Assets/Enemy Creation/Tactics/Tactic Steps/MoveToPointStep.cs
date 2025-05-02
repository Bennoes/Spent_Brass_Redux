using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPointStep : ITacticalStep
{
    public EnemyBaseControl EnemyBase { get; set; }

    //context holds info that step might need
    public TacticContext Context { get; set; }

    private EnemyWeaponControl enemyWeaponControl;


    public void InitialiseStep(TacticContext context)
    {

    }

    // Start is called before the first frame update
    public bool Execute()
    {
        if(enemyWeaponControl == null)
        {
            enemyWeaponControl = EnemyBase.GetComponentInChildren<EnemyWeaponControl>();
        }


        if(Context.DestinationPosition == null) return true;

        //get enemy position now and target
        Vector2 currentPosition = EnemyBase.gameObject.transform.position;
        Vector2 target = (Vector2)Context.DestinationPosition;

        //check if that matches target position
        float distance = Vector2.Distance(currentPosition,target);
        float precision = 0.1f;

        if(Mathf.Abs(distance) < precision) return true;

        EnemyBase.transform.position = Vector2.MoveTowards(currentPosition, target, EnemyBase.speed * Time.deltaTime);

        enemyWeaponControl.RotateShootPoints(EnemyBase.travelDirection);
 
        return false;
    }

    public MoveToPointStep(EnemyBaseControl enemyBase, TacticContext context)
    {
        EnemyBase = enemyBase;
        Context = context;
        
    }
}
