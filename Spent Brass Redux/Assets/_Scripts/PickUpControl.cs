using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpControl : MonoBehaviour
{
    public WeaponSO assignedWeapon;
    public int maxMags = 3;


    public WeaponManager heldWeapon;
    // Start is called before the first frame update
    void Start()
    {
        //tempory use until enemies start dropping weapons
        if (assignedWeapon != null)
        {
            WeaponWrapUp();

        }
        else
        {
            Debug.Log("weapon not assigned on " + this.name);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void WeaponWrapUp()
    {
        int numberOfMags = Random.Range(1, maxMags);

        WeaponState weaponState = new WeaponState(assignedWeapon.maxAmmo, numberOfMags, false);

        WeaponManager weaponHolder = new WeaponManager(assignedWeapon, weaponState);

        heldWeapon  = weaponHolder;

    }




}


