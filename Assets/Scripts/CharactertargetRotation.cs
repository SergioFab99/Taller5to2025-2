using UnityEngine;

public class CharacterTarget : MonoBehaviour
{


    public void Initialize(Transform camera)
    {

        var rotation = camera.transform.rotation;
        transform.rotation = rotation;

    }

    public void UpdateRotation(Transform camera)
    {

        var rotation = camera.transform.rotation;
        transform.rotation = rotation;
    }
}
