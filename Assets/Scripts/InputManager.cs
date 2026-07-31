using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Grab another script from the player
    PlayerControls playerControls;

    //All Stick/Input directions
    public Vector2 movementInput;
    public bool jumpInput;
    public float verticalInput;
    public float horizontalInput;

    private void OnEnable()
    {
        //Check if there isn't player controls to prevent overload
        if (playerControls == null)
        {
            //Create player controls to use
            playerControls = new PlayerControls();

            //Check if we are pressing any of the inputs
            playerControls.Gameplay.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            //Otherwise, we should not be moving at this time
            //This can prevent the inputs from saving while we are playing
            playerControls.Gameplay.Movement.canceled += i => movementInput = Vector2.zero;
            playerControls.Gameplay.Jump.started += i => jumpInput = true;
            playerControls.Gameplay.Jump.canceled += i => jumpInput = false;

        }
        //Enalbe the player controls
        playerControls.Enable();
    }

    private void OnDisable()
    {
        //Disable the player controls when the game is finished
        //This prevents the controls from being allowed while not playing
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        //This is where all the inputs for the player are being handled
        HandleMovementInput();
        //HandleJump
        //HandleActionInput
    }

    private void HandleMovementInput()
    {
        //This is where we get the left and right input of the player
        //This is so the player can read which direction it has to go
        horizontalInput = movementInput.x;
    }
}
