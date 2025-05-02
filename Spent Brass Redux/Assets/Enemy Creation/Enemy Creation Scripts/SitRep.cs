using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitRep
{
    // a class that is used to send enemy info to tactic selection.
    //it holds a slice of info at the moment the enemy calls for it

    public float Aggression { get; set; }
    public float Confidence { get; set; }
    public bool PlayerInSight { get; set; }
    public float EnemyLevel { get; set; }
    public EnemyType EnemyType { get; set; }
    public int CurrentAmmo {  get; set; }

    public int MaxAmmo { get; set; }
    public Vector2 PlayerPosition { get; set; }
    public float DistanceToPlayer { get; set; }



    public SitRep()
    {

    }
    


}
