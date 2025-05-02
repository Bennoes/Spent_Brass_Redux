using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBack : TacticBase
{

    public new readonly TacticTitle tacticTitle = TacticTitle.FallBack;
    public FallBack(EnemyBaseControl enemyBase, PathFinderController pathFinder) : base(enemyBase, pathFinder)
    {


    }

    public override void InitialiseTactic()
    {
        Debug.Log("FALL BACK!");
        TacticContext context = new(null, null,null);

        WaitFor step = new WaitFor();
        step.InitialiseStep(context);

        tacticalSteps.Enqueue(step);
    }
}

