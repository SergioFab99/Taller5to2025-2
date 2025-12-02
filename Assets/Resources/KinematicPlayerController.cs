using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinematicPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Player";
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);
    }
}