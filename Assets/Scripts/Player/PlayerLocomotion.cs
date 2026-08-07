using System;
using System.Collections;
using System.Collections.Generic;
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

    //Set player values that will be used in game
    //This can be changed within the unity workspace to simplify where it is
    //For example, I can make the speed faster by changing movementSpd;

    //Public Variables
    [Header("Movement Speeds")]
    public float movementSpd = 7;
    public float rotationSpd = 15;
    public float acceleration = 25f;
    public float deceleration = 40f;
    public float jumpSize = 8;
    public float currentAcceleration = 0f; //DEBUG
    
    [Header("Falling Based")]
    public float gravityScale = 2;
    public float rayCastSize = 1f;

    [Header("Flags & States")]
    public bool isGrounded; //DEBUG
    public bool isTouchingRoof; //DEBUG
    public bool canJump; //DEBUG
    public bool isCrouch; //DEBUG
    public string State = "S";

    [Header("DEBUG")]
    public Vector3 normal = Vector3.up;
    private Vector3 LastDir;
    public float Angle;

    private void Awake()
    {
        //Grab all the components inside the player
        inputManager = GetComponent<InputManager>();
        plrRigidbody = GetComponent<Rigidbody>();
        hitbox = GetComponent<CapsuleCollider>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement(bool jumpInput, bool crouchInput)
    {
        //Handle EVERYTHING that's inside the player
        //This gets handled inside the plzzayer manager
        HandleAllCollision(); //ALWAYS at the top to prevent bugs
        HandleState();
        HandleMovement();
        HandleRotation();
        HandleJump(jumpInput);
        HandleCrouch(crouchInput);
    }

    private void HandleState()
    {
        //Check if we are in the small state
        //If so, we should have a smaller hitbox
        if (State == "S" || isCrouch)
            hitbox.height = 1.214885f; //Small Hitbox
        else
            hitbox.height = 2.005388f; //Big Hitbox
        
        //Check if there is any models that are in the modelList
        //This is to prevent any errors that may occur if I have no models
        if (modelList.Count <= 0) return;
        //Make the character size visible based on what state the player is
        modelList[0].GetComponent<MeshRenderer>().enabled = (State == "S");
        modelList[1].GetComponent<MeshRenderer>().enabled = (State == "B");
        modelList[2].GetComponent<MeshRenderer>().enabled = (State == "F");
        modelList[3].GetComponent<MeshRenderer>().enabled = (State == "BS");
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
        
        //Smoothly move the player to the target
        if (isCrouch)
            horizontalVel = Vector3.MoveTowards(horizontalVel / 1.2f, moveDir * movementSpd, (accelRate / 1.1f) * Time.fixedDeltaTime);
        else
            horizontalVel = Vector3.MoveTowards(horizontalVel, moveDir * movementSpd, accelRate * Time.fixedDeltaTime);

        plrRigidbody.velocity = new Vector3(horizontalVel.x, plrRigidbody.velocity.y, horizontalVel.z);
        currentAcceleration = horizontalVel.x;
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
            if (input && !isTouchingRoof)
            {
                isCrouch = false;
                normal = Vector3.up;
                //Push the player upwards
                if (State == "S" || isCrouch)
                    plrRigidbody.AddForce(transform.up * (jumpSize / 3.2f), ForceMode.Impulse);
                else
                    plrRigidbody.AddForce(transform.up * jumpSize, ForceMode.Impulse);
                canJump = false;
            }
        }

        //Check if we have only tapped the jump button
        if (!input && plrRigidbody.velocity.y > 0)
            //Bring player downwards
            plrRigidbody.velocity = new Vector3(plrRigidbody.velocity.x, plrRigidbody.velocity.y * 0.5f, plrRigidbody.velocity.z);

        //Check if we aren't holding the jump button, otherwise we can't jump
        if (!input && isGrounded)
            canJump = true;
    }

    private void HandleCrouch(bool crouchInput)
    {
        //We should only be able to crouch on the ground
        if (isGrounded)
        {
            if (crouchInput)
                isCrouch = true;
            else
                isCrouch = false;
        }
    }

    private void HandleRotation()
    {
        //Reset the direction because we are moving somewhere else
        Vector3 targetDir = Vector3.zero;

        //Check which direction the player is looking at
        targetDir = cameraObject.forward * inputManager.verticalInput;
        targetDir = targetDir + cameraObject.right * inputManager.horizontalInput;
        targetDir.y = 0;
        targetDir.Normalize();
        //Prevent the player from looking up

        //If we aren't moving, we should reset the player's direction
        //This helps keep track of where the player was originally looking
        if (targetDir.sqrMagnitude > 0.001f)
            //Keep player's original direction
            LastDir = targetDir;
        else
            targetDir = LastDir;

        //Set the target rotation the player should look at
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpd * Time.deltaTime);
        
        //Find the floor's current normal for the player to angle itself on
        Vector3 currentNormal = Vector3.Slerp(Vector3.up, normal, 60f * Time.deltaTime);

        //Set player rotation based on the normal
        transform.rotation = Quaternion.FromToRotation(Vector3.up, currentNormal) * playerRotation;
        
        //This script functions as where the player looks at. The player has a
        //box to show which direction the player is currently looking at, making
        //it more realistic for where the player is facing.
    }

    private void HandleAllCollision()
    {
        //Raycast a value
        RaycastHit groundHit;
        RaycastHit roofHit;
        
        //Create a boolean for where it is true if we are touching the ground
        //Raycast works as a look system, where it checks if it is touching something and makes sure where to look for it
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out groundHit, rayCastSize);

        if (isGrounded)
        {
            //Find the angle of the floor
            //The normal is where the player finds the right gravity to move on
            normal = groundHit.normal;
            Angle = Vector3.Angle(groundHit.normal, Vector3.up);
        }
        else
            //We are in the air
            normal = Vector3.up;

        //Create a boolean for if we have hit a roof
        //The raycast is point above the player so that it can properly see if we are seeing a roof or not
        isTouchingRoof = Physics.Raycast(transform.position, Vector3.up, out roofHit, rayCastSize / 1.8f);

        //Force the player to be falling down based on the scale
        //This foces the player to go downwards realistically like in real life
        plrRigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }
}
