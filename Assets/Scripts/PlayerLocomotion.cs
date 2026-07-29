using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDir;
    Transform cameraObject;
    Rigidbody plrRigidbody;

    public float movementSpd = 7;
    public float rotationSpd = 15;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        plrRigidbody = GetComponent<Rigidbody>();
    }

    public void HandleMovement()
    {
        moveDir = cameraObject.forward * inputManager.verticalInput;
        moveDir = moveDir + cameraObject.right * inputManager.horizontalInput;
        moveDir.Normalize();
        moveDir.y = 0;
        moveDir = moveDir * movementSpd;

        Vector3 movementVel = moveDir;
        plrRigidbody.velocity = movementVel;
    }

    public void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;

        targetDir = cameraObject.forward * inputManager.verticalInput;
        targetDir = targetDir + cameraObject.right * inputManager.horizontalInput;
        targetDir.Normalize();
        targetDir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpd * Time.deltaTime);
    }
}
