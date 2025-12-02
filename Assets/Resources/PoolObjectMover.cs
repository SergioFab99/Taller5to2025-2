using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolObjectMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    void OnEnable()
    {
        direction = UnityEngine.Random.onUnitSphere;
        direction.y = 0;
        speed = UnityEngine.Random.Range(2f, 7f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
