using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicPatrolTactic : TacticBase
{
    //name of class as enum; ties this class to key data
    public new readonly TacticTitle tacticTitle =  TacticTitle.BasicPatrol;

    EnemySensor sensor;

    public BasicPatrolTactic(EnemyBaseControl enemyBase, PathFinderController pathFinder): base(enemyBase, pathFinder)
    {
            sensor = enemyBase.gameObject.GetComponent<EnemySensor>();
    }

    public override bool ExecuteStep()
    {
        //Debug.Log("execute step");
        if (tacticalSteps.Count == 0) return true;

        var step = tacticalSteps.Peek();

        Vector2 lastPosition = EnemyBase.gameObject.transform.position;

        if (step.Execute())
        {
            tacticalSteps.Dequeue();
        }

        //after enemy has moved this frame
        Vector2 currentPosition = EnemyBase.gameObject.transform.position;

        EnemyBase.travelDirection =  currentPosition - lastPosition;
        //Debug.Log("visual sweep");
        sensor.VisualSweep();

        return tacticalSteps.Count == 0;
    }


    private AtlusNode GetTargetLocation()
    {
        tacticalSteps.Clear();

        if (PathFinderController == null) { Debug.Log("path finder is null"); return null; }

        Vector2 position = EnemyBase.gameObject.transform.position;

        //get random tile in range
        
        List<AtlusNode> nodeList = PathFinderController.atlasController.GetNodesInRange(position, (int)EnemyBase.sightRange, EnemyBase.gameObject);
        AtlusNode node = nodeList[Random.Range(0, nodeList.Count)];

        return node;
    }

    public override void InitialiseTactic()
    {
        //Debug.Log("Patrol");
        //Debug.Log("initialising tactic");
        AtlusNode targetNode = GetTargetLocation();

        if (targetNode != null)
        {
            var list = GetPathOfVectors(targetNode);

            foreach (Vector2 waypoint in list)
            {
               
                TacticContext context = new(waypoint,null, null);

                MoveToPointStep step = new MoveToPointStep(EnemyBase, context);
                step.InitialiseStep(context);
                
                tacticalSteps.Enqueue(step);

            }

        }
        else
        {
            Debug.Log("target node is null");
        }
    }

    private List<Vector2> GetPathOfVectors(AtlusNode targetNode)
    {
        Vector2 position = EnemyBase.gameObject.transform.position;
        Vector2 targetVector = targetNode.worldCoordintates;

        return PathFinderController.GetPathOfVectors(position, targetVector);
    }

    
    
}
