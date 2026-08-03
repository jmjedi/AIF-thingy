using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    //Create player values
    //These will be used to help keep track of what's happening to the player
    Vector3 moveDir;
    public float currentAcceleration = 0f;
    Transform cameraObject;
    Rigidbody plrRigidbody;

    //Set player values that will be used in game
    //This can be changed within the unity workspace to simplify where it is
    //For example, I can make the speed faster by changing movementSpd;
    [Header("Movement Speeds")]
    public float movementSpd = 7;
    public float rotationSpd = 15;
    public float acceleration = 25f;
    public float deceleration = 40f;
    public float jumpSize = 8;

    [Header("Falling Based")]
    public float gravityScale = 2;
    public float rayCastSize = 1f;
    public LayerMask groundLayer;

    [Header("Flags")]
    public bool isGrounded;
    public string State = "Small";

    private void Awake()
    {
        //Grab all the components inside the player
        inputManager = GetComponent<InputManager>();
        plrRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement(bool jumpInput)
    {
        //Handle EVERYTHING that's inside the player
        //This gets handled inside the player manager
        HandleFloorCollision();
        HandleMovement();
        HandleRotation();
        HandleJump(jumpInput);
    }

    private void HandleMovement()
    {
        //Check which direction the player is moving
        moveDir = cameraObject.forward * inputManager.verticalInput;
        moveDir = moveDir + cameraObject.right * inputManager.horizontalInput;
        moveDir.Normalize();
        //Prevent the player from moving up
        moveDir.y = 0;
        //Move the player based on the set speed

        //Set the player's velocity to the move direction
        Vector3 horizontalVel = new Vector3(plrRigidbody.velocity.x, 0, plrRigidbody.velocity.z);

        //Check if the player's input is to a point where we can accelerate
        //Otherwise, it should slow down
        float accelRate = (moveDir * movementSpd).magnitude > 0.01f
            ? acceleration //If Moving
            : deceleration; //Otherwise

        currentAcceleration = accelRate; //For Tracking Reasons
        
        //Smoothly move the player to the target
        horizontalVel = Vector3.MoveTowards(horizontalVel, moveDir * movementSpd, accelRate * Time.fixedDeltaTime);
        plrRigidbody.velocity = new Vector3(horizontalVel.x, plrRigidbody.velocity.y, horizontalVel.z);

        //How this script works is it gets the player's direction
        //and moves the player's position based on where the camera looks
        //at the object.
    }

    private void HandleJump(bool input)
    {
        //We should be able to only jump when we are on the ground
        if (isGrounded)
        {
            //Check if we are pressing the jump input
            if (input)
                //Push the player upwards
                plrRigidbody.AddForce(transform.up * jumpSize, ForceMode.Impulse);
        }

        if (!input && plrRigidbody.velocity.y > 0)
        {
            plrRigidbody.velocity = new Vector3(plrRigidbody.velocity.x, plrRigidbody.velocity.y * 0.5f, plrRigidbody.velocity.z);
        }
    }

    private void HandleRotation()
    {
        //Reset the direction because we are moving somewhere else
        Vector3 targetDir = Vector3.zero;

        //Check which direction the player is looking at
        targetDir = cameraObject.forward * inputManager.verticalInput;
        targetDir = targetDir + cameraObject.right * inputManager.horizontalInput;
        targetDir.Normalize();
        //Prevent the player from looking up
        targetDir.y = 0;

        //If we aren't moving, we should reset the player's direction
        //This helps keep track of where the player was originally looking
        if (targetDir == Vector3.zero)
            //Keep it looking at where the player is looking
            targetDir = transform.forward;

        //Set the target rotation the player should look at
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpd * Time.deltaTime);
        
        //Set player rotation
        transform.rotation = playerRotation;
        
        //This script functions as where the player looks at. The player has a
        //box to show which direction the player is currently looking at, making
        //it more realistic for where the player is facing.
    }

    private void HandleFloorCollision()
    {
        //Raycast a value
        RaycastHit hit;

        //Force the player to be falling down based on the scale
        //This foces the player to go downwards realistically like in real life
        plrRigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        
        //Create a boolean for where it is true if we are touching the ground
        //Raycast works as a look system, where it checks if it is touching something and makes sure where to look for it
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, rayCastSize);
    }
}
