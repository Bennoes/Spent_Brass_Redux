using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy Part", menuName = "Enemy Creation/Enemy Part")]
public class EnemyPartSO : ScriptableObject
{

    [Header("Enemy Part Set Up")]
    [Header("")]

    public EnemyParts enemyPart;
    public EnemyType enemyType;
    public EnemyLevel enemyLevel;

    [Header("Basic Stats")]
    [Header("")]

    public float baseArmour;
    public float armourModifier;

    public float baseSpeed;
    public float speedModifier;

    public float siteRange;

    public float opacityMultiplier;

    [Header("Enemy AI")]
    [Header("")]

    [Range(-1, 1)] public float aggressionModifier;
    [Range(-1, 1)] public float confidenceModifier;

    [Header("Basic Animations")]
    [Header("")]

    public Animator animationControl;

    public List<ClipDirection> animationClips;







}

public enum EnemyParts
{
    HEAD,
    BODY,
    ARMS,
    LEGS
}


[System.Flags]
public enum EnemyType
{
    GRUNT = 1 << 0,
    SCOUT = 1 << 1,
    HEAVY = 1 << 2,
    SHOCK = 1 << 3
}

[System.Flags]
public enum EnemyLevel
{
    ROOKIE = 1 << 0,
    VETERAN = 1 << 1,
    ELITE = 1 << 2
}

public enum Directions
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest
}

public enum AnimType
{
    Idle,
    Run,
}

[System.Serializable]
public struct ClipDirection
{
    public Directions direction;
    
    public EnemyParts enemyPart;

    public AnimType animationType;

    public AnimationClip clip;
}
