using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TacticsLibrary : MonoBehaviour
{
    public PathFinderController pathfinder;

    public Dictionary<TacticTitle, Func<EnemyBaseControl, TacticBase>> TacticFactory = new Dictionary<TacticTitle, Func<EnemyBaseControl, TacticBase>>();


    private void Start()
    {
        if(pathfinder == null)
        {
            Debug.Log("Path finder is null");
        }
        else
        {
            //the => in this allowed the enemy base to be assigned on teh fly
            TacticFactory.Add(TacticTitle.BasicPatrol, enemyBase => new BasicPatrolTactic(enemyBase, pathfinder));
            TacticFactory.Add(TacticTitle.FallBack, enemyBase => new FallBack(enemyBase, pathfinder));
            TacticFactory.Add(TacticTitle.MagDump, enemyBase => new MagDumpTactic(enemyBase, pathfinder));
        }


        
    }


    

}
