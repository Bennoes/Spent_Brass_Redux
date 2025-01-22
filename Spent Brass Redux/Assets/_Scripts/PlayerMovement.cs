using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    Vector2 playerToMouse, mousePointer;
    public Animator playerAnimator;



    private Vector2 movementVector;

    public float playerSpeed = 2;
    private Vector2 playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //updates player transform position
        playerMovement = playerSpeed * Time.deltaTime * movementVector;
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

        //get info from mouse position
        
    }


}


public enum PlayerState
{





}
