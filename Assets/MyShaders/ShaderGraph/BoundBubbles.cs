using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

//Bubbles height must be bounded to the height of the liquid
//Bubbles visibility must be bounded to the amount of liquid (also the height)
//  -No or barely none liquid = no bubbles
//  -Some liquid = some bubbles
//  -Lots of liquid or full = lots of bubbles
//Bubbles upwards direction (rotation of VFX object) must be in sync with LiquidWobble script
//  -Wobble X is the multiplied coefficient of shader's Z axis rotation
//  -Wobble Z is the multiplied coefficient of shader's X axis rotation

public class BoundBubbles : MonoBehaviour
{
    //Global variables:
    Renderer rend;
    private Material potionLiquid;
    private float liquidHeight;
    private float bubblesHeight;
    private float cutOffIntercept;
    private float initXRotation;
    private float initZRotation;
    private float newXRotation;
    private float newZRotation;
    private Vector3 wholeXWobbleAxis;
    private Vector3 wholeZWobbleAxis;
    private float xAxisVector;
    private float zAxisVector;
    private float rotationAmt;
    private VisualEffect potionBubbles;
    public GameObject liquidObject;
    public float spawnRateBase = 16;
    public float particleDampeningToFill = 2.0f;
    public float spawnOffset = 0.0f;

    //Property variable setup for when bubbles should NOT be spawning (too full in object):
    [SerializeField, Range(0, 1)] private float _currentOverfillLimit = 0.8f;
    public float overfillBubbleCutoff
    {
        get { return _currentOverfillLimit; }
        set { _currentOverfillLimit = Mathf.Clamp(value, 0, 1); }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //Initialize shader-material stuff as well as VFX stuff for bubbles
        potionBubbles = GetComponent<VisualEffect>();
        potionLiquid = liquidObject.GetComponent<MeshRenderer>().material;
        liquidHeight = potionLiquid.GetFloat("_Fill");
        cutOffIntercept = potionLiquid.GetFloat("_CutoffRange");
        //Use this formula (taking range modifier away from liquid height) to get height at which bubbles should be:
        bubblesHeight = liquidHeight - cutOffIntercept;
        this.transform.localPosition = new Vector3(potionBubbles.transform.localPosition.x, bubblesHeight, potionBubbles.transform.localPosition.z);

        //Amount of bubbles determined:
        determineBubbleAmount(potionBubbles, spawnRateBase, liquidHeight, particleDampeningToFill, overfillBubbleCutoff);

        //Find what level of liquid bubbles should start spawning at:
        determineBubbleSurfaceSpawn(potionBubbles, spawnOffset, liquidHeight);

        //Make closely significant to 0 so multiplication has effect (0 * wobble = 0, so needed a base)
        initXRotation = 0.01f;
        initZRotation = 0.01f;
        this.transform.localRotation = Quaternion.Euler(initXRotation, (potionBubbles.transform.localRotation.y), initZRotation);

        //Initialize private variables just in case of any future start routines
        wholeXWobbleAxis = Vector3.Normalize(potionLiquid.GetVector("_XWobbleAxis"));
        xAxisVector = wholeXWobbleAxis.x;
        wholeZWobbleAxis = Vector3.Normalize(potionLiquid.GetVector("_ZWobbleAxis"));
        zAxisVector = wholeZWobbleAxis.z;
        rotationAmt = potionLiquid.GetFloat("_RotationAmount");
    }

