using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ACProfileRegistry : MonoBehaviour
{
    //this holds enemy "personalities" based on type
    //they are assigned to the enemy on spawn
    //uses the singleton pattern to make it eaily available
    public static ACProfileRegistry Instance {  get; private set; }

    [SerializeField] private List<ACProfile> profileList;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ACProfile ReturnProfile(EnemyType enemyType)
    {
        return profileList.FirstOrDefault(ACProfile => ACProfile.enemyType == enemyType);

    }
    
}
