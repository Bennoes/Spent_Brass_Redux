using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Vector2[] shootPoints;

    public WeaponSO CurrentWeapon;
    public WeaponSO StoredWeapon;
    public GameObject actualShootPoint;
    public GameObject projectile;

    public Camera cam;

    private Vector2 playerToMouse;
    private Vector2 mousePointer;

    // Start is called before the first frame update
    void Start()
    {
        if(cam == null) cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //trigger pull check
        if (Input.GetMouseButtonDown(0))
        {
            WeaponFire();
        }

        if (Input.GetMouseButtonDown(1))
        {
            WeaponSwap();
        }
        


    }

    private void FixedUpdate()
    {
        RotateShootPoints();
    }


    private void WeaponFire()
    {
        if (CurrentWeapon != null)
        {
            
            GameObject clonedProjectile =  Instantiate(projectile,actualShootPoint.transform.position,Quaternion.identity);
            ProjectileController clonedController = clonedProjectile.GetComponent<ProjectileController>();
            //pass relevant attributes to the newly spawned projectile
            clonedController.projectileDirection = playerToMouse;
            clonedController.projectileSpeed = CurrentWeapon.projectileSpeed;
            clonedController.projectileRange = CurrentWeapon.range;
            
        }
        else
        {
            Debug.Log("No weapon equiped");
        }


    }

    private void WeaponSwap()
    {
        if (CurrentWeapon != null && StoredWeapon !=null)
        {
            Debug.Log("weapon swapped");

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

        playerToMouse = mousePointer - (Vector2) gameObject.transform.position;
        playerToMouse.Normalize();
        
        if(CurrentWeapon == null || CurrentWeapon.shootPoints.Length < 8)
        {
            Debug.Log("Current weapon is null or shootpoints are unasigned");
        }
        else
        {
            
            float highestDot = -Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < CurrentWeapon.shootPoints.Length; i++)
            {
                Vector2 normalisedShootPoint = CurrentWeapon.shootPoints[i];
                normalisedShootPoint.Normalize();
                float dotProduct = Vector2.Dot(normalisedShootPoint, playerToMouse);
                if (dotProduct > highestDot)
                {
                    highestDot = dotProduct;
                    closestIndex = i;
                }
                

            }

            actualShootPoint.transform.localPosition = (Vector3)CurrentWeapon.shootPoints[closestIndex];

        }


    }
}
