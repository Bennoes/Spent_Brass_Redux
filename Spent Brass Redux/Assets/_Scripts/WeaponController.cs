
using System.Collections.Generic;

using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public GameObject actualShootPoint;
    public GameObject projectile;
    public GameObject DropHolder;
    public Camera cam;

    [HideInInspector] public Vector2[] shootPoints;

    //new weapon logic
    [Header("Assigned Weapon")]
    public WeaponSO assignedWeapon;

    public List<WeaponManager> weaponInventory = new List<WeaponManager>() { null,null };


    public float pickUpRange = 1;


    [HideInInspector] public bool isReloading;
    private float reloadTimer;
    


    public LayerMask weaponLayer;
    private GameObject clickedObject;
    private bool triggerPull;
    private float cycleTimer = 0;

    private float recoilTimer = 0;
    private float currentSpreadFuzz;
    private Vector2 playerToMouse;
    private Vector2 mousePointer;

    public RuntimeAnimatorController weaponAnimationController;
    public Animator weaponAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = Camera.main;

        //weapon wrap
        WeaponWrapUp();

        
        weaponAnimator.runtimeAnimatorController = weaponInventory[0].WeaponData.animationController;

    }

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

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            reloadTimer = weaponInventory[0].WeaponData.reloadTime;
            
        }

        ReloadWeapon(weaponInventory[0].State, weaponInventory[0].WeaponData);


    }

    private void FixedUpdate()
    {
        RotateShootPoints();

        if (cycleTimer > 0) { cycleTimer -= Time.fixedDeltaTime; }

        if (weaponInventory[0].State.currentReloadTime > 0) { weaponInventory[0].State.currentReloadTime -= Time.deltaTime;}

        

    }


    private void ReloadWeapon(WeaponState weaponState, WeaponSO weaponData)
    {
        if (!isReloading)
        {
            return;
        }


        //deal with pressing the button when weapon is full
        if(weaponState.currentAmmoCount == weaponData.maxAmmo)
        {
            Debug.Log("no need to reload");
            isReloading = false;
            reloadTimer = 0;
            return;
        }

        //deal with no ammo left
        if (weaponState.reloadsRemaining == 0 && weaponState.currentAmmoCount == 0)
        {           
            Debug.Log("weapon is out of ammo and reloads");
            DiscardWeapon(weaponState);
            isReloading = false;
            reloadTimer = 0;
            return;
        }

        //deal with no reloads left
        if (weaponState.reloadsRemaining == 0)
        {           
            Debug.Log("No more reloads");
            isReloading = false;
            return;
        }

        //reload one round at a time vs reload a mag
        if (weaponData.sequencialReload)
        {
            //Debug.Log("Sequencial Reload");
            SequencialReload(weaponState, weaponData);
        }
        else
        {
            //Debug.Log("Normal reload");
            NormalReload(weaponState,weaponData);
        }
    }


    private void SequencialReload(WeaponState state, WeaponSO data)
    {
        if(state.currentAmmoCount == data.maxAmmo)
        {
            Debug.Log("fully reloaded");
            isReloading = false;
            reloadTimer = 0;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("reload cancelled");
            isReloading = false;
            reloadTimer = 0;
            return;
        }

        if (reloadTimer <= 0)
        {
            Debug.Log("one round added");
            state.currentAmmoCount ++;
            state.reloadsRemaining--;
            reloadTimer = data.reloadTime;
            
        }
        else
        {
            reloadTimer -= Time.deltaTime;
        }


    }

    private void NormalReload(WeaponState state, WeaponSO data)
    {
        //if reload animation isnt playing - play it
        if(reloadTimer <= 0)
        {
            state.currentAmmoCount = data.maxAmmo;
            state.reloadsRemaining--;
            isReloading = false;
        }
        else
        {
            reloadTimer -= Time.deltaTime;
        }
      
    }



    private void WeaponWrapUp()
    {
        WeaponState weaponState = new WeaponState(assignedWeapon.maxAmmo,int.MaxValue, true);

        WeaponManager playerPrimary = new WeaponManager(assignedWeapon,weaponState);

        weaponInventory[0] = playerPrimary;

    }
    

    // Update is called once per frame
    

    private void PickUpOrSwapWeapon()
    {
        WeaponManager weaponPickUp = CheckForPickUpAndReturn();

        if (weaponPickUp != null)
        {
            Debug.Log("pick up that GUN");
            
             bool pickUpSuccess = CheckInventoryForEmpty(weaponPickUp);

            if (!pickUpSuccess) DiscardAndEquip(weaponPickUp); //run the discard weapon logic

        }
        else
        {
            WeaponSwap();
        }
    }

    private bool CheckInventoryForEmpty(WeaponManager pickedWeapon)
    {        
        for (int i = 0; i < weaponInventory.Count; i++ )
        {
            //Debug.Log("for loop is running");
            if(weaponInventory[i] == null)
            {
                Debug.Log("empty weapon slot at: " + i);
                Debug.Log("so adding " + pickedWeapon.WeaponData.name);
                weaponInventory[i] = pickedWeapon;
                Destroy(clickedObject);
                return true;               
            }           
        }
        return false;      
    }

    private void DiscardAndEquip(WeaponManager pickedWeapon)
    {
        //make sure not to discard main weapon
        //need to search for the secondary with for loop

        for (int i = 0; i < weaponInventory.Count; i++)
        {
            if (!weaponInventory[i].WeaponData.primaryWeapon)
            {
               Debug.Log ("this weapon is not primary. Bin it " + weaponInventory[i].WeaponData.name);

               GameObject tempDropHolder = Instantiate(DropHolder,transform.position,Quaternion.identity);
               PickUpControl tempPickUpControl = tempDropHolder.GetComponent<PickUpControl>();
               tempPickUpControl.heldWeapon = weaponInventory[i];

                weaponInventory[i] = pickedWeapon;

                WeaponSwap();
                Destroy(clickedObject);
            }
        }
    }

    private WeaponManager CheckForPickUpAndReturn()
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
            return pickUp.heldWeapon;           
        }
        else
        {
            return null;
        }
    }


    
    private void ProjectileCreation()
    {

        for (int i = 0; i < weaponInventory[0].WeaponData.shotsPerPull; i++)
        {

            GameObject clonedProjectile = Instantiate(projectile, actualShootPoint.transform.position, Quaternion.identity);
            ProjectileController clonedController = clonedProjectile.GetComponent<ProjectileController>();

            //pass relevant attributes to the newly spawned projectile - more might be added here
            WeaponSO weaponData = weaponInventory[0].WeaponData;
            clonedController.weapon = weaponData;

            currentSpreadFuzz = Mathf.Lerp(weaponData.spreadNominalFuzz, weaponData.maxSpreadFuzz, recoilTimer * weaponData.recoilRate);
            //Debug.Log(currentSpreadFuzz);


            float angleFuzz = Random.Range(-currentSpreadFuzz, currentSpreadFuzz);
            Quaternion rotationZ = Quaternion.Euler(0,0,angleFuzz);


            clonedController.projectileDirection = rotationZ * playerToMouse;
            clonedController.projectileMaxSpeed = weaponInventory[0].WeaponData.projectileSpeed;
            //need to add fuzz to range
            float rangeFuzz = Random.Range(-weaponInventory[0].WeaponData.rangeFuzz, weaponInventory[0].WeaponData.rangeFuzz);
            clonedController.projectileRange = weaponInventory[0].WeaponData.rangeNominal + rangeFuzz;
            clonedController.ProjectileSpeedDistance = weaponInventory[0].WeaponData.ProjectileSpeedDistance;
            clonedController.DamageOverDistance = weaponInventory[0].WeaponData.DamageOverDistance;



        }
        
    }


    private void WeaponFire()
    {
        if(playerMovement.isDashing) { return; }

        var currentWeapon = weaponInventory[0];

        if (currentWeapon == null || currentWeapon.WeaponData == null)
        {
            Debug.Log("No weapon equipped");
            return;
        }

        WeaponState weaponState = currentWeapon.State;
        WeaponSO weaponData = currentWeapon.WeaponData;

        

        // Check ammo availability
        if (weaponState.currentAmmoCount < 1)
        {
            Debug.Log("Out of ammo!");

            if(!isReloading)
            {
                isReloading = true;
                reloadTimer = weaponInventory[0].WeaponData.reloadTime;

            }

            
            //ReloadWeapon(weaponState, weaponData);  

            //HandleOutOfAmmo(weaponState,weaponData);
            return;
        }

        // Check if ready to fire
        if (cycleTimer > 0)
        {
            //Debug.Log("Weapon cooling down");
            return;
        }

        
        // Fire based on weapon type
        ProjectileCreation();
        weaponState.currentAmmoCount--; // Decrement ammo count
        cycleTimer = weaponData.cycleRate;

        // Handle semi-auto trigger reset
        if (!weaponData.FullAuto)
        {
            triggerPull = false;
        }
    }

    // Handles behaviour when out of ammo
    private void HandleOutOfAmmo(WeaponState weaponState, WeaponSO weaponData)
    {
        //no reloads left discard weapon and return
        if(weaponState.reloadsRemaining == 0)
        {
            Debug.Log("No mags left");
            DiscardWeapon(weaponState);
            return;
        }

        //gun is ready to fire again
        if(weaponState.isReloading && weaponState.currentReloadTime <=0)
        {
            Debug.Log("Reloading");
            weaponState.currentAmmoCount = weaponData.maxAmmo;

            if (!weaponState.primaryWeapon) weaponState.reloadsRemaining--;
            weaponState.isReloading = false;
            return;
        }
        
        if( weaponState.isReloading && weaponState.currentReloadTime > 0)
        {
            
            Debug.Log("counting down to reload: " + weaponState.currentReloadTime);
            return;
        }

        if(!weaponState.isReloading)
        {
            Debug.Log("set up reload bool and time");
            weaponState.isReloading = true;
            weaponState.currentReloadTime = weaponData.reloadTime;
        }
     
        
    }


    private void DiscardWeapon(WeaponState state)
    {
        Debug.Log("discard weapon");
        if (state.primaryWeapon)
        {
            Debug.Log("can't discard primary");
            return;
        }

        WeaponSwap();

        weaponInventory[1] = null;

    }

    private void WeaponSwap()
    {
        if (weaponInventory[0] != null && weaponInventory[1] != null)
        {
            Debug.Log("weapon swapped");
            (weaponInventory[0], weaponInventory[1]) = (weaponInventory[1], weaponInventory[0]);
            //swap out the animation contoller 
            weaponAnimator.runtimeAnimatorController = weaponInventory[0].WeaponData.animationController;

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

        if (weaponInventory[0] == null || weaponInventory[0].WeaponData.shootPoints.Length < 8)
        {
            Debug.Log("Current weapon is null or shootpoints are unasigned");
        }
        else
        {

            float highestDot = -Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < weaponInventory[0].WeaponData.shootPoints.Length; i++)
            {
                Vector2 normalisedShootPoint = weaponInventory[0].WeaponData.shootPoints[i];
                normalisedShootPoint.Normalize();
                float dotProduct = Vector2.Dot(normalisedShootPoint, playerToMouse);
                if (dotProduct > highestDot)
                {
                    highestDot = dotProduct;
                    closestIndex = i;


                }


            }

            actualShootPoint.transform.localPosition = (Vector3)weaponInventory[0].WeaponData.shootPoints[closestIndex];
            //make the animtor move teh gun to the correct rotation:
            Vector2 gunAnimXandY = weaponInventory[0].WeaponData.shootPoints[closestIndex];
            gunAnimXandY.Normalize();
            weaponAnimator.SetFloat("Horizontal", gunAnimXandY.x);
            weaponAnimator.SetFloat("Vertical", gunAnimXandY.y);
        }


    }
}
