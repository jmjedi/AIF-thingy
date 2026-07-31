using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Grab all the other scripts from the player
    InputManager inputManager;
    PlayerLocomotion plrLocomotion;

    private void Awake()
    {
        //Set all the variables to any of the components the player has
        //This makes it easier to use while coding
        inputManager = GetComponent<InputManager>();
        plrLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        //This function updates everything inside of the input manager
        //This is to make scripting clean and easier to understand where everything is
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        //This handles everything that's inside the player, including Movement and jumping
        plrLocomotion.HandleAllMovement(inputManager.jumpInput);
    }
}
