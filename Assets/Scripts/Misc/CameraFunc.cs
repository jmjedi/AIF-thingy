using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFunc : MonoBehaviour
{
    [SerializeField] public GameObject target;

    private void Update()
    {
        //Check if there is a target or not
        //Otherwise stop the script
        if (!target) return;

        //Check if the player is moving to the right
        if (target.transform.position.x >= 0)
            //If so, move the camera to the right, making sure that it stays with the player in the centre
            transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);

    }
}
