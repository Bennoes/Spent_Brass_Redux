using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


//AC controller manages AC levels and generates a sit rep to pass to tactics coordinator
public class ACController : MonoBehaviour, IStimuliResponder
{
    //event to pass sitrep to coordinator
    public event Action<SitRep> OnSitRepUpdate;
    public event Action OnRequestCancelTactic;
    

    public EnemyBaseControl enemyBaseControl;
    public EnemySensor enemySensor;
    
    private EnemyWeaponControl enemyWeaponControl;

    [SerializeField] private ACProfile profile;     //the SO that contains enemy type specific AC reactions to events
    private Dictionary<StimulusType, Vector2> ACResponceDictionary;

 

    private Dictionary<StimulusType, Action<Stimulus>> stimulsHandlers;

    float Aggression = 0;
    float Confidence = 0;

    private void Awake()
    {
        InitialiseHandlerDictionary();
       
    }

    private void Start()
    {
        enemyWeaponControl = gameObject.GetComponentInChildren<EnemyWeaponControl>();
        if (enemyWeaponControl == null) Debug.Log("enemy weapon control is null");
        GetACProfile();

        InitialiseACResponse();

        
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(this.gameObject.name + " current AC is");
            Debug.Log(Aggression + " and " + Confidence);
        }
    }

    private void InitialiseHandlerDictionary()
    {
        //sets up the type and method in a dictionary
        stimulsHandlers = new Dictionary<StimulusType, Action<Stimulus>>();

        stimulsHandlers.Add(StimulusType.TookDamage, HitByPlayer);
        stimulsHandlers.Add(StimulusType.SpottedPlayer, SpottedPLayer);
    }
    

    private void GetACProfile()
    {
        EnemyType type = enemyBaseControl.enemyType;

        profile = ACProfileRegistry.Instance.ReturnProfile(type);
    }

    public void InitialiseACResponse()
    {
        if (profile == null) return;

        ACResponceDictionary = new Dictionary<StimulusType, Vector2>();

        foreach (var response in profile.responses)
        {
            ACResponceDictionary[response.stimulus] = response.acChange;
            //could fuzz the values here as well to add additional 
        }
    }

    public Vector2 GetResponse(StimulusType type)
    {
        return ACResponceDictionary.TryGetValue(type, out var value) ? value : Vector2.zero;
    }

    public void ReceiveStimulus(Stimulus stimulus)
    {
        //Debug.Log(stimulus.type);
        //Debug.Log("run ac update");
        
        if(stimulsHandlers.TryGetValue(stimulus.type, out var handler))
        {
            handler(stimulus);
        }
        else
        {
            Debug.Log("key not found. AC unchanged");
           
        }
    }

   

    private void HitByPlayer(Stimulus stimulus)
    {
              
       Vector2 response = GetResponse(stimulus.type);

        Debug.Log("hit by player. AC changed by " + response);
        Aggression += response.x;
        Confidence += response.y;

        
    }




    private void SpottedPLayer(Stimulus stimulus)
    {
        OnRequestCancelTactic?.Invoke();

        Vector2 response = GetResponse(stimulus.type);
        Debug.Log("Spotted Player. AC changed by " + response);
        Aggression += response.x;
        Confidence += response.y;

        SitRep sitrep = GenerateSitRep();
        //invokes event that pushes sitrep to ac controller
        OnSitRepUpdate?.Invoke(sitrep);
    }

    public SitRep GenerateSitRep()
    {
        //if(enemyWeaponControl == null) enemyWeaponControl = gameObject.GetComponentInChildren<EnemyWeaponControl>();
        
        
        SitRep sitrep = new SitRep
        {

            Aggression = Aggression,
            Confidence = Confidence,
            PlayerInSight = enemySensor.CanSeePlayer(),
            EnemyLevel = enemyBaseControl.enemyRank,
            EnemyType = enemyBaseControl.enemyType,
            CurrentAmmo = enemyWeaponControl.currentAmmoCount,
            MaxAmmo = enemyWeaponControl.weaponData.maxAmmo,
            //other stats
        };


        return sitrep;
    }

}

