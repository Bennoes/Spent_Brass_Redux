using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    
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
