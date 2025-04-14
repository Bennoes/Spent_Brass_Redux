using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACController : MonoBehaviour, IStimuliResponder
{
    public EnemyBaseControl enemyBaseControl;

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
        GetACProfile();

        InitialiseACResponse();
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
        Debug.Log(stimulus.type);
        Debug.Log("run ac update");
        
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
        Vector2 response = GetResponse(stimulus.type);
        Debug.Log("Spotted Player. AC changed by " + response);
        Aggression += response.x;
        Confidence += response.y;

    }

}    
