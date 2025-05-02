using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class EnemySensor : MonoBehaviour
{
    //responsible for gathering info around the unit
    //visual range and arc
    //hearing range

    public EnemyBaseControl enemyBase;


    //enemy stats
    private float sightRange = 10;
    private float fieldOfView = 160;

    //private float perceptionTick;

    [SerializeField] private LayerMask visionMask;

    public Transform GetPlayerTransform()
    {
        Collider2D[] localColliders = Physics2D.OverlapCircleAll(this.transform.position, sightRange);
        GameObject tempPlayer = null;

        foreach (Collider2D col in localColliders)
        {
            if (col.CompareTag("Player"))
            {
                //Debug.Log("found player collider");
                tempPlayer = col.gameObject;
                break;
            }
        }

        if (tempPlayer == null) return null;

        if (!HasLineOfSight(tempPlayer.transform.position, sightRange, visionMask)) return null;

        return tempPlayer.transform;
    }

    public bool CanSeePlayer()
    {
        Vector2 facingDirection = (enemyBase.travelDirection).normalized;

        Transform playerTransform = GetPlayerTransform();

        if(playerTransform == null) return false;

        Vector3 playerPos = playerTransform.position;

        Vector2 enemyToPlayer = (playerPos - this.transform.position).normalized;

        float angle = Vector2.Angle(enemyToPlayer, facingDirection);

        // Draw the facing direction
        Debug.DrawRay(transform.position, facingDirection * sightRange, Color.green);

        // Draw the direction to the player
        Debug.DrawRay(transform.position, enemyToPlayer * sightRange, Color.red);

        // Draw FoV boundaries
        Vector2 leftBound = Quaternion.Euler(0, 0, fieldOfView / 2f) * facingDirection;
        Vector2 rightBound = Quaternion.Euler(0, 0, -fieldOfView / 2f) * facingDirection;

        Debug.DrawRay(transform.position, leftBound * sightRange, Color.blue);
        Debug.DrawRay(transform.position, rightBound * sightRange, Color.blue);

        return angle <= fieldOfView / 2;
    }

    


    public bool VisualSweep()
    {
        //Debug.Log("vis sweep running " + Time.time);
        if (!CanSeePlayer()) return false;

        IStimuliResponder responder = gameObject.GetComponent<IStimuliResponder>();
        Stimulus thisStim = new Stimulus(StimulusType.SpottedPlayer);

        Debug.Log("spotted player");
        responder?.ReceiveStimulus(thisStim);

        return true;
    }

    private bool HasLineOfSight(Vector2 target, float sightRange, LayerMask mask)
    {
        Vector2 origin = this.transform.position;

        Vector2 direction = (target - origin).normalized;
        

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, sightRange + 1, mask);
        //Debug.DrawRay(origin, direction * (sightRange + 1), Color.red, 0.1f);

               
        if (hit.collider != null && hit.transform.gameObject.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            if(hit.collider != null) Debug.Log("check ray hit " + hit.collider.name);

            return false;
        }


    }

  

}
