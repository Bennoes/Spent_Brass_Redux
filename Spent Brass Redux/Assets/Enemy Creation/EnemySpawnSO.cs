using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Part", menuName = "Enemy Creation/Enemy Spawn Info")]
public class EnemySpawnSO : ScriptableObject
{

    
    

    [Header("Enemy Rank Icons")]
    [Header("")]
    public List<Sprite> rankInsignia;


    [Header("Enemy Class Icons")]
    [Header("")]
    public Sprite grunt;
    public Sprite scout;
    public Sprite heavy;
    public Sprite shock;

    [Header("Enemy Modules")]
    [Header("")]
    public List<EnemyPartControl> enemyParts;
    


}
