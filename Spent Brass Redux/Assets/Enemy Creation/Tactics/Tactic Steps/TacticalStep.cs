using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a class to hold building blocks of tactics
//these simple moves should be built up to create more complicated tactics
public interface ITacticalStep
{
    public EnemyBaseControl EnemyBase { get; set; }

    public TacticContext Context { get; set; }

    public bool Execute();

    public void InitialiseStep(TacticContext context);




}

public class TacticContext
{
    // Holds shared data needed by all steps within a tactic.
    // Used to pass consistent context (e.g., player position, target nodes) to each TacticalStep during execution.

    public Vector2? DestinationPosition { get; set; }
    public float? WaitTime { get; set; }

    public Vector2? Target { get; set; }

    //public GameObject EnemyObject { get; set; }

    //public EnemyBaseControl EnemyController { get; set; }

    public TacticContext(Vector2? destinationPosition, float? waitTime, Vector2? target)
    {
        DestinationPosition = destinationPosition;
        WaitTime = waitTime;
        Target = target;
    }
}


