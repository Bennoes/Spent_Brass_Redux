using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour,IHittable
{
    public event Action OnDeath;
    public float CurrentArmour { get; set; }

    public float MaxHitPoints;

    public float speed = 5;
    private float fuzzedSpeed;
    private float speedFuzzTimer;
    public float speedChangeTime;
    public float speedRange;

    public float siteRange = 5;

    


    public GameObject hitText;

    public EnemyStates currentState = EnemyStates.Patrol;

    public PathFinderController pathFinder;
    private Vector2 nextDestination;
    public GameObject player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private List<Vector2> path = new List<Vector2>();

    //look and move direction tracking
    [SerializeField] GameObject lookDirectionIndicator;
    [SerializeField] GameObject travelDirectionIndicator;

    private Vector2 lastPosition;
    private Vector2 currentPosition;
    private Vector2 travelDirection;



    public void OnHit(float damage, Vector2 textPos)
    {
        Debug.Log("Enemy Hit");
        
        CurrentArmour -= damage;
        

        Vector2 newTextPos = Utilities.Fuzz(textPos, 0.5f);

        GameObject thisHitText = Instantiate(hitText, newTextPos, Quaternion.identity);
        HitTextController thisTextControl = thisHitText.GetComponent<HitTextController>();
        thisTextControl.damage = damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentArmour = MaxHitPoints;
        path.Clear();
        lastPosition = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentArmour    <= 0)
        {
            Destroy(gameObject);
        }

        currentPosition = transform.position;

        EnemyStateMachine();

        lastPosition = transform.position;

        travelDirection = lastPosition - currentPosition;
        travelDirection = travelDirection.normalized;
        travelDirectionIndicator.transform.localPosition = travelDirection;

        SpeedFuzzer();
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

                if(x >= arrayXmax || y >= arrayYmax) continue;

                if (pathFinder.atlasController.gameAtlus[x,y] == null) continue;

                if (pathFinder.atlasController.gameAtlus[x,y].permanentInaccessable) continue;

                //Debug.Log("node added");
                patrolList.Add(pathFinder.atlasController.gameAtlus[x,y]);
            }
        }


        int randomNodeNumber = UnityEngine.Random.Range(0, patrolList.Count);
        //Debug.Log("length of array " + patrolList.Count);
        //Debug.Log("random number: " + randomNodeNumber );
        AtlusNode targetNode = patrolList[randomNodeNumber];

         

        path = pathFinder.GetPathOfVectors(this.transform.position,targetNode.worldCoordintates);

        


    }



    public void EnemyStateMachine()
    {
        


        switch (currentState)
        {
            case EnemyStates.Patrol:

                PatrolState();


                break;

            case EnemyStates.Attack:

                Debug.Log("Enemy Attacking");

                break;

        }

    }


    private void PatrolState()
    {
        //Debug.Log("enemy patrolling");
        if(path.Count == 0)
        {
            //get path from pathfinder
            //Debug.Log("path complete");
            TempEnemyPatrol();
        }
        else
        {
            nextDestination = path[0];
            //follow path
            
            
            
            if(Vector2.Distance(transform.position, nextDestination) < 0.1)
            {
                //you've arrived - remove next destination from list
                //Debug.Log("you've arrived at the next vector 2 position");
                path.Remove(nextDestination);
                
            }
            else
            {
                //Debug.Log("moving toward " + nextDestination.x + "," + nextDestination.y);
                transform.position = Vector2.MoveTowards(transform.position, nextDestination, fuzzedSpeed * Time.deltaTime);

            }
           
        }
    }



    private void SpeedFuzzer()
    {
        if (speedFuzzTimer <= 0)
        {
            fuzzedSpeed = Utilities.Fuzz(speed, speedRange);
            speedFuzzTimer = speedChangeTime;
        }
        else
        {
            speedFuzzTimer -= Time.deltaTime;
        }
    }
}

public enum EnemyStates
{
    Patrol,
    Attack,
    Wait,
    Escape
}



