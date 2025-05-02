using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "New Key" , menuName = "Tactics/Create Key")]
public class TacticsKey : ScriptableObject
{
    public TacticTitle title;

    public EnemyType enemyType;
    
    public AnimationCurve enemyLevel;

    public AnimationCurve requiredAmmo;

    

    public AnimationCurve Aggression;
    public AnimationCurve Confidence;

    public bool PlayerInSight;



}

public enum TacticTitle
{
    BasicPatrol,
    MagDump,
    FallBack,



}


