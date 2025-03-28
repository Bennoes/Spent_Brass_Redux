using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileMapBulletHit : MonoBehaviour, IHittable
{
    public event Action OnDeath;
    float IHittable.CurrentArmour { get ; set; }

    void IHittable.OnHit(float damage, Vector2 textPos)
    {
        //Debug.Log("wall hit");
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
