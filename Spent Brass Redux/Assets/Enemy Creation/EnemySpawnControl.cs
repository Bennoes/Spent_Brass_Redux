
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EnemySpawnControl : MonoBehaviour
{
    public Vector2Int squadSizeRange;
    private int spawnCount;
    private bool spawning;


    public GameObject squadPrefab;
    private SquadControl thisSquadControl;

    public GameObject enemyBase;

    public TacticsLibrary tacticsLibrary;


    public PathFinderController pathFinder;

    [SerializeField] private EnemySpawnSO enemyPartLibrary;
    [SerializeField] private EnemyWeaponsLIbrary weaponsLibrary;
    [SerializeField] private int spawnLevel;
    [SerializeField] private AnimationCurve spawnCurve;

    

    // Start is called before the first frame update
    void Start()
    {

        

        if(enemyPartLibrary == null)
        {
            Debug.Log("enemy part library is null");
        }
        if(weaponsLibrary == null)
        {
            Debug.Log("enemy weapon library is null");
        }


    }

    // Update is called once per frame
    void Update()
    {
        SpawnSquad();
    }


    private void SpawnSquad()
    {
        

        //generate random number squad size
        if (Input.GetKeyDown(KeyCode.Space) && !spawning)
        {
            
            int squadSize = Random.Range(squadSizeRange.x, squadSizeRange.y + 1);
            spawnCount = squadSize;
            spawning = true;

            GameObject thisSquad = Instantiate(squadPrefab);

            thisSquadControl = thisSquad.GetComponent<SquadControl>();

        }

        if (spawning)
        {
            spawnCount--;
            if (spawnCount <= 0)
            {
                spawning = false;
                
                return;
            }

            EnemyBaseControl thisBaseControl = SpawnEnemy();

            thisSquadControl.squadMembers.Add(thisBaseControl);

            thisBaseControl.transform.SetParent(thisSquadControl.transform);
            
            


        }
        




    }

    private EnemyBaseControl SpawnEnemy()
    {
        //create the enemy base that will hold all of the component parts
        GameObject thisEnemy  = Instantiate(enemyBase, this.transform.position,Quaternion.identity);

        //set up enemy base object with its own refs first

        EnemyBaseControl thisEnemyControl = SetUpBaseReferences(thisEnemy);
        //might have to cache this next part
        TacticsCoordinator tacticsCoordinator = thisEnemy.GetComponent<TacticsCoordinator>();


        


        //choose head, body and legs based on spawn level
        GameObject head = Instantiate(CreateEnemyPart(EnemyParts.HEAD,0));

        GameObject body = Instantiate(CreateEnemyPart(EnemyParts.BODY, 0));

        GameObject legs = Instantiate(CreateEnemyPart(EnemyParts.LEGS, 0));

        if(head == null || body == null || legs == null)
        {
            Debug.Log("body part(s) missing");
            return null;               
        }
        //add them to the enemy base object
        head.transform.SetParent(thisEnemy.transform);

        body.transform.SetParent(thisEnemy.transform);

        legs.transform.SetParent(thisEnemy.transform);

        //calculate enemy base type from consituents


        thisEnemyControl.enemyType = AssignEnemyType(head, body, legs);

        int enemyRankNumber = GetPartLevelNumber(head) + GetPartLevelNumber(body) + GetPartLevelNumber(legs);
        //Debug.Log("enemy rank number is " + enemyRankNumber);

        thisEnemyControl.enemyRank = enemyRankNumber;

        //get access to UI panels on the base object to add the appropriate sprites according to type and rank

        SetUpEnemyHud(thisEnemyControl,enemyBase);

        //Debug.Log("enemy type is " + thisEnemyControl.enemyType);

        //Debug.Log("enemy base name is " + thisEnemyControl.gameObject.name);
        AssignPartPartRefsToBase(thisEnemyControl,head,body,legs);

        SetUpEnemyStats(thisEnemyControl);

        SetUpEnemyAnimations(thisEnemyControl);

        ApplyAnimations(thisEnemyControl);

        //at the momnent only giving rookie arms...
        CreateEnemyArms(EnemyLevel.ROOKIE, thisEnemy);
        
        //thesevalues will be set by parts. hard coded for now
        //eventually path finder will only be used by the tactics coordinator
        thisEnemyControl.pathFinder = pathFinder;
        tacticsCoordinator.pathFinder = pathFinder;
        tacticsCoordinator.tacticsLibrary = tacticsLibrary;
        thisEnemyControl.sightRange = 5;
     
        return thisEnemyControl;

    }

    private EnemyBaseControl SetUpBaseReferences(GameObject enemyBase)
    {
        if (enemyBase == null) Debug.Log("enemy base is null");

        EnemyBaseControl enemyBaseControl = enemyBase.gameObject.GetComponent<EnemyBaseControl>();

        if (enemyBaseControl == null) Debug.Log("enemy base control is null");

        enemyBaseControl.enemyHud = enemyBase.GetComponentInChildren<Canvas>();




        return enemyBaseControl;
    }

    private GameObject CreateEnemyPart(EnemyParts partRequired , int partLevel)
    {
        

        int enemyLevelInt = Random.Range(0, 3);

        

        EnemyLevel enemyLevel = (EnemyLevel)enemyLevelInt;

        //this should get a list of heads that are of the correct level
        //get all heads:
        List<EnemyPartControl> correctParts = enemyPartLibrary.enemyParts.Where
        (part => part.partData.enemyPart == partRequired).ToList();

        

        //then get the correct level
        
        
        List<EnemyPartControl> correctPartsAndLevel = correctParts.Where
        (part => part.partData.enemyLevel == enemyLevel).ToList();

        if (correctParts.Count <= 0)
        {
            Debug.Log("part list is empty - cancel");
            return null;
        }
            

        int chooseRandom = Random.Range(0, correctPartsAndLevel.Count);

        

        EnemyPartControl createdPart = correctPartsAndLevel[chooseRandom];
        
        return createdPart.gameObject;

    }

    private void CreateEnemyArms(EnemyLevel level, GameObject enemyBase)
    {
        //get a list of arms from the library
        
        EnemyParts partRequired = EnemyParts.ARMS;

        List<EnemyPartControl> allArms = enemyPartLibrary.enemyParts.Where
        (part => part.partData.enemyPart == partRequired).ToList();

        

        EnemyPartControl armsControl = allArms.FirstOrDefault();

        //Debug.Log("arms name " + armsControl.gameObject.name);
        //get type and level
        //pick a random from the remaining list

        GameObject thisArms = Instantiate(armsControl.gameObject, enemyBase.transform);
    
        EnemyWeaponControl enemyWeaponControl = thisArms.GetComponent<EnemyWeaponControl>();

        enemyWeaponControl.enemyBaseControl = enemyBase.GetComponent<EnemyBaseControl>();

        int randomInt = Random.Range(0, weaponsLibrary.enemyWeapons.Count);

        enemyWeaponControl.weaponData = weaponsLibrary.enemyWeapons[randomInt];

        if(enemyWeaponControl.animator != null)
        {
            //Debug.Log("weaponb anim is not null");
            enemyWeaponControl.animator.runtimeAnimatorController = enemyWeaponControl.weaponData.animationController;
        }
        else
        {
            Debug.Log("its null");
        }

        WeaponDropper dropper = enemyBase.GetComponent<WeaponDropper>();
        dropper.assignedWeapon = enemyWeaponControl.weaponData;







    }


    private EnemyType AssignEnemyType(GameObject head, GameObject body, GameObject legs)
    {
        EnemyPartSO headData = head.GetComponent<EnemyPartControl>().partData;
        EnemyPartSO bodyData = body.GetComponent<EnemyPartControl>().partData;
        EnemyPartSO legData = legs.GetComponent<EnemyPartControl>().partData;

        Dictionary<EnemyType,int> typeCount = new Dictionary<EnemyType,int>();

        //add data to dictionary

        AddToDictionary(typeCount, headData);
        AddToDictionary(typeCount, bodyData);
        AddToDictionary(typeCount, legData);

        int highestCount = 0;
        EnemyType dominantType = headData.enemyType;
        bool isDraw = false;

        foreach  (KeyValuePair<EnemyType,int> kvp in typeCount)
        {
            if(kvp.Value > highestCount)
            {
                highestCount = kvp.Value;
                isDraw = false;
                dominantType = kvp.Key;
            }
            else
            {
                isDraw = true;
            }
        }

        return isDraw ? headData.enemyType : dominantType;

    }

    private void AddToDictionary(Dictionary<EnemyType,int> dictionary, EnemyPartSO data)
    {

        if (dictionary.ContainsKey(data.enemyType))
        {
            dictionary[data.enemyType] ++;

        }
        else
        {
            dictionary.Add(data.enemyType, 1);
        }
    }


    private int GetPartLevelNumber(GameObject part)
    {
        EnemyPartControl partControl = part.GetComponent<EnemyPartControl>();

        int levelInt = (int)partControl.partData.enemyLevel;

        return levelInt;

    }

    private void SetUpEnemyHud(EnemyBaseControl enemyBaseControl, GameObject enemyBase)
    {
        Sprite rankInsignia = enemyPartLibrary.rankInsignia[enemyBaseControl.enemyRank];

        //get access to approriate panels on enemy canvas
        Canvas enemyCanvas = enemyBaseControl.enemyHud;

        RectTransform panel = enemyCanvas.GetComponentInChildren<RectTransform>();

        RectTransform[] UIpanels = panel.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform UIpanel in UIpanels)
        {
            if(UIpanel.name == "Enemy Rank")
            {
                //Debug.Log("enemy rank panel found");
                Image rankImage = UIpanel.GetComponent<Image>();
                rankImage.sprite = rankInsignia;


            }
            else if(UIpanel.name == "")
            {

            }
        }

    }

    private void AssignPartPartRefsToBase(EnemyBaseControl baseControl, params GameObject[] enemParts)
    {
        
        foreach (GameObject part in enemParts)
        {
            
            EnemyPartControl partControl = part.GetComponent<EnemyPartControl>();
            baseControl.enemyParts.Add(partControl.partData.enemyPart, partControl);

        }
    }

    private void SetUpEnemyStats(EnemyBaseControl thisBaseControl)
    {

        float totalArmour = 0;
        float totalSpeed = 0;
        float totalOpacity = 1;

        foreach (KeyValuePair<EnemyParts,EnemyPartControl> kvp in thisBaseControl.enemyParts)
        {
            totalSpeed += kvp.Value.partData.baseSpeed;

            totalArmour += kvp.Value.partData.baseArmour;

            totalOpacity *= kvp.Value.partData.opacityMultiplier;
        }


        thisBaseControl.maxArmour = totalArmour;
        thisBaseControl.speed = totalSpeed;
        thisBaseControl.CurrentArmour = totalArmour;
        thisBaseControl.totalOpacityMultiplier = totalOpacity;

    }

    private void SetUpEnemyAnimations(EnemyBaseControl thisBaseControl)
    {
        
        AnimatorOverrideController overrideController = thisBaseControl.AnimatorOverrideController;

        Dictionary<string, AnimationClip> animationDictionary = new Dictionary<string, AnimationClip>();

        foreach (KeyValuePair<EnemyParts,EnemyPartControl> kvp in thisBaseControl.enemyParts)
        {
            foreach (ClipDirection clipData in kvp.Value.partData.animationClips)
            {
                string key = CreateKey(clipData);

                if(animationDictionary.ContainsKey(key))
                {
                    Debug.Log("dictionary already contains clip key");
                    continue;

                }
                else
                {
                    animationDictionary.Add(key, clipData.clip);
                }
              
            }         
        }

        List<KeyValuePair< AnimationClip,AnimationClip>> overrideList = new List<KeyValuePair< AnimationClip,AnimationClip>>();

        overrideController.GetOverrides(overrideList);
        //key structure: "part type direction"

        // Modify the override list with the correct animations
        for (int i = 0; i < overrideList.Count; i++)
        {
            string key = CreateKey(overrideList[i].Key); // Key for placeholder animation

            if (animationDictionary.TryGetValue(key, out AnimationClip correctClip))
            {
               // Debug.Log($"Replacing clip {overrideList[i].Key.name} with {correctClip.name}");
                overrideList[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrideList[i].Key, correctClip);
            }
            else
            {
                Debug.LogWarning($"No matching clip found for {overrideList[i].Key.name} in the dictionary.");
            }
        }

        // Apply the updated list back to the AnimatorOverrideController
        overrideController.ApplyOverrides(overrideList);

        //Debug.Log("Overrides applied successfully.");
    }

    

    private void ApplyAnimations(EnemyBaseControl thisBaseControl)
    {
        //Debug.Log("firing?");
        //thisBaseControl.enemyAnimator.runtimeAnimatorController = thisBaseControl.baseAnimationController;

        thisBaseControl.enemyAnimator.runtimeAnimatorController = thisBaseControl.AnimatorOverrideController;
    }

    private string CreateKey(ClipDirection clipDirection)
    {
        //key structure: "part type direction"
        string part = clipDirection.enemyPart.ToString().ToLower();
        string type = clipDirection.animationType.ToString().ToLower();
        string direction = clipDirection.direction.ToString().ToLower();

        string key = $"{part} {type} {direction}";

        return key;
    }

    private string CreateKey(AnimationClip clip)
    {
        //key structure: "part type direction"
        string key = clip.name.ToLower();

        return key;
    }


}
