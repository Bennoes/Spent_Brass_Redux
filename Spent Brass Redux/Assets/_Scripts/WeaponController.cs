
using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //delegate/event to let UI know weapons are changing

    public static event Action OnWeaponUpdate;
    public static event Action SetUpMag;
    public static event Action DepleteByOne;
    public static event Action ReloadMag;
    public static event Action ReloadOneRound;
    public static event Action OnInitialSetUp;



    public PlayerMovement playerMovement;

    public GameObject actualShootPoint;
    public GameObject ejectionPoint;
    public GameObject spentAmmo;
    public GameObject gunPiv;
    


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


    WeaponUtils weaponUtils;


    private void Awake()
    {
        weaponUtils = new WeaponUtils();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = Camera.main;

        //weapon wrap
        WeaponWrapUp();

        
        weaponAnimator.runtimeAnimatorController = weaponInventory[0].WeaponData.animationController;
        SetUpMag?.Invoke();
        SetWeaponPivot();

        OnInitialSetUp?.Invoke();
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
            //Debug.Log("fully reloaded");
            isReloading = false;
            reloadTimer = 0;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("reload cancelled");
            
            reloadTimer = 0;
            cycleTimer =  weaponInventory[0].WeaponData.reloadTime;
            isReloading = false;
            return;
        }

        if (reloadTimer <= 0)
        {
            //Debug.Log("one round added");
            state.currentAmmoCount ++;
            state.reloadsRemaining--;
            reloadTimer = data.reloadTime;

            ReloadOneRound?.Invoke();
            
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
            ReloadMag?.Invoke();
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
            //Debug.Log("pick up that GUN");
            
             bool pickUpSuccess = CheckInventoryForEmpty(weaponPickUp);

            if (!pickUpSuccess)
            {
                DiscardAndEquip(weaponPickUp);
            }
            else
            {
                //if there is a space in the inventory "checkInventory" will assign it. We now need to swap to it
                //might be better assigning it here rather than in the checkInvemtory method
                WeaponSwap();
            }

        }
        else
        {
            WeaponSwap();
        }

        OnWeaponUpdate?.Invoke();
    }

    private bool CheckInventoryForEmpty(WeaponManager pickedWeapon)
    {        
        for (int i = 0; i < weaponInventory.Count; i++ )
        {
            //Debug.Log("for loop is running");
            if(weaponInventory[i] == null)
            {               
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

                GameObject tempDropHolder = Instantiate(DropHolder, transform.position, Quaternion.identity);
                PickUpControl tempPickUpControl = tempDropHolder.GetComponent<PickUpControl>();
                tempPickUpControl.heldWeapon = weaponInventory[i];

                weaponInventory[i] = pickedWeapon;

                Destroy(clickedObject);
            }
        }

        if (weaponInventory[0].WeaponData.primaryWeapon)
        {
            WeaponSwap();
        }
        else
        {
            weaponAnimator.runtimeAnimatorController = weaponInventory[0].WeaponData.animationController;
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

        WeaponSO weaponData = weaponInventory[0].WeaponData;

        for (int i = 0; i < weaponData.shotsPerPull; i++)
        {

            GameObject clonedProjectile = Instantiate(projectile, actualShootPoint.transform.position, Quaternion.identity);
            ProjectileController clonedController = clonedProjectile.GetComponent<ProjectileController>();

            //pass relevant attributes to the newly spawned projectile - more might be added here
            
            clonedController.weapon = weaponData;

            currentSpreadFuzz = Mathf.Lerp(weaponData.spreadNominalFuzz, weaponData.maxSpreadFuzz, recoilTimer * weaponData.recoilRate);
            //Debug.Log(currentSpreadFuzz);


            float angleFuzz = UnityEngine.Random.Range(-currentSpreadFuzz, currentSpreadFuzz);
            Quaternion rotationZ = Quaternion.Euler(0,0,angleFuzz);


            clonedController.projectileDirection = rotationZ * playerToMouse;
            clonedController.projectileMaxSpeed = weaponData.projectileSpeed;
            clonedController.isPiercing = weaponData.piercingAmmo;
            //need to add fuzz to range
            float rangeFuzz = UnityEngine.Random.Range(-weaponData.rangeFuzz, weaponData.rangeFuzz);
            clonedController.projectileRange = weaponData.rangeNominal + rangeFuzz;
            clonedController.ProjectileSpeedDistance = weaponData.ProjectileSpeedDistance;
            clonedController.DamageOverDistance = weaponData.DamageOverDistance;



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
        if (weaponState.currentAmmoCount < 1 && weaponData.sequencialReload)
        {
            //Debug.Log("Out of ammo!");

            if (!isReloading)
            {
                isReloading = true;
                reloadTimer = weaponData.reloadTime;

            }

            return;
        }

        // Check if ready to fire
        if (cycleTimer > 0)
        {
            
            return;
        }

        if (isReloading) return;
        // Fire based on weapon type
        ProjectileCreation();

        EjectSpentAmmo();

        weaponState.currentAmmoCount--; // Decrement ammo count
        cycleTimer = weaponData.cycleRate;
        DepleteByOne?.Invoke();

        if (weaponState.currentAmmoCount < 1)
        {
            //Debug.Log("Out of ammo!");

            if (!isReloading)
            {
                isReloading = true;
                reloadTimer = weaponData.reloadTime;

            }

            return;
        }

        // Handle semi-auto trigger reset
        if (!weaponData.FullAuto)
        {
            triggerPull = false;
        }
    }

    // Handles behaviour when out of ammo

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
        OnWeaponUpdate?.Invoke();
    }



    private void WeaponSwap()
    {

        if (weaponInventory[0] != null && weaponInventory[1] != null) //carrying two weapons
        {
            //Debug.Log("weapon swapped");
            (weaponInventory[0], weaponInventory[1]) = (weaponInventory[1], weaponInventory[0]);
            //swap out the animation contoller 
            weaponAnimator.runtimeAnimatorController = weaponInventory[0].WeaponData.animationController;
            SetUpMag?.Invoke();
            SetWeaponPivot();
        }
        else
        {
            //Debug.Log("No weapon to swap to");
        }
        OnWeaponUpdate?.Invoke();
    }

    private void EjectSpentAmmo()
    {
        WeaponSO weaponData = weaponInventory[0].WeaponData;
              
        Instantiate(weaponData.SpentAmmo, ejectionPoint.transform.position,Quaternion.identity);
                  
    }

    private void RotateShootPoints()
    {
        mousePointer = cam.ScreenToWorldPoint(Input.mousePosition);

        playerToMouse = mousePointer - (Vector2)gunPiv.transform.position;
        playerToMouse.Normalize();

        weaponUtils.SetShootAndEjectionPoints(weaponInventory[0].WeaponData,playerToMouse, actualShootPoint,ejectionPoint,weaponAnimator);


        /*
        //check player to mouse vector against list of shoot points
        //and choose closest match
        mousePointer = cam.ScreenToWorldPoint(Input.mousePosition);

        playerToMouse = mousePointer - (Vector2)gunPiv.transform.position;
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
                //use this to determine which ejection point to use too
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

            ejectionPoint.transform.localPosition = (Vector3)weaponInventory[0].WeaponData.ejectionPoints [closestIndex];

            //make the animtor move teh gun to the correct rotation:
            Vector2 gunAnimXandY = weaponInventory[0].WeaponData.shootPoints[closestIndex];
            gunAnimXandY.Normalize();
            weaponAnimator.SetFloat("Horizontal", gunAnimXandY.x);
            weaponAnimator.SetFloat("Vertical", gunAnimXandY.y);
        }
        */

    }

    private void SetWeaponPivot()   
    {
        Vector2[] shootPoints = weaponInventory[0].WeaponData.shootPoints;
        Vector2 pivot = (shootPoints[0] + shootPoints[1] + shootPoints[2] + shootPoints[3] + shootPoints[4]
                         + shootPoints[5] + shootPoints[6] + shootPoints[7]) / 8;

        gunPiv.transform.localPosition = pivot;


    }
}
