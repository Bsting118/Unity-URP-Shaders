using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************
* Script: SackboyController                     *
* Purpose: Allow player to move sackboy object. *
* Programmed by: Brendan Sting                  *
* Date: 3-13-2023                               *
************************************************/
public class SackboyController : MonoBehaviour
{
    //The sackboy character's rigid body:
    private Rigidbody rigbod;

    public GameObject sackboyTargetModel;

    //Inspector variable used to set the movement intensity
    public float movementIntensity = 1;

    //Inspector variable used to set the jump height intensity
    public float jumpIntensity = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize sackboy's rigid value in this script right away:
        rigbod = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //If player presses the right D-key, move them right (2D perspective like Little Big Planet) <-- relative to player object
        if (Input.GetKey(KeyCode.D))
        {
            sackboyTargetModel.transform.Translate(0, 0, (0.01f * movementIntensity));
        }

        //If player presses the left A-key, move them left (2D perspective like Little Big Planet) <-- relative to player object
        if (Input.GetKey(KeyCode.A))
        {
            sackboyTargetModel.transform.Translate(0, 0, (-0.01f * movementIntensity));
        }

        //If player presses the W OR the Space key, make them jump up
        if ((Input.GetKeyDown(KeyCode.W)) || Input.GetKeyDown(KeyCode.Space))
        {
            rigbod.AddForce(sackboyTargetModel.transform.up * jumpIntensity, ForceMode.Impulse);
        }
    }
}
