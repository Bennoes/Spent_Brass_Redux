
using System.Collections.Generic;

using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Vector2[] shootPoints;

    
    public List<WeaponSO> storedWeapons = new List<WeaponSO>() { null,null};

    public float pickUpRange = 1;

    public GameObject actualShootPoint;
    public GameObject projectile;
    public GameObject DropHolder;

    public Camera cam;

    private Vector2 playerToMouse;
    private Vector2 mousePointer;


    public LayerMask weaponLayer;
    private GameObject clickedObject;
    private bool triggerPull;
    private float cycleTimer = 0;

    private float recoilTimer = 0;
    private float currentSpreadFuzz;
    
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = Camera.main;
       
    }

    

    // Update is called once per frame
    void Update()
    {
        
        //trigger pull check
        if (Input.GetMouseButtonDown(0)) 
        {         
            triggerPull = true;

        }

        if (Input.GetMouseButtonUp(0))
        {
            triggerPull = false;
            recoilTimer = 0;
            
        }

        if (triggerPull)
        {
            WeaponFire();
            recoilTimer += Time.deltaTime;
            
        }


        if (Input.GetMouseButtonDown(1))
        {
            PickUpOrSwapWeapon();
            
          
        }
    }

    private void PickUpOrSwapWeapon()
    {
        WeaponSO weaponPickUp = CheckForPickUpAndReturn();

        if (weaponPickUp != null)
        {
            Debug.Log("pick up that GUN");
            //check inventory for empty slot
            //eject non perm weapon
            //assign new weapon to empty slot
             bool pickUpSuccess = CheckInventoryForEmpty(weaponPickUp);

            if (!pickUpSuccess) DiscardAndEquip(weaponPickUp); //run the discard weapon logic

        }
        else
        {
            WeaponSwap();
        }
    }

    private bool CheckInventoryForEmpty(WeaponSO pickedWeapon)
    {        
        for (int i = 0; i < storedWeapons.Count; i++ )
        {
            Debug.Log("for loop is running");
            if(storedWeapons[i] == null)
            {
                Debug.Log("empty weapon slot at: " + i);
                Debug.Log("so adding " + pickedWeapon.name);
                storedWeapons[i] = pickedWeapon;
                Destroy(clickedObject);
                return true;               
            }           
        }
        return false;      
    }

    private void DiscardAndEquip(WeaponSO pickedWeapon)
    {
        //make sure not to discard main weapon
        //need to search for the secondary with for loop

        for (int i = 0; i < storedWeapons.Count; i++)
        {
            if (!storedWeapons[i].primaryWeapon)
            {
               Debug.Log ("this weapon is not primary. Bin it " + storedWeapons[i].name);

               GameObject tempDropHolder = Instantiate(DropHolder,transform.position,Quaternion.identity);
               PickUpControl tempPickUpControl = tempDropHolder.GetComponent<PickUpControl>();
               tempPickUpControl.weapon = storedWeapons[i];

                storedWeapons[i] = pickedWeapon;
                Destroy(clickedObject);
            }
        }
    }

    private WeaponSO CheckForPickUpAndReturn()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //raycast at the mouse position
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, weaponLayer);
        float playerToPickUpDistance = Vector2.Distance(this.transform.position, mousePosition);
        //will need to check weapon pick up is in range
        if(hit.collider != null && playerToPickUpDistance < pickUpRange)
        {
            clickedObject = hit.collider.gameObject;
            //need to get the weaponSO stored in the controller script on the clicked gameobject
            PickUpControl pickUp = clickedObject.GetComponent<PickUpControl>();
            return pickUp.weapon;           
        }
        else
        {
            return null;
        }
    }


    private void FixedUpdate()
    {
        RotateShootPoints();

        if(cycleTimer > 0) { cycleTimer -= Time.fixedDeltaTime; }

    }
    private void ProjectileCreation()
    {

        for (int i = 0; i < storedWeapons[0].shotsPerPull; i++)
        {

            GameObject clonedProjectile = Instantiate(projectile, actualShootPoint.transform.position, Quaternion.identity);
            ProjectileController clonedController = clonedProjectile.GetComponent<ProjectileController>();

            //pass relevant attributes to the newly spawned projectile - more might be added here
            WeaponSO weapon = storedWeapons[0];
            clonedController.weapon = weapon;

            currentSpreadFuzz = Mathf.Lerp(weapon.spreadNominalFuzz, weapon.maxSpreadFuzz, recoilTimer * weapon.recoilRate);
            //Debug.Log(currentSpreadFuzz);


            float angleFuzz = Random.Range(-currentSpreadFuzz, currentSpreadFuzz);
            Quaternion rotationZ = Quaternion.Euler(0,0,angleFuzz);


            clonedController.projectileDirection = rotationZ * playerToMouse;
            clonedController.projectileSpeed = storedWeapons[0].projectileSpeed;
            //need to add fuzz to range
            float rangeFuzz = Random.Range(-storedWeapons[0].rangeFuzz, storedWeapons[0].rangeFuzz);
            clonedController.projectileRange = storedWeapons[0].rangeNominal + rangeFuzz;
        }      
    }

    private void WeaponFire()
    {
        if (storedWeapons[0] != null && storedWeapons[0].FullAuto && cycleTimer <= 0)
        {
            //full auto version
            ProjectileCreation();
            
            cycleTimer = storedWeapons[0].cycleRate;

        }
        else if (storedWeapons[0] != null && !storedWeapons[0].FullAuto && cycleTimer <= 0)
        {            
            //semi auto version
            ProjectileCreation();
            cycleTimer = storedWeapons[0].cycleRate;
            triggerPull = false;

        }
        else
        {
            //Debug.Log("No weapon equiped");
        }


    }

    private void AccuracyFuzzCalculator()
    {
        float actualSpread = 1;    //needs to be a comniation of nominal spread and recoil spread
        

        float angleFuzz = Random.Range(-actualSpread, actualSpread);

        //calculate recoil multiplier
        Quaternion rotationZ = Quaternion.Euler(0, 0, angleFuzz);



    }


    private void WeaponSwap()
    {
        if (storedWeapons[0] != null && storedWeapons[1] != null)
        {
            Debug.Log("weapon swapped");
            (storedWeapons[0], storedWeapons[1]) = (storedWeapons[1], storedWeapons[0]);

        }
        else
        {
            Debug.Log("No weapon to swap to");
        }
    }



    private void RotateShootPoints()
    {
        //check player to mouse vector against list of shoot points
        //and choose closest match
        mousePointer = cam.ScreenToWorldPoint(Input.mousePosition);

        playerToMouse = mousePointer - (Vector2)gameObject.transform.position;
        playerToMouse.Normalize();

        if (storedWeapons[0] == null || storedWeapons[0].shootPoints.Length < 8)
        {
            Debug.Log("Current weapon is null or shootpoints are unasigned");
        }
        else
        {

            float highestDot = -Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < storedWeapons[0].shootPoints.Length; i++)
            {
                Vector2 normalisedShootPoint = storedWeapons[0].shootPoints[i];
                normalisedShootPoint.Normalize();
                float dotProduct = Vector2.Dot(normalisedShootPoint, playerToMouse);
                if (dotProduct > highestDot)
                {
                    highestDot = dotProduct;
                    closestIndex = i;
                }


            }

            actualShootPoint.transform.localPosition = (Vector3)storedWeapons[0].shootPoints[closestIndex];

        }


    }
}
