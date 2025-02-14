using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetControl : MonoBehaviour, IHittable
{
    public GameObject hitText;
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

    public void OnHit(float damage, Vector2 textPos)
    {
        
        Debug.Log(gameObject.name + " was hit");
        HitPoints -= damage;
        //Debug.Log($"hit points remaing: {HitPoints}");

        Vector2 newTextPos = Utilities.Fuzz(textPos, 0.5f);
        
        GameObject thisHitText = Instantiate(hitText, newTextPos,Quaternion.identity);
        HitTextController thisTextControl = thisHitText.GetComponent<HitTextController>();
        thisTextControl.damage = damage;
        
    }

   
}
