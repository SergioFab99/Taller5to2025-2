using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
public class CameraShake : MonoBehaviour
{
    CinemachineImpulseSource impulse;
    public static CameraShake cameraShakeInstance;
    public bool wantShake;
    private NoiseSettings[] mySignals;
    private void Awake()
    {
        cameraShakeInstance = this;
    }
    public static CameraShake GetInstance()
    {
        return cameraShakeInstance;
    }
    void Start()
    {
        impulse = GetComponent<CinemachineImpulseSource>();
    }


    public void Shake(float shakeForce, Vector3 velocity)
    {
        if(wantShake)
        impulse.GenerateImpulse(shakeForce);
        impulse.DefaultVelocity = velocity;
        //impulse.ImpulseDefinition.RawSignal
    }
}
