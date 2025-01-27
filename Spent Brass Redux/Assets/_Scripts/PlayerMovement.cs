using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    Vector2 playerToMouse, mousePointer;
    public Animator playerAnimator;



    private Vector2 movementVector;

    private float actualSpeed = 2;
    public float playerSpeed = 2;
    public float dashMultiplier = 1.5f;
    public bool isDashing;

    private Vector2 playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //updates player transform position
        playerMovement = actualSpeed * Time.deltaTime * movementVector;
        gameObject.transform.position += (Vector3)playerMovement;



        mousePointer = cam.ScreenToWorldPoint(Input.mousePosition);

        playerToMouse = mousePointer - (Vector2)gameObject.transform.position;
        playerToMouse.Normalize();

        playerAnimator.SetFloat("Horizontal", playerToMouse.x);
        playerAnimator.SetFloat("Vertical", playerToMouse.y);
    }

    void FixedUpdate()
    {
        //Gets info from arrow or wasd keys
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.y = Input.GetAxis("Vertical");
        movementVector.Normalize();

        if(Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("dashing");
            actualSpeed = playerSpeed * dashMultiplier;
            isDashing = true;
        }
        else
        {
            
            actualSpeed = playerSpeed;
            isDashing = false;
        }
        

        //get info from mouse position
        
    }


}


public enum PlayerState
{





}
