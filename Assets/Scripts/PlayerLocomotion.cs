using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    //Create player values
    //These will be used to help keep track of what's happening to the player
    Vector3 moveDir;
    Transform cameraObject;
    Rigidbody plrRigidbody;

    //Set player values that will be used in game
    //This can be changed within the unity workspace to simplify where it is
    //For example, I can make the speed faster by changing movementSpd;
    [Header("Movement Speeds")]
    public float movementSpd = 7;
    public float rotationSpd = 15;

    [Header("Falling Based")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallSpd = 8;
    public LayerMask groundLayer;

    [Header("Flags")]
    public bool isGrounded;

    private void Awake()
    {
        //Grab all the components inside the player
        inputManager = GetComponent<InputManager>();
        plrRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        //Handle EVERYTHING that's inside the player
        //This gets handled inside the player manager
        HandleMovement();
        HandleRotation();
        HandleFloorCollision();
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
        moveDir = moveDir * movementSpd;

        //Set the player's velocity to the move direction
        Vector3 movementVel = moveDir;
        plrRigidbody.velocity = movementVel;

        //How this script works is it gets where the player is looking
        //and moves the player's position based on where the camera looks
        //at the object.
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
        //box to show which directiont he player is currently looking at, making
        //it more realistic for where the player is facing.
    }

    private void HandleFloorCollision()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;

        if (!isGrounded)
        {
            inAirTimer += Time.deltaTime;
            plrRigidbody.AddForce(transform.forward * leapingVelocity);
            plrRigidbody.AddForce(-Vector3.up * fallSpd * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
