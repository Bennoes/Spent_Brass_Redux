using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpentAmmoControl : MonoBehaviour
{

    public Vector2 initialVelocity = new Vector2(0.1f, 0.1f);
    private Vector2 fuzzedVelocity;
    public float fuzzFactor = 0.05f;
    public float distanceToGround;

    private float fuzzedDistanceToFloor;
    public float grav = 10;
    

    private float timeElapsed;
    private Vector2 velocityAtTime;

    public Animator animator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        fuzzedVelocity = Utilities.Fuzz(initialVelocity,fuzzFactor);
        fuzzedDistanceToFloor = Utilities.Fuzz(distanceToGround,0.2f);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        MoveAlongCurve();
    }

    private void MoveAlongCurve()
    {
        
        if(velocityAtTime.y > -fuzzedDistanceToFloor)
        {
            velocityAtTime.x = fuzzedVelocity.x * timeElapsed;

            float vt = fuzzedVelocity.y * timeElapsed;
            float halfGTsq = 0.5f * grav * Mathf.Pow(timeElapsed, 2);
            velocityAtTime.y = vt - halfGTsq;

            this.transform.localPosition = (Vector3)velocityAtTime;

        }
        else
        {
            //Debug.Log("spent brass has hit the floor");
            animator.speed = 0;

        }
        //calculate current x position value
        
    }


}
