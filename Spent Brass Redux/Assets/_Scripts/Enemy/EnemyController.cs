using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour,IHittable
{
    public float HitPoints { get; set; }

    public float MaxHitPoints;

    public float speed = 5;
    public GameObject hitText;

    public EnemyStates currentState = EnemyStates.Patrol;

    public PathFinderController pathFinder;
    private Vector2 nextDestination;
    public GameObject player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private List<Vector2> path = new List<Vector2>();

    public void OnHit(float damage, Vector2 textPos)
    {
        Debug.Log("Enemy Hit");
        
        HitPoints -= damage;
        

        Vector2 newTextPos = Utilities.Fuzz(textPos, 0.5f);

        GameObject thisHitText = Instantiate(hitText, newTextPos, Quaternion.identity);
        HitTextController thisTextControl = thisHitText.GetComponent<HitTextController>();
        thisTextControl.damage = damage;

        spriteRenderer.color = Color.red;

        Debug.Log("targer pos " + transform.position);

        Debug.Log("player pos " + player.transform.position);

        SpriteRenderer mesh = player.GetComponent<SpriteRenderer>();
        Vector2 playerFeet = new(player.transform.position.x, player.transform.position.y - mesh.bounds.size.y/2);
        path = pathFinder.GetPathOfVectors(transform.position, playerFeet);
    }

    // Start is called before the first frame update
    void Start()
    {
        HitPoints = MaxHitPoints;
        path.Clear();

        
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (HitPoints <= 0)
        {
            Destroy(gameObject);
        }

        EnemyStateMachine();



        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
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
                transform.position = Vector2.MoveTowards(transform.position, nextDestination, speed * Time.deltaTime);

            }
           
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



