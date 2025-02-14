using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour
{
    public TargetControl control;
    // Start is called before the first frame update
    public WeaponSO assignedWeapon;
    public int maxMags;

    public GameObject pickUp;

    private void Update()
    {
        if(control.HitPoints <= 0)
        {
            GameObject newPickup = Instantiate(pickUp, this.transform.position, Quaternion.identity);

            PickUpControl pickUpControl = newPickup.GetComponent<PickUpControl>();
            pickUpControl.maxMags = maxMags;
            pickUpControl.assignedWeapon = assignedWeapon;
        }
    }



}
