using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [HideInInspector] public float projectileMaxSpeed;
    [HideInInspector] public float projectileMaxDamage;
    [HideInInspector] public float projectileRange;
    [HideInInspector] public Vector2 projectileDirection;

    [HideInInspector] public WeaponSO weapon;
    public TrailRenderer tracer;

    private float distanceTravelled = 0;
    private float currentSpeed;
    private float distanceToHit;
    private Vector2 hitPoint;
    private IHittable hitObject;

    [SerializeField] private GameObject hitMarker;

    public AnimationCurve ProjectileSpeedDistance;
    public AnimationCurve DamageOverDistance;

    public LayerMask layerMask;



    // Start is called before the first frame update
    void Start()
    {
        tracer.time = weapon.tracerLength / weapon.projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        float graphXValue = distanceTravelled / projectileRange;

        currentSpeed = ProjectileSpeedDistance.Evaluate(graphXValue) * projectileMaxSpeed;
        //Debug.Log("proejectile speed: " + currentSpeed);
        Vector2 currentPos = transform.position;

        bool hit = FireRayOneFrame();

        if (hit)
        {
            transform.position += new Vector3(hitPoint.x,hitPoint.y,0);
            distanceTravelled += distanceToHit * Time.deltaTime;
            Instantiate(hitMarker, hitPoint, Quaternion.identity);

            float hitDamage = DamageOverDistance.Evaluate(graphXValue) * weapon.damage;
            //tell the object its been hit
            hitObject.OnHit(hitDamage, hitPoint);

            Debug.Log("damage is " + hitDamage);

            Destroy(gameObject);


        }
        else
        {
            transform.position += (Vector3)projectileDirection * currentSpeed * Time.deltaTime;
            distanceTravelled += currentSpeed * Time.deltaTime;
            if (distanceTravelled >= projectileRange)
            {
                Instantiate(hitMarker, this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        
        
        

        
        
    }

    private bool FireRayOneFrame()
    {
        float rayLength = currentSpeed * Time.deltaTime;       

        RaycastHit2D hit =  Physics2D.Raycast(transform.position,projectileDirection,rayLength, layerMask);

        if (hit.collider == null)
        {
           // Debug.Log("no hits");
           return false;
        }
        else
        {
            Debug.Log("hit " +  hit.collider.gameObject.name);
            distanceToHit = hit.distance;
            hitPoint = hit.point;

            hitObject = hit.collider.gameObject.GetComponent<IHittable>();

            

            return true;
        }

    }

}


