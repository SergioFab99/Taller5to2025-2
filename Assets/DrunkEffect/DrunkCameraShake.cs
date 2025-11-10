using UnityEngine;

public class DrunkCameraShake : MonoBehaviour
{
    [Header("Magnitude & Frequencies")]
    public bool isDrunk = false;
    public float rotationIntensity = 15f;
    public float positionIntensity = 0.25f;
    public float frequency = 2f;

    private Quaternion originalRot;
    private Vector3 originalPos;
    private float timeVar;

    void Start()
    {
        isDrunk = false;
        originalRot = transform.localRotation;
        originalPos = transform.localPosition;
        timeVar = UnityEngine.Random.Range(0f, 100f);
    }

    void Update()
    {
        if (isDrunk)
        {
            float rotX = Mathf.Sin(Time.time * frequency) * rotationIntensity;
            float rotY = Mathf.PerlinNoise(Time.time * 0.3f + timeVar, 0) * rotationIntensity - rotationIntensity * 0.5f;
            float rotZ = Mathf.Cos(Time.time * frequency * 0.7f) * (rotationIntensity * 0.7f);

            transform.localRotation = originalRot * Quaternion.Euler(rotX, rotY, rotZ);

            float posX = Mathf.Sin(Time.time * (frequency * 0.5f) + timeVar) * positionIntensity;
            float posY = Mathf.Sin(Time.time * (frequency * 0.85f) + timeVar) * positionIntensity;
            float posZ = Mathf.Cos(Time.time * (frequency * 0.6f) + timeVar) * (positionIntensity * 0.6f);

            transform.localPosition = originalPos + new Vector3(posX, posY, posZ);
        }
        else
        {
            transform.localRotation = originalRot;
            transform.localPosition = originalPos;
        }
    }

    public void ActivateDrunk(float duration = 0f)
    {
        isDrunk = true;
        Debug.Log("Efecto borracho activo");
    }

    public void DeactivateDrunk()
    {
        isDrunk = false;
        transform.localRotation = originalRot;
        transform.localPosition = originalPos;
        Debug.Log("Efecto borracho desactivado");
    }
}
