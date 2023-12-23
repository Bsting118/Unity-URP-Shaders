using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObject : MonoBehaviour
{
    //SerializeField = allows editing private stuff in Inspector editor WITHOUT changing other fields elsewhere (good practice!)
    [SerializeField] private float noiseStrength = 0.25f;
    [SerializeField] private float objectHeight = 1.0f;

    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        var time = Time.time * Mathf.PI * 0.25f;

        float height = transform.position.y;
        height += Mathf.Sin(time) * (objectHeight / 2.0f);
        SetHeight(height);

    }

    private void SetHeight(float height)
    {
        material.SetFloat("_CutoffHeight", height);
        material.SetFloat("_NoiseStrength", noiseStrength);
    }
}
