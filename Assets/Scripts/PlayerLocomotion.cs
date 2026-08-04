using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics; buzz off bro
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    //Create player values
    //These will be used to help keep track of what's happening to the player
    Vector3 moveDir; //Current Direciton of player
    Transform cameraObject; //Player camera
    Rigidbody plrRigidbody; //Player's rigid body (Movement)
    CapsuleCollider hitbox; // Player's hitbox

    //Get Player Models
    public List<GameObject> modelList = new List<GameObject>();
    public LayerMask layerMask;

    //Set player values that will be used in game
    //This can be changed within the unity workspace to simplify where it is
    //For example, I can make the speed faster by changing movementSpd;

    //Non-public Variabes
    float currentAcceleration = 0f;

    //Public Variables
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

    [Header("Flags & States")]
    public bool isGrounded;
    public bool canJump;
    public string State = "Small";

    private void Awake()
    {
        //Grab all the components inside the player
        inputManager = GetComponent<InputManager>();
        plrRigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponent<CapsuleCollider>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement(bool jumpInput)
    {
        //Handle EVERYTHING that's inside the player
        //This gets handled inside the plzzayer manager
        HandleState();
        HandleFloorCollision();
        newFloorAlign();
        HandleMovement();
        HandleRotation();
        HandleJump(jumpInput);
    }

    private void HandleState()
    {
        
        if (State == "S")
            hitbox.height = 1.214885f;
        else
            hitbox.height = 2.005388f;
        
        //Check if there is any models that are in the modelList
        //This is to prevent any errors that may occur if I have no models
        if (modelList.Count <= 0) return;
        //Make the character size visible based on what state the player is
        modelList[0].GetComponent<MeshRenderer>().enabled = (State == "S");
        modelList[1].GetComponent<MeshRenderer>().enabled = (State == "B");
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
                if (State == "S")
                    plrRigidbody.AddForce(transform.up * (jumpSize / 3.2f), ForceMode.Impulse);
                else
                    plrRigidbody.AddForce(transform.up * jumpSize, ForceMode.Impulse);
                canJump = false;
        }

        //Check if we have only tapped the jump button
        if (!input && plrRigidbody.velocity.y > 0)
        {
            //Bring player downwards
            plrRigidbody.velocity = new Vector3(plrRigidbody.velocity.x, plrRigidbody.velocity.y * 0.5f, plrRigidbody.velocity.z);
        }

        //Check if we aren't holding the jump button, otherwise we can't jump
        if (!input && isGrounded)
            canJump = true;
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

    private void newFloorAlign() //UNUSED
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.1f, layerMask))
            transform.up = Vector3.Slerp(transform.up, hit.normal, 0.8f);
        else
            transform.up = Vector3.up;
    }
}
