using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public float projectileDamage;
    [HideInInspector] public float projectileRange;
    [HideInInspector] public Vector2 projectileDirection;

    [HideInInspector] public WeaponSO weapon;
    public TrailRenderer tracer;

    private float distanceTravelled = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        tracer.time = weapon.tracerLength / weapon.projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPos = transform.position;
        transform.position += (Vector3)projectileDirection * projectileSpeed * Time.deltaTime;
        distanceTravelled += projectileSpeed * Time.deltaTime;
        

        if(distanceTravelled >= projectileRange)
        {
            Destroy(gameObject);
        }
        
    }


}
