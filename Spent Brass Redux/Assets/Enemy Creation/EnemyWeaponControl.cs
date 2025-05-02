using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyWeaponControl : EnemyPartControl
{
    public GameObject shootPoint;
    public GameObject ejectPoint;

    public GameObject projectile;


    public WeaponSO weaponData;
    public Animator animator;
    [HideInInspector] public EnemyBaseControl enemyBaseControl;

    WeaponUtils weaponUtils;

    private float recoilTimer = 0;
    private float currentSpreadFuzz;

    public float cycleTimer = 0;
    public int currentAmmoCount;

    //[HideInInspector] public EnemyParts arms = EnemyParts.ARMS;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmoCount = weaponData.maxAmmo;
    }

    private void Awake()
    {
        weaponUtils = new WeaponUtils();
    }

    // Update is called once per frame
    void Update()
    {
        if(cycleTimer > 0)
        {
            cycleTimer -= Time.deltaTime;
        }
        else
        {
            cycleTimer = 0;
        }


        if (animator != null)
        {
            //RotateShootPoints(enemyBaseControl.travelDirection);

            //animator.SetFloat("Horizontal", enemyBaseControl.travelDirection.x);
            //animator.SetFloat("Vertical", enemyBaseControl.travelDirection.y);

        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            //test logic
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            GameObject playerG = player.gameObject;

            ShootAt(playerG.transform.position);
        }




        
    }

    public void RotateShootPoints(Vector2 aimDirection)
    {  

        weaponUtils.SetShootAndEjectionPoints(weaponData,aimDirection, shootPoint, ejectPoint,animator);
    }

    public void ShootAt(Vector2 target)
    {      
        Vector2  pointToTarget = target - (Vector2)shootPoint.transform.position;
        pointToTarget.Normalize();
        
        for (int i = 0; i < weaponData.shotsPerPull; i++)
        {
            GameObject projectileInstance = Instantiate(projectile, shootPoint.transform.position, Quaternion.identity);
            ProjectileController controllerInstance = projectileInstance.GetComponent<ProjectileController>();

            controllerInstance.weapon = weaponData;

            currentSpreadFuzz = Mathf.Lerp(weaponData.spreadNominalFuzz, weaponData.maxSpreadFuzz, recoilTimer * weaponData.recoilRate);
            //Debug.Log(currentSpreadFuzz);


            float angleFuzz = UnityEngine.Random.Range(-currentSpreadFuzz, currentSpreadFuzz);
            Quaternion rotationZ = Quaternion.Euler(0, 0, angleFuzz);

            controllerInstance.enemyBullet = true;
            controllerInstance.projectileDirection = rotationZ * pointToTarget;
            controllerInstance.projectileMaxSpeed = weaponData.projectileSpeed;
            controllerInstance.isPiercing = weaponData.piercingAmmo;
            //need to add fuzz to range
            float rangeFuzz = UnityEngine.Random.Range(-weaponData.rangeFuzz, weaponData.rangeFuzz);
            controllerInstance.projectileRange = weaponData.rangeNominal + rangeFuzz;
            controllerInstance.ProjectileSpeedDistance = weaponData.ProjectileSpeedDistance;
            controllerInstance.DamageOverDistance = weaponData.DamageOverDistance;

            Debug.Log("cycle time added " + weaponData.cycleRate);
            
            currentAmmoCount--;
        }
        cycleTimer += weaponData.cycleRate;
    }
}
