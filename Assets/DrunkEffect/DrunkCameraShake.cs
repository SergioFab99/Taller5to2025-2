using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DrunkCameraShake : MonoBehaviour
{
    public bool isDrunk = false;
    public float drunkIntensity = 1f;
    public float drunkFrequency = 1f;

    private Quaternion originalRot;

    void Start()
    {
        originalRot = transform.localRotation;
    }

    void Update()
    {
        if (isDrunk)
        {
            float x = Mathf.Sin(Time.time * drunkFrequency) * drunkIntensity;
            float y = Mathf.PerlinNoise(Time.time, 0) * drunkIntensity * 0.5f; 
            float z = Mathf.Sin(Time.time * drunkFrequency * 0.8f) * drunkIntensity * 0.7f;

            transform.localRotation = originalRot * Quaternion.Euler(x, y, z);
        }
        else
        {
            transform.localRotation = originalRot;
        }
    }

    public void ActivateDrunk()
    {
        isDrunk = true;
    }

    public void DeactivateDrunk()
    {
        isDrunk = false;
        transform.localRotation = originalRot;
    }
}
