using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public GameObject model;
    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    //Grab an event for the player
    //This can be used anywhere, meaning anything can be part of a function

    // Update is called once per frame
    void Update()
    {
        //Rotate the model
        model.transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Make sure that the PLAYER is touching the object
        if (other.CompareTag("Player"))
        {
            //Send out the event
            Actions.OnRingCollect(1);
            Destroy(gameObject); //Destroy's itself
        }
    }
}