    // Update is called once per frame
    void Update()
    {
        //Finding height of the bubbles relative to height of the liquid (shader):
        liquidHeight = potionLiquid.GetFloat("_Fill");
        cutOffIntercept = potionLiquid.GetFloat("_CutoffRange");
        bubblesHeight = liquidHeight - cutOffIntercept;
        this.transform.localPosition = new Vector3(potionBubbles.transform.localPosition.x, bubblesHeight, potionBubbles.transform.localPosition.z);

        //Amount of bubbles determined:
        determineBubbleAmount(potionBubbles, spawnRateBase, liquidHeight, particleDampeningToFill, overfillBubbleCutoff);

        //Find what level of liquid bubbles should start spawning at:
        determineBubbleSurfaceSpawn(potionBubbles, spawnOffset, liquidHeight);

        //Finding rotation of the bubbles using LiquidWobble's calculations for the shader (so it moves with it):
        wholeXWobbleAxis = Vector3.Normalize(potionLiquid.GetVector("_XWobbleAxis"));
        xAxisVector = wholeXWobbleAxis.x;
        wholeZWobbleAxis = Vector3.Normalize(potionLiquid.GetVector("_ZWobbleAxis"));
        zAxisVector = wholeZWobbleAxis.z;
        rotationAmt = potionLiquid.GetFloat("_RotationAmount");

        //Normalize the wobble vector values:
        xAxisVector = xAxisVector / xAxisVector;
        //LiquidWobble wobbleConnection = new LiquidWobble();
        LiquidWobble wobbleConnectionV2 = liquidObject.GetComponent<LiquidWobble>();
        newXRotation = (wobbleConnectionV2.getWobbleAmtX() * xAxisVector * rotationAmt) + initXRotation;
        newZRotation = (wobbleConnectionV2.getWobbleAmtZ() * zAxisVector * rotationAmt) + initZRotation;

        //newXRotation = (wobbleConnectionV2.getWobbleAmtX() * 1 * 90) + initXRotation;

        this.transform.localRotation = Quaternion.Euler(newXRotation, (potionBubbles.transform.localRotation.y), newZRotation);

       
    }

    private void determineBubbleAmount(VisualEffect theBubbleParticles, float baseSpawnRate, float heightOfLiquid, float dampeningFactor, float overfillLimit)
    {
        //Amount of bubbles determined:
        if (heightOfLiquid < 0.16f && (heightOfLiquid < overfillLimit)) //we must always check we aren't past the defined overfill
        {
            theBubbleParticles.SetFloat("SpawnRate", 0.0f);
        }
        else if ((heightOfLiquid >= 0.16f && heightOfLiquid < 0.333f) && (heightOfLiquid < overfillLimit))
        {
            theBubbleParticles.SetFloat("SpawnRate", (baseSpawnRate / (dampeningFactor * dampeningFactor)));
        }
        else if ((heightOfLiquid >= 0.333f && heightOfLiquid < 0.5f) && (heightOfLiquid < overfillLimit))
        {
            theBubbleParticles.SetFloat("SpawnRate", (baseSpawnRate / dampeningFactor));
        }
        else if (heightOfLiquid >= overfillLimit) //if we are overfilled then make bubbles back to 0 spawn rate
        {
            theBubbleParticles.SetFloat("SpawnRate", 0.0f);
        }
        else
        {
            theBubbleParticles.SetFloat("SpawnRate", baseSpawnRate);
        }
    }

    private void determineBubbleSurfaceSpawn(VisualEffect theBubbleParticle, float spawnOffset, float heightOfLiquid)
    {
        float newY;

        //Adding liquid height check to handle the case that the liquid is too shallow and bubbles must need a minor "boost" above liquid surface:
        if (heightOfLiquid < 0.25f)
        {
            //newY = (theBubbleParticle.transform.localPosition.y / 2) + spawnOffset + (spawnOffset); //not enough
            newY = (theBubbleParticle.transform.localPosition.y / 2) + (spawnOffset + (heightOfLiquid / 2));
        }
        else
        {
            newY = (theBubbleParticle.transform.localPosition.y / 2) + spawnOffset;
        }
        //newY = (theBubbleParticle.transform.localPosition.y / 2) + spawnOffset;

        Vector3 newSpawnHeight = new Vector3(theBubbleParticle.transform.localPosition.x, newY, theBubbleParticle.transform.localPosition.z);
        theBubbleParticle.SetVector3("SpawnCenter", newSpawnHeight);
    }
}
