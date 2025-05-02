using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TacticBase
{
    public readonly TacticTitle tacticTitle;

    public Queue<ITacticalStep> tacticalSteps = new Queue<ITacticalStep>();

    protected EnemyBaseControl EnemyBase { get; set; }

    protected PathFinderController PathFinderController { get; set; }


    public TacticBase(EnemyBaseControl enemyBase, PathFinderController pathFinder)
    {
        EnemyBase = enemyBase;
        PathFinderController = pathFinder;
    }


    public virtual bool ExecuteStep()
    {
        //Debug.Log("execute step");
        if (tacticalSteps.Count == 0) return true;

        var step = tacticalSteps.Peek();

        

        if (step.Execute())
        {
            tacticalSteps.Dequeue();
        }

        return tacticalSteps.Count == 0;
    }

    public abstract void InitialiseTactic();

    public virtual TacticContext GenerateContext()
    {
        return null;
    }


}
