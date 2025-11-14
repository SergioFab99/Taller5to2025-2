using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
public class ShakeCamera : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin cinemachinePCamera;
    public void OnShakeTrigger(CameraShakeArgs args)
    {
        cinemachinePCamera.AmplitudeGain = args.amount;
        StartCoroutine(GraduallyReduceShake(args, cinemachinePCamera));
    }

    private IEnumerator GraduallyReduceShake(CameraShakeArgs args, CinemachineBasicMultiChannelPerlin noise)
    {
        var time = args.time;
        var reductionFactor = cinemachinePCamera.AmplitudeGain / time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            cinemachinePCamera.AmplitudeGain -= reductionFactor * Time.deltaTime;
            yield return null;
        }
        cinemachinePCamera.AmplitudeGain = 0;
    }
}

public class CameraShakeArgs
{
    public float amount { get; }
    public float time { get; }

    public CameraShakeArgs(float amount, float time)
    {
        this.amount = amount;
        this.time = time;
    }
}
