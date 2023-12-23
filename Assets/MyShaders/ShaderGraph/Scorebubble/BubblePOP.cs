using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
* Script: BubblePOP                                   *
* Purpose: Modify and affect the score bubble shader. *
* Programmed by: Brendan Sting                        *
* Date: 3-13-2023                                     *
******************************************************/
public class BubblePOP : MonoBehaviour
{
    //This score bubble object's assigned material (keep it private cause pre-fab stuff, ya know?)
    private Material bubbleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //Here, .sharedMaterial changes the appearance of all objects using this material, but only want 1 bubble to pop so only use .material
        bubbleMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This will detect, using Colliders and RigidBody, when the bubble and the player object touch;
    //In that instance, the core and other effects can be affected
    private void OnTriggerEnter(Collider other)
    {
        //Make bubble core disappear or have an opacity of 0%:
        bubbleMaterial.SetFloat("_GlowStrength", 0);
    }
}
