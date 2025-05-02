using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.ComponentModel;


//this class needs to take a sitrep from the ACController and respond with a tactic
public class TacticsCoordinator : MonoBehaviour
{
    //references to other objects that the coordinator needs
    public ACController ACController;
    public EnemyBaseControl enemyBaseControl;
    public EnemyWeaponControl enemyWeaponControl;
    //assigned by spawn controller
    public TacticsLibrary tacticsLibrary;

    //assigned on spawn
    [HideInInspector] public PathFinderController pathFinder;

    //keys to tactics
    public  TacticsKeyList keyList;
    private List<TacticsKey> keyChoices = new List<TacticsKey>();

    //cached current tactic
    TacticBase currentTactic;

    //method to analyse and score tactics againsts sitrep.
    //first remove not suitable methods by enemy type, bool and avaialable AC

    private void Start()
    {
        //subscribe to event that will push sitrep update:
        ACController.OnSitRepUpdate += HandleSitRepUpdate;
        ACController.OnRequestCancelTactic += CancelCurrentTactic;

        UpdateCurrentTactic();
    }


    private void Update()
    {
        if (RunTactic())
        {
            //get the most appropriate tactic from the list
            UpdateCurrentTactic();
            //set up the selected tactic
            currentTactic.InitialiseTactic();

            

        }
    }

    private void CancelCurrentTactic()
    {
        if (currentTactic != null)
        {
            //Debug.Log("cancel tactic");
            currentTactic = null;
        }
    }

    private void UpdateCurrentTactic()
    {
       
        SitRep sitrep = RequestSitRep();       

        HandleSitRepUpdate(sitrep);
    }


    private bool RunTactic()
    {
        //Debug.Log("execute step");
        if (currentTactic == null)
        {
            Debug.Log("current tactic is null");
            return true;

        }
        //no more step to be run -- need new tactic
        if (currentTactic.tacticalSteps.Count == 0) 
        {
            
            return true;

        }
        

        //get ref to tacticStep at top of queue
        if (!currentTactic.ExecuteStep())
        {
            return false;
        }
        else
        {
            return true;
        }      
       
    }

   public void HandleSitRepUpdate(SitRep sitrep)
    {
        TacticsKey key = AnalyseSitRep(sitrep);

        TacticTitle title = key.title;

        if (tacticsLibrary.TacticFactory.TryGetValue(key.title, out var factory))
        {
            currentTactic = factory(enemyBaseControl); // Pass the enemy instance
        }
        else
        {
            Debug.LogError($"Tactic not found in library for title: {key.title}");
        }
    }

    private SitRep RequestSitRep()
    {
        if(ACController == null)
        {
            Debug.Log("ac controller not assigned");
            return null;
        }
        else
        {
            SitRep sitrep = ACController.GenerateSitRep();

            return sitrep;
        }
        
    }

    //this gets a sitrep from the AC controller and needs to score it and compare to keys
    public TacticsKey AnalyseSitRep(SitRep sitrep)
    {
        //whittle down keys based on info in sitrep first.
        if (keyList == null) Debug.Log("keylist is null");
        
        var keys = keyList.Keys.Where(key => key.PlayerInSight == sitrep.PlayerInSight);

        //wrap the data in  a class that also holds a score

        List<ScoredTacticKey> scoredKeyList = new List<ScoredTacticKey>();

        foreach (var key in keys)
        {
            scoredKeyList.Add(WrapDataAndScore(key));
        }
        ScoreForAC(sitrep,scoredKeyList);
        //then score the remainder and arrange in score order
        ScoreForEnemyLevel(enemyBaseControl.enemyRank, scoredKeyList);

        ScoreForCurrentAmmo(sitrep, scoredKeyList);

        scoredKeyList = scoredKeyList.OrderByDescending(key => key.Score).ToList();

        //more methods to score

        foreach (var key in scoredKeyList)
        {
            //Debug.Log(key.Key.title + " scored: " + key.Score);
        }

        

        return scoredKeyList.FirstOrDefault().Key;


    }

    private void ScoreForAC(SitRep sitrep, List<ScoredTacticKey> scoredKeys)
    {
        //needs to be weighted quite heavily as AC sahould be deciding factor
        foreach (ScoredTacticKey key in scoredKeys)
        {
            Debug.Log("AC score is " + key.Key.Aggression.Evaluate(sitrep.Aggression) + "/" + key.Key.Confidence.Evaluate(sitrep.Confidence));

            key.Score += key.Key.Aggression.Evaluate(sitrep.Aggression);

            key.Score += key.Key.Confidence.Evaluate(sitrep.Confidence);
        }
    }

    private void ScoreForEnemyLevel(float EnemyLevel, List<ScoredTacticKey> scoredKeys)
    {
        
        foreach (ScoredTacticKey key in scoredKeys)
        {
            Debug.Log(key.Key.name + " Score enemy level is " + key.Key.enemyLevel.Evaluate(EnemyLevel));
            //get score from curve:
            key.Score += key.Key.enemyLevel.Evaluate(EnemyLevel);
        }
     
    }

    private void ScoreForCurrentAmmo(SitRep sitrep, List<ScoredTacticKey> scoredKeys)
    {
        int currentAmmo = sitrep.CurrentAmmo;
        int maxAmmo = sitrep.MaxAmmo;

        float ammoFraction = currentAmmo / maxAmmo;

        foreach (ScoredTacticKey key in scoredKeys)
        {
            //get score from curve:
            Debug.Log(key.Key.name + " Score for ammo is " + key.Key.requiredAmmo.Evaluate(ammoFraction));
            key.Score += key.Key.requiredAmmo.Evaluate(ammoFraction);
        }
    }




    private ScoredTacticKey WrapDataAndScore(TacticsKey key)
    {
        int score = 0;

        return new ScoredTacticKey(key, score);
        
    }



}

public class ScoredTacticKey
{
    public TacticsKey Key { get; set; }
    public float Score { get; set; }

    public ScoredTacticKey(TacticsKey key, float score)
    {
        Key = key;
        Score = score;
    }

}
