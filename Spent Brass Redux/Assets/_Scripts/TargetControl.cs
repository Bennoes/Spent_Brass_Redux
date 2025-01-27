using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour, IHittable
{

    public float HitPoints { get; set; }

    public float MaxHitPoints;
    

    // Start is called before the first frame update
    void Start()
    {
        HitPoints = MaxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(HitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnHit(float damage)
    {
        
        Debug.Log(gameObject.name + " was hit");
        HitPoints -= damage;
        Debug.Log($"hit points remaing: {HitPoints}");

        
    }

   
}
