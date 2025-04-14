using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class EnemyBaseControl : MonoBehaviour, IHittable
{
    public event Action OnDeath;

    [HideInInspector] public Dictionary<EnemyParts, EnemyPartControl> enemyParts = new Dictionary<EnemyParts, EnemyPartControl>();

    public float CurrentArmour { get; set; }
    public float maxArmour;
    public float speed;
    public float siteRange;
    public float totalOpacityMultiplier;
    public SpriteRenderer spriteRenderer;

    public GameObject hitText;

    public EnemyType enemyType;
    [HideInInspector] public int enemyRank; 

    public Canvas enemyHud;


    //animation control
    public RuntimeAnimatorController baseAnimationController;
    public AnimatorOverrideController AnimatorOverrideController;

    public Animator enemyAnimator;
    

    private Vector2 lastPosition;
    private Vector2 currentPosition;
    public Vector2 travelDirection;
    

    [SerializeField] GameObject travelDirectionIndicator;

    public PathFinderController pathFinder;
    private Vector2 nextDestination;
    private List<Vector2> path = new List<Vector2>();

    private ACController ACcontrol;

    

    // Start is called before the first frame update
    void Start()
    {
        ACcontrol = gameObject.GetComponent<ACController>();


        if(spriteRenderer != null)
        {
            
            Color temp = spriteRenderer.color;
            temp.a =  totalOpacityMultiplier;
            spriteRenderer.color = temp;
            
        }
        else
        {
            Debug.Log("sprite renderer is null");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        

        


        
        currentPosition = transform.position;

        //method that moves object 

        if(path.Count == 0 )
        {

            if(pathFinder != null)
            {
                TempEnemyPatrol();
            }
            else
            {
                //Debug.Log("path finder null");
            }
            
        }
        else
        {

            nextDestination = path[0];
            //follow path

            if (Vector2.Distance(transform.position, nextDestination) < 0.1)
            {
                path.Remove(nextDestination);

            }
            else
            {
                currentPosition = transform.position;

                transform.position = Vector2.MoveTowards(transform.position, nextDestination, speed * Time.deltaTime);
                lastPosition = transform.position;

                travelDirection = lastPosition - currentPosition;
                travelDirection = travelDirection.normalized;
                
                travelDirectionIndicator.transform.localPosition = travelDirection;
            }

        }
        
        RunAnimations();

        
    }

    public void OnHit(float damage, Vector2 textPos)
    {
        //Debug.Log("Enemy Hit");

        CurrentArmour -= damage;


        Vector2 newTextPos = Utilities.Fuzz(textPos, 0.5f);

        GameObject thisHitText = Instantiate(hitText, newTextPos, Quaternion.identity);
        HitTextController thisTextControl = thisHitText.GetComponent<HitTextController>();
        thisTextControl.damage = damage;
        
        if(CurrentArmour <= 0)
        {
            Die();
        }

        //following code runs appropriate response in the AC controller
        //sends data via Stimulus struct
        IStimuliResponder responder = gameObject.GetComponent<IStimuliResponder>();
        Stimulus thisStim = new Stimulus(StimulusType.TookDamage);
        responder?.ReceiveStimulus(thisStim);



        
    }

    private void Die()
    {

        OnDeath?.Invoke();

        Destroy(gameObject);

    }

    

    private void TempEnemyPatrol()
    {
        List<AtlusNode> patrolList = new List<AtlusNode>();

        AtlusNode currentNode = pathFinder.atlasController.GetNodeAtPoint(this.transform.position);

        float currentX = currentNode.arrayCoordinates.x;
        float currentY = currentNode.arrayCoordinates.y;

        int arrayXmax = pathFinder.atlasController.gameAtlus.GetLength(0);
        int arrayYmax = pathFinder.atlasController.gameAtlus.GetLength(1);


        for (int x = (int)currentX - (int)siteRange; x < currentX + siteRange; x++)
        {
            for (int y = (int)currentY - (int)siteRange; y < currentY + siteRange; y++)
            {
                //Debug.Log(x + " and " + y);
                if (x < 0 || y < 0) continue;

                if (x >= arrayXmax || y >= arrayYmax) continue;

                if (pathFinder.atlasController.gameAtlus[x, y] == null) continue;

                if (pathFinder.atlasController.gameAtlus[x, y].permanentInaccessable) continue;

                patrolList.Add(pathFinder.atlasController.gameAtlus[x, y]);
            }
        }

        int randomNodeNumber = UnityEngine.Random.Range(0, patrolList.Count);
        
        AtlusNode targetNode = patrolList[randomNodeNumber];

        path = pathFinder.GetPathOfVectors(this.transform.position, targetNode.worldCoordintates);

    }

    private void RunAnimations()
    {
        if (enemyAnimator == null) Debug.Log("animator is also null");

        if(enemyAnimator.runtimeAnimatorController != null)
        {
            
            enemyAnimator.SetFloat("Horizontal", travelDirection.x);
            enemyAnimator.SetFloat("Vertical", travelDirection.y);

            
        }
        else
        {
            //Debug.Log("No run time animation controller set ");
        }
    }


}
