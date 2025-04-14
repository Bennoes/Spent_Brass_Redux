using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySensor : MonoBehaviour
{
    //responsible for gathering info around the unit
    //visual range and arc
    //hearing range

    public EnemyBaseControl enemyBase;


    //enemy stats
    private float sightRange = 5;
    private float fieldOfView = 90;

    private float perceptionTick;

    [SerializeField] private LayerMask visionMask;

   


    // Start is called before the first frame update
    void Start()
    {
        perceptionTick = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % perceptionTick  == 0)
        {
            
            VisualSweep();
        }
    }

    private void VisualSweep()
    {
        Collider2D[] localColliders = Physics2D.OverlapCircleAll(this.transform.position, sightRange);

        GameObject tempPlayer = null;

        foreach (Collider2D col in localColliders)
        {
            if (col.CompareTag("Player"))
            {              
                tempPlayer = col.gameObject;
                break;
            }
            else
            {
                continue;
            }
        }

        if (tempPlayer == null) return;

        

        Vector2 facingDirection = enemyBase.travelDirection;

        if (!HasLineOfSight(facingDirection, sightRange, visionMask)) return;

        Vector2 enemyToPlayer = tempPlayer.transform.position - this.transform.position;

        if(Vector2.Angle(enemyToPlayer, facingDirection) <= fieldOfView / 2)
        {
            
            IStimuliResponder responder = gameObject.GetComponent<IStimuliResponder>();
            Stimulus thisStim = new Stimulus(StimulusType.SpottedPlayer);
            responder?.ReceiveStimulus(thisStim);
        }
    }

    private bool HasLineOfSight(Vector2 direction, float sightRange, LayerMask mask)
    {
        
         RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, sightRange, mask);

        if (hit.collider != null && hit.transform.gameObject.CompareTag("Player"))
        {
            return true;
        }
        else
        {          
            return false;
        }

    }


}
