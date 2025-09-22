using UnityEngine;

public class CameraSpring : MonoBehaviour
{

    public float damping = 0.8f;
    public float frecuency = 2f;

    SpringUtils.tDampedSpringMotionParams  springParams = new SpringUtils.tDampedSpringMotionParams();

    private Vector3 velocity;
    private Vector3 targetPosition;

    // Update is called once per frame
    public void UpdateSpring(Transform camera, float deltaTime)
    {
        targetPosition = camera.position;
        transform.rotation = camera.transform.rotation;
        var currentPos = transform.position;

        SpringUtils.CalcDampedSpringMotionParams(ref springParams, deltaTime, frecuency, damping);

         SpringUtils.UpdateDampedSpringMotion(ref currentPos.x, ref velocity.x, targetPosition.x, springParams);

         SpringUtils.UpdateDampedSpringMotion(ref currentPos.y, ref velocity.y, targetPosition.y, springParams);
         SpringUtils.UpdateDampedSpringMotion(ref currentPos.z, ref velocity.z, targetPosition.z,springParams);

        transform.position = new Vector3(currentPos.x,currentPos.y,currentPos.z);
    }
}

