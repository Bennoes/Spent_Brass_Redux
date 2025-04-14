using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropper : MonoBehaviour
{
    
    public IHittable control;
    // Start is called before the first frame update
    public WeaponSO assignedWeapon;
    public int maxMags;

    public GameObject pickUp;

    private void Start()
    {

        control = GetComponent<IHittable>();
        if (control != null)
        {
            control.OnDeath += SpawnPickup;
        }

    }

    

    private void SpawnPickup()
    {
        //Debug.Log("spawning weapon pick up");
        GameObject newPickup = Instantiate(pickUp, this.transform.position, Quaternion.identity);

        PickUpControl pickUpControl = newPickup.GetComponent<PickUpControl>();
        pickUpControl.maxMags = maxMags;
        pickUpControl.assignedWeapon = assignedWeapon;
    }



}
