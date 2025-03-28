using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetControl : MonoBehaviour, IHittable
{
    public GameObject hitText;
    public float CurrentArmour { get; set; }

    public float MaxHitPoints;

    public event Action OnDeath;




    // Start is called before the first frame update
    void Start()
    {
        CurrentArmour = MaxHitPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Die()
    {

        OnDeath?.Invoke();

        Destroy(gameObject);

    }

    

    public void OnHit(float damage, Vector2 textPos)
    {
        
        Debug.Log(gameObject.name + " was hit");
        CurrentArmour -= damage;
        //Debug.Log($"hit points remaing: {HitPoints}");

        Vector2 newTextPos = Utilities.Fuzz(textPos, 0.5f);
        
        GameObject thisHitText = Instantiate(hitText, newTextPos,Quaternion.identity);
        HitTextController thisTextControl = thisHitText.GetComponent<HitTextController>();
        thisTextControl.damage = damage;

        if(CurrentArmour <= 0)
        {
            Die();
        }
        
    }

   
}
